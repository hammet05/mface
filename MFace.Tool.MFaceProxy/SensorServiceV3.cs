namespace MFace.Tool.MFaceProxy
{
	using Common.Interfaces;
	using MFaceServiceV3Kit;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Runtime.CompilerServices;
	using System.ServiceModel;
	using System.Xml;

	public class SensorServiceV3 : SensorService//,ISensorServiceV3
	{

		private Dictionary<string, string> configurationTable = new Dictionary<string, string>()
		{
			{ "Display.LiveCameraId","live.cameraId" }
		};

		public SensorServiceV3(ILogger logger) : base(logger)
		{
			base.mVersion = 3;
			mISensorService = "MFace.Tool.MFaceProxy.MFaceServiceV3Kit.ISensorService";
		}

		protected override dynamic GetChannelFactory(string name)
		{
			return new ChannelFactory<MFaceServiceV3Kit.ISensorService>(name);
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
				SourceType = (SourceType)Enum.Parse(typeof(SourceType), sourceType.ToString()),
				BiometricTypeSpecified = true,
				SourceTypeSpecified = true				
			};
			return biometricData;
		}



		#region Capture Operations
		/// <summary>
		/// This operation retrieves biometric data of possibly different people from the ongoing or the latest 
		/// completed capture process.
		/// </summary>
		/// <param name="captureToken">The capture token to retreive result from.</param>
		/// <param name="timeout">The amount of time this operation is going to wait for a new biometric data</param>		
		/// <param name="autoStopCapture">Whether the capture workflows should be automatically stopped when sufficient biometric data had been acquired.</param>
		/// <param name="getCroppedImage">Whether to retrieve cropped image arround the face that has been detected</param>
		/// <param name="getTemplate">Whether to retrieve the biometric template from the capture.</param>
		/// <param name="getImage">Whether to retrieve the image from the capture.</param>
		/// <returns>List of PersonData</returns>
		public List<Types.PersonData> GetCaptureResult(string captureToken,int operationTimeout, int captureTimeout, bool autoStopCapture, bool getCroppedImage, bool getTemplate, bool getImage)
		{
			var response = (GetCaptureResultResponse)CallMFaceService(Args(new KeyValuePair<string, object>("CaptureToken", captureToken),
											new KeyValuePair<string, object>("Timeout", XmlConvert.ToString(TimeSpan.FromMilliseconds(captureTimeout))),
											new KeyValuePair<string, object>("GetImage", getImage),
											new KeyValuePair<string, object>("GetCroppedImage", getCroppedImage),
											new KeyValuePair<string, object>("GetTemplate", getTemplate),
											new KeyValuePair<string, object>("AutoStopCapture", autoStopCapture)));

			List<Types.PersonData> resultPersonDatas = new List<Types.PersonData>();

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
		#endregion Capture Operations

		#region Maintenance Operation
		/// <summary>
		/// This operation retrieves a set of service log records stored by the service.
		/// Please note that log records may be discarded by the service on a first-in first-out basis 
		/// to preserve space depending on service configuration parameters
		/// </summary>
		/// <param name="logType">The log type identifier to query.</param>
		/// <param name="startDate">Start date of the asked log</param>
		/// <param name="endDate">End date of the asked log</param>
		/// <param name="captureToken">CaptureToken of the asked log</param>
		public string[] GetServiceLog(Enumerations.LogType logType, DateTimeOffset startDate, DateTimeOffset endDate, string captureToken)
		{
			LogType v3logType = LogType.Acquisition;


			switch (logType)
			{
				case Enumerations.LogType.LibMorphoWay:
					v3logType = LogType.Acquisition;
					break;
				case Enumerations.LogType.MorphoLite:
					v3logType = LogType.CodingMatching;
					break;
				case Enumerations.LogType.Operational:
					v3logType = LogType.Operational;
					break;
				case Enumerations.LogType.Technical:
					throw new ArgumentException("Technical log not available in MFaceServiceV3Kit");
			}

			GetServiceLogRequest request = new GetServiceLogRequest();

			var response = (GetServiceLogResponse)CallMFaceService(Args(new KeyValuePair<string, object>("CaptureToken", captureToken.ToUpper()),
																			new KeyValuePair<string, object>("EndDate", endDate.DateTime),
																			new KeyValuePair<string, object>("StartdATE", startDate.DateTime),
																			new KeyValuePair<string, object>("LogType", v3logType)));

			if(response.Log != "")
			{
				string[] result = response.Log.Split('\n');
				logger.Log(LogLevel.INFO, $"Result {GetCurrentMethodName()} for {v3logType}, LogRecord Count: {result.Count()}");

				return result;
			}
			else
			{
				logger.Log(LogLevel.INFO, $"Result {GetCurrentMethodName()} for {v3logType} is empty.");
				return new string[0];
			}
		}

		public byte[] GetBigDump(string captureToken, uint desiredFramesNumber)
		{
			dynamic response = CallMFaceService(Args(new KeyValuePair<string, object>("CaptureToken", captureToken),
														new KeyValuePair<string, object>("FrameOffset", (UInt64?)desiredFramesNumber)));
			

			logger.Log(LogLevel.DEBUG, $"Get {GetCurrentMethodName()} result , BigDump size: {response.BigDump.Data.Length}B");
			return response.BigDump.Data;
		}

		

		public new void SetConfiguration(Dictionary<string, string> configurations)
		{
			SetConfigurationRequest r = new SetConfigurationRequest();
			r.Configuration = new ArrayOfKeyValueOfstringstringKeyValueOfstringstring[configurations.Count];
			for(int i = 0; i < configurations.Count();i++)
			{
				string key = configurations.ElementAt(i).Key;
				//Convert V2 to V3 Key
				if (configurationTable.ContainsKey(configurations.ElementAt(i).Key))
				{
					key = configurationTable[configurations.ElementAt(i).Key];
				}

				r.Configuration[i] = new ArrayOfKeyValueOfstringstringKeyValueOfstringstring
				{
					Key = key,
					Value = configurations.ElementAt(i).Value
				};
			}
			var response = (SetConfigurationResponse)service.SetConfiguration(r);
			commonResponseCheck(new Types.CommonResponse(response.CommonResponse));

		}

	/*public List<Types.AlarmRecord> GetServiceAlarm()
		{
			GetServiceAlarmRequest r = new GetServiceAlarmRequest();
			dynamic response = service.GetServiceAlarm(r);

			List<Types.AlarmRecord> result = new List<Types.AlarmRecord>();
			string message = String.Empty;

			foreach (var record in response.AlarmRecords)
			{
				result.Add(new Types.AlarmRecord(record));
				message += $"{record.Source},";
			}

			//logger.Log(LogLevel.INFO, $"Result {GetCurrentMethodName()}, {message}");
			return result;
		}*/


		#endregion
	}
}
