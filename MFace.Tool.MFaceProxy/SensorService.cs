using MFace.Tool.Common.Interfaces;
using MFace.Tool.MFaceProxy.Enumerations;
using MFace.Tool.MFaceProxy.Responses;
using MFace.Tool.MFaceProxy.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;
using ServiceV2 = Morpho.MFace.Sensor;

namespace MFace.Tool.MFaceProxy
{
	public abstract class SensorService : Service
	{

		#region Members

		public static BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
		private string mVersionString = String.Empty;
		internal string mISensorService;
		private List<MethodInfo> mMethods;
		private Type mType;
		private States _currentState = States.None;
		protected bool isInitialize;
		protected ushort mVersion;
		Assembly mAssemblyVersion;
		public enum States
		{
			None,
			/// <summary>Initilize in progress</summary>
			Initializing,
			/// <summary>Sensor initialize and ready to be used</summary>
			Initialize,
			/// <summary>UnInitializing in progress</summary>
			UnInitializing,
			UnInitialize,
			Canceling,
			Ready,
			Detection,
			Detected,
			Capturing,
			CaptureDoneSucessfull,
			CaptureDoneFail,
			Identify,
			IdentifyDoneSuccess,
			IdentifyDoneFail
		}
		public delegate void OnNewStateDelegate(States state);
		public event OnNewStateDelegate OnNewState;
		public delegate void CommonResponseCheckDelegate(CommonResponse commonReponse);

		#endregion

		public SensorService(ILogger logger) : base(logger)
		{
			isInitialize = false;
		}
		public bool IsInitialize
		{
			get { return isInitialize; }
		}
		public ushort Version
		{
			get
			{
				return mVersion;
			}
		}
		public Assembly AssemblyVersion
		{
			get
			{
				switch (Version)
				{
					case 2:
						mAssemblyVersion = Assembly.GetAssembly(typeof(Morpho.MFace.Sensor.Service.ISensorService));							
						break;
					case 3:
						mAssemblyVersion = Assembly.GetAssembly(typeof(MFaceServiceV3Kit.ISensorService));
						break;
				}
				return mAssemblyVersion;
			}

		}


		protected void InnerNewState(States newState)
		{
			_currentState = newState;
			if (OnNewState != null)
				OnNewState.Invoke(_currentState);
		}

		private Type SensorServiceType
		{
			get
			{
				if (mType == null)
				{
					mType = AssemblyVersion.GetType(mISensorService);
				}
				return mType;
			}
		}
		private Type GetTypeByName(Assembly aAssemblyVersion, string aTypeName)
		{
			return (from theType in aAssemblyVersion.GetTypes()
					where theType.Name.ToLower() == aTypeName.ToLower()
					select theType).FirstOrDefault();
		}
		private Type GetTypeByFullName(Assembly aAssemblyVersion, string aTypeFullName)
		{
			return (from theType in aAssemblyVersion.GetTypes()
					where theType.FullName == aTypeFullName
					select theType).SingleOrDefault();
		}
		private MethodInfo GetMethodByName(List<MethodInfo> aMethods, string aName)
		{
			MethodInfo locale = null;

			foreach (MethodInfo method in aMethods)
			{
				if (method.Name.ToLower() == aName.ToLower())
				{
					locale = method;
					break;
				}
			}



			return locale;
		}
		private List<MethodInfo> methods
		{
			get
			{
				if (mMethods == null)
				{
					mMethods = new List<MethodInfo>(SensorServiceType.GetMethods(flags));
				}
				return mMethods;
			}
		}

		protected static KeyValuePair<string,object>[] Args(params KeyValuePair<string, object>[] args)
		{
			return args;
		}

		//WORK IN PROGRESS - SHOULD REPLACE ALL CallMFaceService
		protected object CallMFaceService(KeyValuePair<string,object>[] requestParameter = null, [CallerMemberName] string methodName = "")
		{
            logger.Log(LogLevel.DEBUG, "Step 1");
            //Get the method parameter (eg: StartCapture -> StartCaptureRequest)
            //Assembly assembly = service.GetType().Assembly;
            Type request = GetTypeByName(AssemblyVersion, String.Concat(methodName, "Request"));
			object[] theArgs = null;
			string logRequestParameter = "";

