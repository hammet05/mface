using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Linq;
using System.Xml;
using MFaceService = Morpho.MFace.Sensor.Service;
using Morpho.MFace.Sensor.Service.Entities;
using Morpho.MFace.Sensor.Service.Enumerations;
using Morpho.MFace.Sensor.Service.Messages;
using MFace.Tool.Common.Interfaces;
using Morpho.MFace.Sensor.Service;

namespace MFace.Tool.MFaceProxy
{
	public class SensorServiceV2 : SensorService
	{
		public SensorServiceV2(ILogger logger) : base(logger)
		{
			base.mVersion = 2;
			 mISensorService = "Morpho.MFace.Sensor.Service.ISensorService";
		}

		protected override dynamic GetChannelFactory(string name)
		{
			return new ChannelFactory<MFaceService.ISensorService>(name);
		}


		protected override object CreateContentType(byte[] data, string referenceFormat)
		{
			Content content = new Content
			{
				ContentType = referenceFormat,
				Data = data
			};
			return content;
		}

		public override object CreateBiometricData(object contentType, Enumerations.SourceType sourceType)
		{
			BiometricData biometricData = new BiometricData
			{
				BiometricType = BiometricType.Face,
				Content = (Content)contentType,
				SourceType = (SourceType)Enum.Parse(typeof(SourceType), sourceType.ToString())
			};
			return biometricData;
		}

		private MethodInfo GetMethodByName(List<MethodInfo> aMethods, Type aType, string aName)
		{
			MethodInfo locale = null;

			foreach (MethodInfo method in aMethods)
			{
				if (method.Name.Equals(aName))
				{
					locale = method;
					break;
				}
			}

			return locale;
		}

		#region Configuration Operation

		public void SetIndicatorLight(Color color1, Color color2, TimeSpan time1, TimeSpan time2, Enumerations.LightMode lightMode)
		{
			SetIndicatorLightRequest request = new SetIndicatorLightRequest()
			{
				Color1 = System.Windows.Media.Color.FromArgb(color1.A, color1.R, color1.G, color1.B).ToString(),
				Color2 = System.Windows.Media.Color.FromArgb(color2.A, color2.R, color2.G, color2.B).ToString(),
				LightMode = (LightMode)lightMode,
				Time1 = time1,
				Time2 = time2
			};
			var response = this.service.SetIndicatorLight(request);
			commonResponseCheck(response.CommonResponse);
		}
		#endregion

		#region Maintenance operations 

		public Responses.GetSoftwareUpgradeResultResponse GetSoftwareUpgradeResult()
		{
			IdentifyRequest r = new IdentifyRequest();
			GetSoftwareUpgradeResultRequest request = new GetSoftwareUpgradeResultRequest();
			GetSoftwareUpgradeResultResponse response = this.service.GetSoftwareUpgradeResult(request);
			Responses.GetSoftwareUpgradeResultResponse result = new Responses.GetSoftwareUpgradeResultResponse(response);
			commonResponseCheck(new Types.CommonResponse(response.CommonResponse));
			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				logger.Log(LogLevel.DEBUG, $"Result {GetCurrentMethodName()}, New Version: {response.NewVersion}, Old Version: {response.OldVersion}, Status: {response.Status}, DisplaySoftwareVersion: {response.DisplayInfo.SoftwareVersion}");
			}
			return result;
		}

		public string DeployAlgorithmFile(Stream algorithmFile, string fileName, Enumerations.AlgorithmFileType fileType)
		{			
			DeployAlgorithmFileRequest request = new DeployAlgorithmFileRequest()
			{
				AlgorithmFile = algorithmFile,
				FileName = fileName,
				//FileType = (AlgorithmFileType)fileType
			};

			var response = this.service.DeployAlgorithmFile(request);
			commonResponseCheck(new Types.CommonResponse(response.CommonResponse));
			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				logger.Log(LogLevel.INFO, $"End {GetCurrentMethodName()}, CaptureToken: {response.ConfirmationToken}");
			}