			if (request != null)
			{
                logger.Log(LogLevel.DEBUG, "Step 2");
                ConstructorInfo defaultConstructor = request.GetConstructor(new Type[] { });
				object requestObject = defaultConstructor.Invoke(new object[] { });

				//if(!(requestParameter == null & request.GetFields().Count() == 0) && (request.GetFields().Count() != requestParameter?.Count()))
				//throw new ArgumentException($"Internal exception. Number of argument for dynamic call of {methodName} invalid. Expected {request.GetFields().Count()}, received {requestParameter.Count()}");

				//Affect each paramter of the request
				if (requestParameter != null)
				{
                    logger.Log(LogLevel.DEBUG, "Step 3");
                    foreach (var param in requestParameter)
					{
						logRequestParameter += param.Key + ": ";
						logRequestParameter += param.Value == null ? "empty, " : param.Value.ToString() + ", ";

						FieldInfo field = request.GetFields().Where(x => x.Name.ToLower() == param.Key.ToLower()).FirstOrDefault(); //Get the request parameter

						if (field == null)
						{
                            logger.Log(LogLevel.DEBUG, "Step 4");
                            throw new ArgumentException($"Internal exception. Argument for dynamic call of {methodName} invalid. Request parameter {param.Key} not found");
                        }
                           

						object requestParam = param.Value;


						switch (field.FieldType.ToString())
						{
							case "System.Guid":
								requestParam = new Guid(param.Value.ToString());
								break;
						}


						if (field.FieldType.ToString() == "System.String" && param.Value.GetType().ToString() == "System.TimeSpan")
						{
                            logger.Log(LogLevel.DEBUG, "Step 5"); 
							requestParam = XmlConvert.ToString((TimeSpan)param.Value);
                        }
					



						field.SetValue(requestObject, requestParam);

					}
				}
				theArgs = new object[] { requestObject };
                logger.Log(LogLevel.DEBUG, "Step 6");

            }

			//Get the method from MFace
			List<MethodInfo> oneLstMethodInfo = new List<MethodInfo>(SensorServiceType.GetMethods(flags));
			MethodInfo oneMethodInfo = GetMethodByName(oneLstMethodInfo, methodName);

			//Call MFace service with the request parameter
			if (methodName != "GetServiceAlarm") 
			logger.Log(LogLevel.DEBUG, "Step 7");
            logger.Log(LogLevel.DEBUG, $"Send {methodName} {logRequestParameter}.");
            logger.Log(LogLevel.DEBUG, "Step 8");

            try
			{
                logger.Log(LogLevel.DEBUG, "Step 9.1");
                var response = oneMethodInfo.Invoke(service, theArgs);
                logger.Log(LogLevel.DEBUG, "Step 10");
                //commonResponseCheck(new CommonResponse(response.CommonResponse), methodName);
				return response;
			}
			catch (TargetInvocationException tex)
			{
                logger.Log(LogLevel.ERROR, $"TargetInvocationException: {tex.InnerException?.Message}");
                logger.Log(LogLevel.ERROR, tex.InnerException?.StackTrace);

                // Re-lanza el error conservando contexto
                throw new Exception(
                    $"The service method '{methodName}' threw an internal exception. " +
                    $"See InnerException for details.",
                    tex.InnerException ?? tex);
                //throw new Exception($"Cannot connect to MFace.Service at {channelFactory.Endpoint.Address.ToString()}.");
            }
            catch (ArgumentException aex)
            {
                logger.Log(LogLevel.ERROR, $"ArgumentException: {aex.Message}");
                logger.Log(LogLevel.ERROR, aex.StackTrace);

                throw new ArgumentException(
                    $"Invalid arguments passed to '{methodName}'. " +
                    $"Check parameter names and types.",
                    aex);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.ERROR, $"Unhandled exception: {ex.Message}");
                logger.Log(LogLevel.ERROR, ex.StackTrace);

                // Aquí mantenemos el stacktrace original
                throw;
            }
   //         catch (Exception ex2)
			//{
   //             logger.Log(LogLevel.DEBUG, ex2.StackTrace);
   //             throw new Exception("Failing in: oneMethodInfo.Invoke(service, theArgs)");
   //         }

		}

		protected abstract object CreateContentType(byte[] data, string referenceFormat);

		public abstract object CreateBiometricData(object contentType, SourceType sourceType);
		#region ISensorService


		/// <summary>
		/// This operation initializes the sensor components of the service. 
		/// </summary>
		public bool Initialize()
		{
			InnerNewState(States.Initializing);
			dynamic response = CallMFaceService();

			if (response.CommonResponse.Status.ToString() == CommonResponseStatus.Success.ToString())
			{
				InnerNewState(States.Initialize);
				isInitialize = true;
			}
			return isInitialize;
		}

		/// <summary>
		/// This operation uninitializes the sensor components of the service.
		/// </summary>
		public void Uninitialize()
		{
			InnerNewState(States.UnInitializing);

			dynamic response = CallMFaceService();

			if (response.CommonResponse.Status.ToString() == Enumerations.CommonResponseStatus.Success.ToString())
			{
				InnerNewState(States.UnInitialize);
				isInitialize = false;
			}
		}

		#region CAPTURE OPERATION

		/// <summary>
		/// This operation starts a biometric capture process.
		/// </summary>
		/// <param name="captureTimeout">The Capture timeout</param>
		/// <param name="captureMode">The Capture mode</param>
		public string StartCapture(int captureTimeout, Enumerations.CaptureMode captureMode)
		{
			string captureToken = "";

			dynamic response = CallMFaceService(Args(	new KeyValuePair<string, object>("CaptureMode", captureMode),
														new KeyValuePair<string,object>("CaptureTimeout", TimeSpan.FromMilliseconds(captureTimeout))));
			captureToken = response.CaptureToken.ToString();

			InnerNewState(States.Capturing);

			logger.Log(LogLevel.INFO, $"Result {GetCurrentMethodName()}, CaptureToken: {response.CaptureToken}");
			return captureToken;
		}

		/// <summary>
		/// This operation starts a biometric capture process.
		/// </summary>
		/// <param name="captureTimeout">The Capture timeout</param>
		/// <param name="captureMode">The Capture mode</param>
		public string StartCapture(int captureTimeout, Enumerations.CaptureMode captureMode, byte[] reference, string referenceFormat, SourceType sourceType)
		{
			string captureToken = "";

			object contentType = CreateContentType(reference, referenceFormat);
			object biometricData = CreateBiometricData(contentType, sourceType);

			dynamic response = CallMFaceService(Args(new KeyValuePair<string, object>("CaptureMode", captureMode),
														new KeyValuePair<string, object>("CaptureTimeout", TimeSpan.FromMilliseconds(captureTimeout)),
														new KeyValuePair<string, object>("Reference", biometricData)));
			captureToken = response.CaptureToken.ToString();

			InnerNewState(States.Capturing);

			logger.Log(LogLevel.INFO, $"Result {GetCurrentMethodName()}, CaptureToken: {response.CaptureToken}");
			return captureToken;
		}

		/// <summary>
		/// When this operation is called, all currently detected persons status are set to Gone,
		/// and all the ones that were not retrieved beforehand are ready to be retrieved by a final GetCaptureResult request.
		/// </summary>
		/// <param name="captureToken">Capture process token used to stop currently running capture.</param>
		public void StopCapture(string captureToken)
		{
			CallMFaceService(Args(new KeyValuePair<string, object>("CaptureToken",captureToken)));
		}

		/// <summary>
		/// This operation is used to cancel an ongoing operation.This includes notably the 
		/// StartCapture operation (and any concurrent biometric operation).
		/// </summary>
		public void Cancel()
		{
			CallMFaceService();
		}