			return response.ConfirmationToken;
		}

		public void StartSoftwareUpgrade(string confirmationToken)
		{
			logger.Log(LogLevel.DEBUG, $"Send {GetCurrentMethodName()}, ConfirmationToken: {confirmationToken}");

			StartSoftwareUpgradeRequest request = new StartSoftwareUpgradeRequest();
			{
				request.ConfirmationToken = confirmationToken;
			}
			var response = this.service.StartSoftwareUpgrade(request);
			commonResponseCheck(new Types.CommonResponse(response.CommonResponse));
		}

		public string UploadSoftwareUpgrade(Stream upgradePackage)
		{
			logger.Log(LogLevel.DEBUG, $"Send {GetCurrentMethodName()}, : {upgradePackage.Length}");

			UploadSoftwareUpgradeRequest request = new UploadSoftwareUpgradeRequest();
			UploadSoftwareUpgradeResponse response = this.service.UploadSoftwareUpgrade(request);
			request.UpgradePackage = upgradePackage;
			commonResponseCheck(new Types.CommonResponse(response.CommonResponse));
			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				logger.Log(LogLevel.INFO, $"End {GetCurrentMethodName()} , ConfirmationToken: {response.ConfirmationToken}");
			}

			return response.ConfirmationToken;
		}

		public void StartSystemCalibration()
		{
			logger.Log(LogLevel.DEBUG, $"Send {GetCurrentMethodName()}");

			StartSystemCalibrationRequest request = new StartSystemCalibrationRequest();
			StartSystemCalibrationResponse response = this.service.StartSystemCalibration(request);
			commonResponseCheck(new Types.CommonResponse(response.CommonResponse));
		}

		public string[] GetServiceLog(Enumerations.LogType logType, DateTimeOffset startDate, DateTimeOffset endDate, string captureToken)
		{
			var response = (GetServiceLogResponse)CallMFaceService(Args(new KeyValuePair<string, object>("LogType", logType),
														new KeyValuePair<string, object>("StartDate", new DateTimeOffsetIsoXmlSerializable(startDate)),
														new KeyValuePair<string, object>("EndDate", new DateTimeOffsetIsoXmlSerializable(endDate)),
														new KeyValuePair<string, object>("CaptureToken", captureToken)));


			string[] result = new string[0];
			if (response.Records != null && response.Records.Length != 0)
			{
				result = new string[response.Records.Count()];
				for (int i = 0; i < response.Records.Count(); i++)
				{
					result[i] = $"{response.Records[i].Timestamp}-{response.Records[i].Severity}:{response.Records[i].Message}";
				}

				logger.Log(LogLevel.INFO, $"Result {GetCurrentMethodName()} for {logType}, LogRecord Count: {result.Count()}");
				return result;
			}
			else
			{
				logger.Log(LogLevel.INFO, $"Result {GetCurrentMethodName()} for {logType} is empty.");
				return new string[0];
			}
		}

		public byte[] GetBigDump(string captureToken, uint desiredFramesNumber)
		{
			dynamic response = CallMFaceService(Args(new KeyValuePair<string, object>("CaptureToken", captureToken),
														new KeyValuePair<string, object>("DesiredFramesNumber", desiredFramesNumber)));

			logger.Log(LogLevel.DEBUG, $"Get {GetCurrentMethodName()} result , BigDump size: {response.BigDump.Data.Length}B");
			return response.BigDump.Data;
		}
		#endregion

		#region Capture Operations
		public void ActivateAutoStopCapture()
		{
			ActivateAutoStopCaptureRequest request = new ActivateAutoStopCaptureRequest();
			ActivateAutoStopCaptureResponse response = this.service.ActivateAutoStopCapture(request);
			commonResponseCheck(new Types.CommonResponse(response.CommonResponse));
		}

		public void StartSleepMode()
		{
			var response = this.service.StartSleepMode();
			commonResponseCheck(new Types.CommonResponse(response.CommonResponse));
		}

		public void StopSleepMode()
		{
			var response = this.service.StopSleepMode();
			commonResponseCheck(new Types.CommonResponse(response.CommonResponse));
		}


		public Byte[] GetSleepModeBigDump(uint desiredFramesNumber)
		{
			GetSleepModeBigDumpRequest request = new GetSleepModeBigDumpRequest();
			{
				request.DesiredFramesNumber = desiredFramesNumber;
			}
			var response = this.service.GetSleepModeBigDump(request);
			commonResponseCheck(new Types.CommonResponse(response.CommonResponse));
			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				logger.Log(LogLevel.INFO, $"Result {GetCurrentMethodName()}, BigDumpData: {response.BigDump.Data}");
			}
			return response.BigDump.Data;
		}


		public List<Types.PersonData> GetCaptureResult(string captureToken, int operationTimeout, int captureTimeout, bool autoStopCapture, bool getCroppedImage, bool getTemplate, bool getImage)
		{
			List<Types.PersonData> resultPersonDatas = new List<Types.PersonData>();
			dynamic response = CallMFaceService(Args(new KeyValuePair<string, object>("CaptureToken", captureToken),
														new KeyValuePair<string, object>("CaptureTimeout", TimeSpan.FromMilliseconds(captureTimeout)),
														new KeyValuePair<string, object>("OperationTimeout", TimeSpan.FromMilliseconds(operationTimeout)),
														new KeyValuePair<string, object>("getImage", getImage),
														new KeyValuePair<string, object>("GetCroppedImage", getCroppedImage),
														new KeyValuePair<string, object>("GetTemplate", getTemplate),
														new KeyValuePair<string, object>("AutoStopCapture", autoStopCapture)));

			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				foreach (PersonData data in response.PersonDataList)
				{
					resultPersonDatas.Add(new Types.PersonData(data));
				}
				string message = $"Result GetCaptureResult, ";
				message += $"Detected: { resultPersonDatas.Count(x => x.PersonStatus == Enumerations.PersonStatus.Detected)}, ";
				message += $"ReadyForVerificationDelayed: { resultPersonDatas.Count(x => x.PersonStatus == Enumerations.PersonStatus.ReadyForVerificationDelayed)}, ";
				message += $"ReadyForVerification: { resultPersonDatas.Count(x => x.PersonStatus == Enumerations.PersonStatus.ReadyForVerification)}, ";
				message += $"Gone: { resultPersonDatas.Count(x => x.PersonStatus == Enumerations.PersonStatus.Gone)}";
				logger.Log(LogLevel.INFO, message);
			}
			return resultPersonDatas;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="captureToken"></param>
		/// <param name="sourceType"></param>
		/// <param name="reference"></param>
		/// <param name="referenceFormat"></param>
		/// <returns></returns>
		#endregion


		#region Maintenance Operation

		public void SetSystemTime(DateTimeOffset dateTime, string timeZoneName)
		{
			CallMFaceService(Args(new KeyValuePair<string, object>("DateTime", new DateTimeOffsetIsoXmlSerializable(dateTime)),
									new KeyValuePair<string, object>("TimeZoneName", timeZoneName)));
		}
		#endregion

	}
}