		public short SetReference(string captureToken, Enumerations.SourceType sourceType, byte[] reference, string referenceFormat)
		{
			object biometricData = null;
			if (reference != null)
			{
				object contentType = CreateContentType(reference, referenceFormat);
				biometricData = CreateBiometricData(contentType, sourceType);
			}


			dynamic response = CallMFaceService(Args(	new KeyValuePair<string, object>("CaptureToken", captureToken),
														new KeyValuePair<string, object>("Reference", biometricData)));

			if (response.CommonResponse.Status.ToString() == Enumerations.CommonResponseStatus.Success.ToString())
				logger.Log(LogLevel.INFO, $"Result {GetCurrentMethodName()}, ReferenceQuality: {response.ReferenceQuality}");

			return response.ReferenceQuality;
		}
		#endregion

		/// <summary>
		/// This operation allows to ask if a presence has been detected in the section of
		/// the image defined in the configuration file. The processed images come from the camera
		/// specified in the configuration file as well. This operation works only if StartSleepMode() 
		/// has been called and StopSleepMode() has not been called since.
		/// </summary>
		/// <returns>True is a person is detected</returns>
		public bool GetPresence()
		{
			dynamic response = CallMFaceService();

			logger.Log(LogLevel.DEBUG, $"Result {GetCurrentMethodName()}, PresenceDetected: {response.PresenceDetected}");
			return response.PresenceDetected;
		}

		/// <summary>
		/// This operation provides information about the currently active service alarms
		/// </summary>
		/// <returns>List of AlarmRecord currently active</returns>
		public virtual List<AlarmRecord> GetServiceAlarm()
		{
			dynamic response = CallMFaceService();

			List<AlarmRecord> result = new List<AlarmRecord>();
			string message = String.Empty;

			foreach (var record in response.AlarmRecords)
			{
				result.Add(new Types.AlarmRecord(record));
				message += $"{record.Source},";
			}

			//logger.Log(LogLevel.INFO, $"Result {GetCurrentMethodName()}, {message}");
			return result;
		}

		/// <summary>
		/// This operation gives information about Licences installed. 
		/// </summary>
		/// <returns></returns>
		public GetLicensesInfoResponse GetLicensesInfo()
		{
			dynamic response = CallMFaceService();
			GetLicensesInfoResponse result = new GetLicensesInfoResponse();
			if (response.CommonResponse.Status.ToString() == Enumerations.CommonResponseStatus.Success.ToString())
			{
				result = new GetLicensesInfoResponse(response.Serial, response.InstalledLicenses);
				logger.Log(LogLevel.DEBUG, $"Result {GetCurrentMethodName()}, InstalledLicenses: {String.Join(",",result.Installedicences)}, Serial: {response.Serial}");
			}
			return result;
		}

		/// <summary>
		/// This operation returns Status of Service.
		/// </summary>
		/// <returns>Current service status</returns>
		public Enumerations.ServiceStatus GetServiceStatus()
		{
			dynamic response = CallMFaceService();

			logger.Log(LogLevel.DEBUG, $"Result {GetCurrentMethodName()}, Status Service is: {response.ServiceStatus}");
			return (Enumerations.ServiceStatus)response.ServiceStatus;
		}

		/// <summary>
		/// This operation retrieves statistics about the usage of the service.
		/// </summary>
		/// <returns></returns>
		public ServiceStatistics GetServicesStatistics()
		{
			dynamic response = CallMFaceService();
			ServiceStatistics result = new ServiceStatistics(response.ServiceStatistics.TotalCaptureCount, response.ServiceStatistics.TotalVerifyCount);
			logger.Log(LogLevel.DEBUG, $"Result {GetCurrentMethodName()}, TotalCaptureCount: {result.TotalCaptureCount}, TotalVerifyCount: {result.TotalVerifyCount}");
			return result;
		}

		/// <summary>
		/// This operation provides general information about the sensor and service.
		/// </summary>
		/// <returns></returns>
		public GetServiceInfoResponse GetServiceInfo()
		{
			dynamic response = CallMFaceService();

			if (response.CommonResponse.Status.ToString() == Enumerations.CommonResponseStatus.Success.ToString())
			{
				logger.Log(LogLevel.INFO, $"Response {GetCurrentMethodName()}, SoftwareVersion of Service: {response.ServiceInfo.SoftwareVersion},Alarm Severity: {response.ServiceInfo.AlarmSeverity},Servie Initialized: {response.ServiceInfo.Initialized},ServiceUptime: {response.ServiceInfo.ServiceUptime}, SoftwareVersion of Display: {response.DisplayInfo}");
			}

			return new GetServiceInfoResponse(response.ServiceInfo, response.DisplayInfo);;
		}

		/// <summary>
		/// This opeartion Perform a hardware diagnostic and returns the collected data
		/// </summary>
		/// <returns></returns>
		public List<SelfTestStatus> SelfTest()
		{
			dynamic response = CallMFaceService();

			List<SelfTestStatus> result = new List<SelfTestStatus>();

			foreach (var selfTestStatus in response.SelfTestsStatusArray)
			{
				result.Add(new SelfTestStatus(selfTestStatus.id, selfTestStatus.level));
			}
			string message = $"Result { GetCurrentMethodName()}, ";
			
			message +=$"DANGER: {String.Join(", ", result.Where(x => x.level == SelfTestLevel.DANGER).Select(x => x.id).ToList())}.";
			message +=$"WARNING: {String.Join(", ", result.Where(x => x.level == SelfTestLevel.WARNING).Select(x => x.id).ToList())}.";
			message +=$"NOK: {String.Join(", ", result.Where(x => x.level == SelfTestLevel.NOK).Select(x => x.id).ToList())}.";
			message +=$"OK: {String.Join(", ", result.Where(x => x.level == SelfTestLevel.OK).Select(x => x.id).ToList())}.";

			logger.Log(LogLevel.INFO, message);
			return result;
		}

		/// <summary>
		/// This operation is used to confirm that the secure communication is correctly enable or disable and communication is still possible between the service and its clients.
		/// </summary>
		/// <param name="confirmationToken">Token returned by EnableSecureMode or DisableSecureMode operations used to confirm the communication mode change.</param>
		public void ConfirmSecureMode(string confirmationToken)
		{
			dynamic response = CallMFaceService(Args(new KeyValuePair<string, object>("ConfirmationToken",confirmationToken)));
		}

		/// <summary>
		/// This operation is used to upload a license file onto the system, which is then deployed by Morpho Protect.
		/// </summary>
		/// <param name="data">The content attached data.</param>
		/// <param name="contentType">The MIME type of the Data attribute. </param>
		/// <param name="licenseFileName">The license file name.</param>
		public void DeployLicenseFile(byte[] data, string contentType, string licenseFileName)
		{
			var licenseFile = CreateContentType(data, contentType);
				
			CallMFaceService(Args(new KeyValuePair<string, object>("licenseFile", licenseFile),
									new KeyValuePair<string, object>("LicenseFileName", licenseFileName)));
		}

		/// <summary>
		/// This operation disables the secure mode for the communication and optionally deletes the certificate stored on the system.
		/// </summary>
		/// <param name="confirmationTimeout">The timeout for receiving the ConfirmSecureMode before reverting to the previous communication mode.</param>
		/// <returns></returns>
		public string DisableSecureMode(TimeSpan confirmationTimeout)
		{
			dynamic response = CallMFaceService(Args(new KeyValuePair<string, object>("ConfirmationTimeout", confirmationTimeout)));
			if (response.CommonResponse.Status.ToString() == Enumerations.CommonResponseStatus.Success.ToString())
			{
				logger.Log(LogLevel.DEBUG, $"Result {GetCurrentMethodName()}, ConfirmationToken: {response.ConfirmationToken}");
			}

			return response.ConfirmationToken;
		}

		/// <summary>
		/// This operation retrieves live information from an ongoing face biometric capture process.
		/// It can also be used to periodically retrieve the capture live image for display a live capture stream outside the MFACE.
		/// </summary>
		/// <param name="getImage">Whether to retrieve the live image from the camera along with the capture live information.</param>
		/// <returns></returns>
		public GetCaptureLiveDataResponse GetCaptureLiveData(bool getImage)
		{
			dynamic response = CallMFaceService(Args( new KeyValuePair<string, object>("GetImage", getImage)));

			GetCaptureLiveDataResponse result = new GetCaptureLiveDataResponse();
			if (response.CommonResponse.Status.ToString() == Enumerations.CommonResponseStatus.Success.ToString())
			{
				result = new GetCaptureLiveDataResponse(response);
			}

			return result;
		}

		/// <summary>
		/// This operation executes a 1:few identification process between the specified database and biometric data 
		/// from an ongoing or recently completed capture process.
		/// </summary>
		/// <param name="captureToken">Unique ID of the person</param>
		/// <param name="personID">Unique ID of the person</param>
		/// <param name="databaseIndex">The index of the database to match against</param>
		/// <returns>Matching score and ReferenceID of the identify person</returns>
		public IdentifyResponse Identify(string captureToken, string personId, short databaseIndex)
		{
			dynamic response = CallMFaceService(Args(new KeyValuePair<string, object>("CaptureToken", captureToken),
														new KeyValuePair<string, object>("DatabaseIndex", databaseIndex),
														new KeyValuePair<string, object>("PersonID", personId)));

			if (response.CommonResponse.Status.ToString() == Enumerations.CommonResponseStatus.Success.ToString())
				logger.Log(LogLevel.DEBUG, $"Result {GetCurrentMethodName()}, MatchingScore: {response.MatchingScore}, ReferenceID: {response.ReferenceID}");


			return new IdentifyResponse(response);;
		}

		/// <summary>
		/// This operation resets the MFACE system to its factory configuration settings.
		/// </summary>
		/// <param name="resetConfigurationKeys">Configuration settings fully qualified name to reset. Full list can be retrieve by using GetCongifuration request</param>
		public void ResetFactoryConfiguration(string[] resetConfigurationKeys)
		{
			CallMFaceService(Args(new KeyValuePair<string, object>("ResetConfigurationKeys", resetConfigurationKeys)));
		}

		public void SetSystemTime(DateTimeOffset dateTime, string timeZoneName)
		{
			CallMFaceService(Args(new KeyValuePair<string, object>("DateTime", dateTime),
									new KeyValuePair<string, object>("TimeZoneName", timeZoneName)));
		}

		/// <summary>
		/// This operation is used to upload a configuration file onto the system. The file must exist in the MFace installation. The file will be replaced.
		/// </summary>
		/// <param name="data">The content attached data.</param>
		/// <param name="configurationFileName">The configuration file name.</param>
		/// <param name="contentType">The MIME type of the Data attribute.</param>
		public void UpdateConfigurationFile(byte[] data, string configurationFileName)
		{
			var configurationFile = CreateContentType(data, "application/octet-stream");

			CallMFaceService(Args(new KeyValuePair<string, object>("ConfigurationFile", configurationFile),
									new KeyValuePair<string, object>("ConfigurationFileName", configurationFileName)));
		}

		/// <summary>
		/// This operation takes a face image and perform an analysis to evaluate a selection of ICAO criteria.
		/// Then, it returns the result (“Is the image compliant ?”), listing the criteria the image is not compliant with.
		/// </summary>
		/// <param name="sourceType">The source type of the reference</param>
		/// <param name="reference">The reference data</param>
		/// <param name="referenceFormat">The MIME type of the Data attribute</param>
		public GetICAOAnalysisResponse GetICAOAnalysis(Enumerations.SourceType sourceType, byte[] reference, string referenceFormat)
		{
			object contentType = CreateContentType(reference, referenceFormat);
			object image = CreateBiometricData(contentType, sourceType);

			dynamic response = CallMFaceService(Args(	new KeyValuePair<string, object>("Image", image)));

			GetICAOAnalysisResponse result = new GetICAOAnalysisResponse(response);

			if (response.CommonResponse.Status.ToString() == Enumerations.CommonResponseStatus.Success.ToString())
			{
				logger.Log(LogLevel.DEBUG, $"Result {GetCurrentMethodName()}, NonCompliantCriteria : {response.NonCompliantCriteria}, Compliant: {response.Compliant}");

			}
			return result;
		}

		/// <summary>
		/// This operation executes a 1:1 authentication process between the specified reference biometric data
		/// and biometric data from an ongoing or recently completed capture process.
		/// </summary>
		/// <param name="captureToken">The capture token to match data with.</param>
		/// <param name="personId">Unique ID of the person</param>
		/// <param name="sourceType">The source type of the reference</param>
		/// <param name="reference">The reference data</param>
		/// <param name="referenceFormat">The MIME type of the Data attribute.</param>
		/// <returns>Matching score</returns>
		public virtual VerifyResponse Verify(string captureToken, string personId, Enumerations.SourceType sourceType, byte[] reference, string referenceFormat)
		{
			object contentType = CreateContentType(reference, referenceFormat);
			object biometricData = CreateBiometricData(contentType, sourceType);

            // todo: Alejandro testeo
			var endpoint = channelFactory.Endpoint?.Address?.Uri?.ToString();
            logger.Log(LogLevel.DEBUG, $"Connecting to MFace.Service at: {endpoint}");

            if (!Uri.TryCreate(endpoint, UriKind.Absolute, out Uri p))
            {
                throw new Exception($"Invalid endpoint URL configured: {endpoint}");
            }
            // fin testeo

            dynamic response = CallMFaceService(Args(	new KeyValuePair<string, object>("CaptureToken", captureToken),
														new KeyValuePair<string, object>("PersonID", personId),
														new KeyValuePair<string, object>("Reference", biometricData)));

			if (response.CommonResponse.Status.ToString() == Enumerations.CommonResponseStatus.Success.ToString())
				logger.Log(LogLevel.DEBUG, $"Result {GetCurrentMethodName()}, MatchingScore: {response.MatchingScore}, ReferenceQuality: {response.ReferenceQuality}");

			return new VerifyResponse(response);
		}

		#region Biometric database management operations
		/// <summary>
		/// This operation cleans one of the available databases by deleting all inserted records.
		/// </summary>
		/// <param name="databaseIndex">The index of the database to insert the biometric data in. Range is 0 to 2.</param>
		public void DataBaseClean(short databaseIndex)
		{
			CallMFaceService(Args(new KeyValuePair<string, object>("DataBaseIndex", databaseIndex)));
		}

		/// <summary>
		/// This operation deletes a biometric data record from one of the available databases.
		/// </summary>
		/// <param name="databaseIndex">The index of the database to insert the biometric data in. Range is 0 to 2.</param>
		/// <param name="referenceID">Reference identification string (maximum size is 64 bytes).</param>
		public void DatabaseDelete(short databaseIndex, string referenceID)
		{
			CallMFaceService(Args(new KeyValuePair<string, object>("DataBaseIndex", databaseIndex),
									new KeyValuePair<string, object>("ReferenceID", referenceID)));
		}

		/// <summary>
		/// This operation retrieves information about one of the available databases.
		/// </summary>
		/// <param name="index">The index of the database to retrieve information from. Range is 0 to 2.</param>
		/// <returns>List of users</returns>
		public string[] DataBaseGetInfo(short databaseIndex)
		{
			dynamic response = CallMFaceService(Args(new KeyValuePair<string, object>("DataBaseIndex", databaseIndex)));

			if (response.CommonResponse.Status.ToString() == Enumerations.CommonResponseStatus.Success.ToString())
				logger.Log(LogLevel.DEBUG, $"Receive {GetCurrentMethodName()}, Number of users: {response.NumberOfRecords}");

			return response.Users;
		}

		/// <summary>
		/// This operation inserts a new biometric data record in one of the available databases
		/// </summary>
		/// <param name="databaseIndex">The index of the database to insert the biometric data in. Range is 0 to 2.</param>
		/// <param name="referenceID">Reference identification string (maximum size is 64 bytes)</param>
		/// <param name="data">The biometric type represented by the attached content. See section </param>
		/// <param name="referenceFormat">The biometric format</param>
		/// <returns>Quality value of the reference that was inserted.</returns>
		public short DataBaseInsert(short databaseIndex, string referenceID, byte[] data, string referenceFormat, SourceType sourceType)
		{
			object contentType = CreateContentType(data, referenceFormat);
			object biometricData = CreateBiometricData(contentType, sourceType);
			dynamic response = CallMFaceService(Args(new KeyValuePair<string, object>("DataBaseIndex", databaseIndex),
														new KeyValuePair<string, object>("Reference", biometricData),
														new KeyValuePair<string, object>("ReferenceID", referenceID)));

			logger.Log(LogLevel.INFO, $"Result {GetCurrentMethodName()}, ReferenceQuality: {response.ReferenceQuality}");

			return response.ReferenceQuality;
		}

		/// <summary>
		/// This operation allow to enable and disable database persistence.
		/// </summary>
		/// <param name="IsDatabasePersistenceEnable">Whether the database is persistent (true) or not (false).</param>
		/// <returns></returns>
		public void DatabasePersistence(bool isDatabasePersistenceEnable)
		{
			CallMFaceService(Args(new KeyValuePair<string, object>("IsDatabasePersistenceEnable", isDatabasePersistenceEnable)));
		}

		#endregion

		/// <summary>
		/// This operation enables the secure mode for communication. Secure communication remains enabled until a DisableSecureMode operation is received and persists across system restarts.
		/// </summary>
		/// <param name="confirmationTimeout">The timeout for receiving the ConfirmSecureMode before reverting to the previous communication mode.</param>
		/// <param name="certificate">The X.509 certificate along with its private key to be used for securing the communication. </param>
		/// <param name="certificatePassword">The password for X.509 certificate if any</param>
		/// <returns></returns>
		public string EnableSecureMode(TimeSpan confirmationTimeout, byte[] certificate, string certificateName, string certificatePassword)
		{
			object content = CreateContentType(certificate, certificateName);

			dynamic response = CallMFaceService(Args(new KeyValuePair<string, object>("Certificate", content),
														new KeyValuePair<string, object>("CertificatePassword", certificatePassword),
														new KeyValuePair<string, object>("ConfirmationTimeout", confirmationTimeout)));

			if (response.CommonResponse.Status.ToString() == CommonResponseStatus.Success.ToString())
			{
				logger.Log(LogLevel.DEBUG, $"Result {GetCurrentMethodName()}, ConfirmationToken: {response.ConfirmationToken}");
			}

			return response.ConfirmationToken;
		}

		/// <summary>
		/// This operation returns a complete list of the service configuration settings.
		/// </summary>
		/// <returns>Dictionary containing the configuration settings of the service.</returns>
		public IDictionary<string, string> GetConfiguration()
		{
			dynamic response = CallMFaceService();

			if (response.CommonResponse.Status.ToString() == CommonResponseStatus.Success.ToString())
				logger.Log(LogLevel.INFO, $"Receive {GetCurrentMethodName()}, Configuration are : {String.Join(",", response.Configuration)}");

			return response.Configuration;
		}

		/// <summary>
		/// This operation changes a specified subset of the service configuration settings. 
		/// Settings not included in the configuration dictionary are unmodified
		/// </summary>
		/// <param name="ConfigurationKey">Configuration Key </param>
		/// <param name="ConfigurationValue">Configuration Value</param>
		public void SetConfiguration(Dictionary<string, string> configurations)
		{
			CallMFaceService(Args(new KeyValuePair<string, object>("Configuration", configurations)));
		}

		/// <summary>
		/// This operation changes the operating mode of the service, which can shut down or restart the system hardware.
		/// Once the system has been shut down, restarting it will require power cycling or using Wake-On-LAN functionality.
		/// </summary>
		/// <param name="operatingMode">The operating mode to enter.</param>
		public void SetOperatingMode(OperatingMode operatingMode)
		{
			CallMFaceService(Args(new KeyValuePair<string, object>("OperatingMode", operatingMode)));
		}

		#endregion ISensorService

	}
}