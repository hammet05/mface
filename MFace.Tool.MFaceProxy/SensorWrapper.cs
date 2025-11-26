namespace MFace.Tool.MFaceProxy
{
	using Common.Interfaces;   
    using Enumerations;
	using Responses;
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using System.IO;
	using System.Windows.Media.Media3D;
	using Types;

	public class SensorWrapper
	{
        

        #region Member

        private ushort mfaceVersion;
		private readonly ILogger mLogger;
		dynamic mSensorService;

		#endregion Member

		public bool IsInitialize
		{
			get { return mSensorService.IsInitialize; }
		}

		public DateTimeOffset ServiceLastestDateTime
		{
			get { return mSensorService.ServiceLastestDateTime; }
		}
	

		#region UTILITY
		/// <summary>
		/// GetFacesCropPosition
		/// </summary>
		/// <param name="captureLiveDatas">captureLiveDatas parameter</param>
		/// <param name="sourceImageWidth">sourceImageWidth parameter</param>
		/// <param name="sourceImageHeight">sourceImageHeight parameter</param>
		/// <returns></returns>
		public List<Point[]> GetFacesCropPosition(List<CaptureLiveData> captureLiveDatas, int sourceImageWidth, int sourceImageHeight)
		{
			List<System.Drawing.Point[]> facesOverlayPoints = new List<System.Drawing.Point[]>();

			foreach (CaptureLiveData data in captureLiveDatas)
			{
				if (data.HasFace3DPositionAndAngle && data.HasLiveImageFacePosition)
				{
					System.Drawing.Point[] result = GetFaceOverlayPoints(data.LiveImageFacePosition, data.Face3DPosition, sourceImageWidth, sourceImageHeight);
					facesOverlayPoints.Add(result);
				}
			}
			return facesOverlayPoints;
		}

		private System.Drawing.Point[] GetFaceOverlayPoints(System.Windows.Point face2DPosition, Point3D face3DPosition, int sourceImageWidth, int sourceImageHeight)
		{
			int zRef = 272;
			int xOffsetRef = -190;
			int yOffsetRef = -260;

			int faceXOffset = Convert.ToInt32((zRef / face3DPosition.Z) * xOffsetRef);
			int faceYOffset = Convert.ToInt32((zRef / face3DPosition.Z) * yOffsetRef);
			int width = Math.Abs(faceXOffset * 2);
			int height = Math.Abs(faceYOffset * 2);

			int faceX = Math.Abs((int)face2DPosition.X);
			int faceY = Math.Abs((int)face2DPosition.Y);

			if (faceX + faceXOffset + width > sourceImageWidth)
				width = sourceImageWidth - (faceX + faceXOffset);

			if (faceY + faceYOffset + height > sourceImageHeight)
				height = sourceImageHeight - (faceY + faceYOffset);

			int x = Math.Abs(faceX + faceXOffset);
			int y = Math.Abs(faceY + faceYOffset);
			System.Drawing.Point[] result = new System.Drawing.Point[4];
			result[0] = new System.Drawing.Point(x, y);
			result[1] = new System.Drawing.Point(x + width, y);
			result[2] = new System.Drawing.Point(x + width, y + height);
			result[3] = new System.Drawing.Point(x, y + height);

			return result;
		}


		#endregion

		#region Constructor

		public SensorWrapper(ILogger logger, ushort aVersion, string serviceEndPointName)
		{
			mLogger = logger;
			mfaceVersion = aVersion;

			switch (mfaceVersion)
			{
				case 3:
					mSensorService = new SensorServiceV3(mLogger);
					mSensorService.CreateChannel(serviceEndPointName);
					break;
				case 2:
				default:
					mSensorService = new SensorServiceV2(mLogger);
					mSensorService.CreateChannel(serviceEndPointName);
					break;
			}
		}

		#endregion Constructor

		#region Common
		/// <summary>
		/// This operation initializes the sensor components of the service. 
		/// </summary>
		public bool Initialize()
		{
			return mSensorService.Initialize();
		}

		public string StartCapture(int captureTimeout, CaptureMode captureMode)
		{
			try
			{
                return mSensorService.StartCapture(captureTimeout, captureMode);
            }
			catch (Exception ex)
			{

				throw ex;
			}
			
		}

		/// <summary>
		/// This operation starts a biometric capture process.
		/// </summary>
		/// <param name="captureTimeout">The Capture timeout</param>
		/// <param name="captureMode">The Capture mode</param>
		/// <param name="data"></param>
		/// <param name="referenceformat"></param>
		/// <param name="sourceType"></param>
		/// <returns></returns>
		public string StartCapture(int captureTimeout, CaptureMode captureMode, byte[] data, string referenceformat, SourceType sourceType)
		{
			return mSensorService.StartCapture(captureTimeout, captureMode, data, referenceformat, sourceType).ToString();
		}

		/// <summary>
		/// This operation retrieves biometric data of possibly different people from the ongoing or the latest 
		/// completed capture process.
		/// </summary>
		/// <param name="captureToken">The capture token to retreive result from.</param>
		/// <param name="captureTimeout">The amount of time the capture is allowed to run if sufficient data is not yet available (SingleFace only). In MFace V3, this parameter is equivalent to "Timeout".</param>
		/// <param name="autoStopCapture">Whether the capture workflows should be automatically stopped when sufficient biometric data had been acquired.</param>
		/// <param name="getCroppedImage">Whether to retrieve cropped image arround the face that has been detected</param>
		/// <param name="getTemplate">Whether to retrieve the biometric template from the capture.</param>
		/// <param name="getImage">Whether to retrieve the image from the capture.</param>
		/// <param name="duration">Request duration</param>
		/// <param name="operationTimeout">MFace V2 MultiFace only - The amount of time this operation is going to wait for a new biometric data </param>
		/// <returns>List of PersonData</returns>
		public List<PersonData> GetCaptureResult(string captureToken, int captureTimeout, bool autoStopCapture, bool getCroppedImage, bool getTemplate, bool getImage, out TimeSpan duration)
		{
            mLogger.Log(LogLevel.DEBUG, "GetCaptureResult 2");
            duration = new TimeSpan();

			if (mfaceVersion == 2)
			{
                mLogger.Log(LogLevel.DEBUG, "GetCaptureResult 3");
                // current work around to be reworked (operationtimeout)
                int operationTimeout = 1000;
                mLogger.Log(LogLevel.DEBUG, "token: "+ captureToken
					+ " timeout: "+ captureTimeout
					+ " autoStopCapture "+ autoStopCapture
                    + " getCroppedImage " + getCroppedImage
                    + " getTemplate " + getTemplate
                    + " getImage " + getImage
                    + " duration " + duration.ToString()
                    );
                mLogger.Log(LogLevel.DEBUG, "GetCaptureResult 3.1");
                return mSensorService.GetCaptureResult(captureToken, operationTimeout, captureTimeout, autoStopCapture, getCroppedImage, getTemplate, getImage);
			}
			else
			{
                mLogger.Log(LogLevel.DEBUG, "GetCaptureResult 4");
                int operationTimeout = 1000;
				return mSensorService.GetCaptureResult(captureToken, operationTimeout, captureTimeout, autoStopCapture, getCroppedImage, getTemplate, getImage);
			}
            
           
		}

		/// <summary>
		/// When this operation is called, all currently detected persons status are set to Gone,
		/// and all the ones that were not retrieved beforehand are ready to be retrieved by a final GetCaptureResult request.
		/// </summary>
		/// <param name="captureToken">Capture process token used to stop currently running capture.</param>
		public void StopCapture(string captureToken)
		{
			mSensorService.StopCapture(captureToken);
		}
		/// <summary>
		/// This operation is used to cancel an ongoing operation.This includes notably the 
		/// StartCapture operation (and any concurrent biometric operation).
		/// </summary>
		public void Cancel()
		{
			mSensorService.Cancel();
		}

		/// <summary>
		/// This operation allows to ask if a presence has been detected in the section of
		/// the image defined in the configuration file. The processed images come from the camera
		/// specified in the configuration file as well. This operation works only if StartSleepMode() 
		/// has been called and StopSleepMode() has not been called since.
		/// </summary>
		/// <returns>True is a person is detected</returns>
		public bool GetPresence()
		{
			return mSensorService.GetPresence();
		}

		/// <summary>
		/// This operation provides information about the currently active service alarms
		/// </summary>
		/// <returns>List of AlarmRecord currently active</returns>
		public List<Types.AlarmRecord> GetServiceAlarm()
		{
			return mSensorService.GetServiceAlarm();
		}

		/// <summary>
		/// This operation gives information about Licences installed. 
		/// </summary>
		/// <returns></returns>
		public GetLicensesInfoResponse GetLicenseInfo()
		{
			return mSensorService.GetLicensesInfo();
		}

		/// <summary>
		/// This operation returns Status of Service.
		/// </summary>
		/// <returns>Current service status</returns>
		public ServiceStatus GetServiceStatus()
		{
			return mSensorService.GetServiceStatus();
		}

		/// <summary>
		/// This operation retrieves statistics about the usage of the service.
		/// </summary>
		/// <returns></returns>
		public ServiceStatistics GetServicesStatistics()
		{
			return mSensorService.GetServicesStatistics();
		}

		/// <summary>
		/// This operation provides general information about the sensor and service.
		/// </summary>
		/// <returns></returns>
		public GetServiceInfoResponse GetServiceInfo()
		{
			return mSensorService.GetServiceInfo();
		}
		
		/// <summary>
		/// This opeartion Perform a hardware diagnostic and returns the collected data
		/// </summary>
		/// <returns></returns>
		public List<SelfTestStatus> SelfTest()
		{
			return mSensorService.SelfTest();
		}
		
		/// <summary>
		/// This operation uninitializes the sensor components of the service.
		/// </summary>
		public void Uninitialize()
		{
			mSensorService.Uninitialize();
		}

		/// <summary>
		/// This operation is used to confirm that the secure communication is correctly enable or disable and communication is still possible between the service and its clients.
		/// </summary>
		/// <param name="confirmationToken">Token returned by EnableSecureMode or DisableSecureMode operations used to confirm the communication mode change.</param>
		public void ConfirmSecureMode(string confirmationToken)
		{
			mSensorService.ConfirmSecureMode(confirmationToken);
		}

		/// <summary>
		/// This operation allows to download BigDump file generated during a given acquisition. 
		/// A BigDump is a set of binary data structured in a proprietary format allowing IDEMIA to obtain detailed data about a capture process.
		/// Those BigDump contains personal data (e.g. a face video stream) and must be carried with care for privacy purpose (e.g. data encryption).
		/// </summary>
		/// <param name="captureToken">The capture token to retrieve BigDump from.</param>
		/// <param name="desiredFramesNumber">The number of frames one would like to retrieve.
		/// This value is 0 by default, in which case all the available frames are sent back. If the number exceed the number of frames available, all the frames are sent back as well.</param>
		/// <returns></returns>
		public Byte[] GetBigDump(string captureToken, uint desiredFramesNumber)
		{
			return mSensorService.GetBigDump(captureToken, desiredFramesNumber);
		}

		/// <summary>
		/// This operation is used to upload a license file onto the system, which is then deployed by Morpho Protect.
		/// </summary>
		/// <param name="data">The content attached data.</param>
		/// <param name="contentType">The MIME type of the Data attribute. </param>
		/// <param name="licenseFileName">The license file name.</param>
		public void DeployLicenseFile(Byte[] data, string contentType, string licenseFileName)
		{
			mSensorService.DeployLicenseFile(data, contentType, licenseFileName);
		}

		/// <summary>
		/// This operation disables the secure mode for the communication and optionally deletes the certificate stored on the system.
		/// </summary>
		/// <param name="confirmationTimeout">The timeout for receiving the ConfirmSecureMode before reverting to the previous communication mode.</param>
		/// <returns></returns>
		public string DisableSecureMode(TimeSpan confirmationTimeout)
		{
			return mSensorService.DisableSecureMode(confirmationTimeout);
		}

		/// <summary>
		/// This operation retrieves live information from an ongoing face biometric capture process.
		/// It can also be used to periodically retrieve the capture live image for display a live capture stream outside the MFACE.
		/// </summary>
		/// <param name="getImage">Whether to retrieve the live image from the camera along with the capture live information.</param>
		/// <returns></returns>
		public GetCaptureLiveDataResponse GetCaptureLiveData(bool getImage)
		{
			return mSensorService.GetCaptureLiveData(getImage);
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
			return mSensorService.Identify(captureToken, personId, databaseIndex);
		}
		
		/// <summary>
		/// This operation resets the MFACE system to its factory configuration settings.
		/// </summary>
		/// <param name="resetConfigurationKeys">Configuration settings fully qualified name to reset. Full list can be retrieve by using GetCongifuration request</param>
		public void ResetFactoryConfiguration(string[] resetConfigurationKeys)
		{
			mSensorService.ResetFactoryConfiguration(resetConfigurationKeys);
		}
		
		/// <summary>
		/// This operation changes the MFACE system date, time and time zone.
		/// </summary>
		/// <param name="dateTime">The new system data and time to set.</param>
		/// <param name="timeZoneName">The new time zone to set.</param>
		public void SetSystemTime(DateTimeOffset dateTime, string timeZoneName)
		{
			mSensorService.SetSystemTime(dateTime, timeZoneName);
		}
		
		/// <summary>
		/// This operation is used to upload a configuration file onto the system. The file must exist in the MFace installation. The file will be replaced.
		/// </summary>
		/// <param name="data">The content attached data.</param>
		/// <param name="configurationFileName">The configuration file name.</param>
		/// <param name="contentType">The MIME type of the Data attribute.</param>
		public void UpdateConfigurationFile(byte[] data, string configurationFileName)
		{
			mSensorService.UpdateConfigurationFile(data, configurationFileName);
		}
		
		/// <summary>
		/// This operation takes a face image and perform an analysis to evaluate a selection of ICAO criteria.
		/// Then, it returns the result (“Is the image compliant ?”), listing the criteria the image is not compliant with.
		/// </summary>
		/// <param name="sourceType">The source type of the reference</param>
		/// <param name="reference">The reference data</param>
		/// <param name="referenceFormat">The MIME type of the Data attribute</param>
		public Responses.GetICAOAnalysisResponse GetICAOAnalysis(SourceType sourceType, byte[] reference, string referenceFormat)
		{
			return mSensorService.GetICAOAnalysis(sourceType, reference, referenceFormat);
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
		public Responses.VerifyResponse Verify(string captureToken, string personId, SourceType sourceType, byte[] reference, string referenceFormat)
		{
			return mSensorService.Verify(captureToken, personId, sourceType, reference, referenceFormat);
		}
		
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
			return mSensorService.GetServiceLog(logType, startDate, endDate, captureToken);
		}
		
		/// <summary>
		/// This operation cleans one of the available databases by deleting all inserted records.
		/// </summary>
		/// <param name="databaseIndex">The index of the database to insert the biometric data in. Range is 0 to 2.</param>
		public void DataBaseClean(short databaseIndex)
		{
			mSensorService.DataBaseClean(databaseIndex);
		}
		
		/// <summary>
		/// This operation deletes a biometric data record from one of the available databases.
		/// </summary>
		/// <param name="databaseIndex">The index of the database to insert the biometric data in. Range is 0 to 2.</param>
		/// <param name="referenceID">Reference identification string (maximum size is 64 bytes).</param>
		public void DatabaseDelete(short databaseIndex, string referenceID)
		{
			mSensorService.DatabaseDelete(databaseIndex, referenceID);
		}
		
		/// <summary>
		/// This operation retrieves information about one of the available databases.
		/// </summary>
		/// <param name="index">The index of the database to retrieve information from. Range is 0 to 2.</param>
		/// <returns>List of users</returns>
		public string[] DataBaseGetInfo(short index)
		{
			return mSensorService.DataBaseGetInfo(index);
		}
		
		/// <summary>
		/// This operation inserts a new biometric data record in one of the available databases
		/// </summary>
		/// <param name="databaseIndex">The index of the database to insert the biometric data in. Range is 0 to 2.</param>
		/// <param name="referenceID">Reference identification string (maximum size is 64 bytes)</param>
		/// <param name="data">The biometric type represented by the attached content. See section </param>
		/// <param name="referenceFormat">The biometric format</param>
		/// <returns>Quality value of the reference that was inserted.</returns>
		public short DataBaseInsert(short databaseIndex, string referenceID, byte[] data, string referenceFormat)
		{
			return mSensorService.DataBaseInsert(databaseIndex, referenceID, data, referenceFormat, SourceType.Unknown);
		}
		
		/// <summary>
		/// This operation allow to enable and disable database persistence.
		/// </summary>
		/// <param name="IsDatabasePersistenceEnable">Whether the database is persistent (true) or not (false).</param>
		/// <returns></returns>
		public void DatabasePersistence(bool isDatabasePersistenceEnable)
		{
			mSensorService.DatabasePersistence(isDatabasePersistenceEnable);
		}
		
		/// <summary>
		/// This operation enables the secure mode for communication. Secure communication remains enabled until a DisableSecureMode operation is received and persists across system restarts.
		/// </summary>
		/// <param name="confirmationTimeout">The timeout for receiving the ConfirmSecureMode before reverting to the previous communication mode.</param>
		/// <param name="certificate">The X.509 certificate along with its private key to be used for securing the communication. </param>
		/// <param name="certificatePassword">The password for X.509 certificate if any</param>
		/// <returns></returns>
		public string EnableSecureMode(TimeSpan confirmationTimeout, byte[] certificate, string certificateName, string certificatePassword)
		{
			return mSensorService.EnableSecureMode(confirmationTimeout, certificate, certificateName, certificatePassword);
		}
		
		/// <summary>
		/// This operation returns a complete list of the service configuration settings.
		/// </summary>
		/// <returns>Dictionary containing the configuration settings of the service.</returns>
		public IDictionary<string, string> GetConfiguration()
		{
			return mSensorService.GetConfiguration();
		}
		
		/// <summary>
		/// This operation changes a specified subset of the service configuration settings. 
		/// Settings not included in the configuration dictionary are unmodified
		/// </summary>
		/// <param name="ConfigurationKey">Configuration Key </param>
		/// <param name="ConfigurationValue">Configuration Value</param>
		public void SetConfiguration(Dictionary<string, string> configurations)
		{
			mSensorService.SetConfiguration(configurations);
		}
		
		/// <summary>
		/// This operation changes the operating mode of the service, which can shut down or restart the system hardware.
		/// Once the system has been shut down, restarting it will require power cycling or using Wake-On-LAN functionality.
		/// </summary>
		/// <param name="operatingMode">The operating mode to enter.</param>
		public void SetOperatingMode(OperatingMode operatingMode)
		{
			mSensorService.SetOperatingMode(operatingMode);
		}

		public void SetReference(string captureToken, byte[] reference, SourceType sourceType, string referenceFormat)
		{
			mSensorService.SetReference(captureToken, sourceType, reference, referenceFormat);
		}

		#endregion Common

		#region V2
		/// <summary>
		/// This operation changes the state of the totem’s indicator light.
		/// </summary>
		/// <param name="lightMode">The indicator light mode.</param>
		/// <param name="color1">The color value to be set on the indicator light as a hexadecimal triplet </param>
		/// <param name="color2">The alternative color value to be set on the indicator light when LightModeattribute is set to “Blink” as a hexadecimal triplet.</param>
		/// <param name="time1">The duration while the Color1 will be set.</param>
		/// <param name="time2">The duration while the Color2 will be set.</param>
		public void SetIndicatorLight(Color color1, Color color2, TimeSpan time1, TimeSpan time2, Enumerations.LightMode lightMode)
		{
			if (mfaceVersion == 3)
				throw new NotImplementedException($"Function SetIndicatorLight is not available in MFace version {mfaceVersion}");

			mSensorService.SetIndicatorLight(color1, color2, time1, time2, lightMode);
		}
		
		/// <summary>
		/// This operation is used to start the upgrade procedure using a previously uploaded upgrade package.
		/// </summary>
		/// <returns></returns>
		public GetSoftwareUpgradeResultResponse GetSoftwareUpgradeResult()
		{
			if (mfaceVersion == 3)
				throw new NotImplementedException($"Function {Utility.GetCurrentMethodName()} is not available in MFace version {mfaceVersion}");

			return mSensorService.GetSoftwareUpgradeResult();
		}
		
		/// <summary>
		/// This operation is used to upload an algorithm binary file, or a normalization binary file onto the system. Depending of the file type, it will be deployed in its corresponding directory.
		/// </summary>
		/// <param name="algorithmFile">Stream data containing the software upgrade package.</param>
		/// <param name="fileName">The name of the file.</param>
		/// <param name="fileType">The type of the file.</param>
		/// <returns>confirmation Token</returns>
		public string DeployAlgorithmFile(Stream algorithmFile, string fileName, Enumerations.AlgorithmFileType fileType)
		{
			if (mfaceVersion == 3)
				throw new NotImplementedException($"Function {Utility.GetCurrentMethodName()} is not available in MFace version {mfaceVersion}");

			return mSensorService.DeployAlgorithmFile(algorithmFile, fileName, fileType);
		}
		
		/// <summary>
		/// This operation is used to start the upgrade procedure using a previously uploaded upgrade package.
		/// </summary>
		/// <param name="confirmationToken">Token returned by UploadSoftwareUpgrade used to start the software upgrade procedure.</param>
		public void StartSoftwareUpgrade(string confirmationToken)
		{
			if (mfaceVersion == 3)
				throw new NotImplementedException($"Function {Utility.GetCurrentMethodName()} is not available in MFace version {mfaceVersion}");

			mSensorService.StartSoftwareUpgrade(confirmationToken);
		}
		
		/// <summary>
		/// This operation is used to upload a binary software upgrade package onto the system. Only one such package is stored by the system at a time; uploading a new package will always overwrite any previously-stored package.
		/// </summary>
		/// <param name="upgradePackage">Stream data containing the software upgrade package.</param>
		/// <returns>Token used to start or confirm the software upgrade procedure.</returns>
		public string UploadSoftwareUpgrade(Stream upgradePackage)
		{
			if (mfaceVersion == 3)
				throw new NotImplementedException($"Function {Utility.GetCurrentMethodName()} is not available in MFace version {mfaceVersion}");

			return mSensorService.UploadSoftwareUpgrade(upgradePackage);
		}
		
		/// <summary>
		/// This operation enables calibration from the Servcie 
		/// </summary>
		public void StartSystemCalibration()
		{
			if (mfaceVersion == 3)
				throw new NotImplementedException($"Function {Utility.GetCurrentMethodName()} is not available in MFace version {mfaceVersion}");

			mSensorService.StartSystemCalibration();
		}
		
		/// <summary>
		/// In a MonoFace capture only, this function stop the Capture when a person is ReadyForVerification.
		/// </summary>
		public void ActivateAutoStopCapture()
		{
			if (mfaceVersion == 3)
				throw new NotImplementedException($"Function {Utility.GetCurrentMethodName()} is not available in MFace version {mfaceVersion}");

			mSensorService.ActivateAutoStopCapture();
		}
		
		/// <summary>
		/// This operation allows to start the sleep mode, during which any presence within
		/// the section defined in the configuration file will be detected.
		/// </summary>
		public void StartSleepMode()
		{
			if (mfaceVersion == 3)
				throw new NotImplementedException($"Function {Utility.GetCurrentMethodName()} is not available in MFace version {mfaceVersion}");

			mSensorService.StartSleepMode();
		}
		
		/// <summary>
		/// This operation allows to stop the sleep mode and the presence detection.
		/// </summary>
		public void StopSleepMode()
		{
			if (mfaceVersion == 3)
				throw new NotImplementedException($"Function {Utility.GetCurrentMethodName()} is not available in MFace version {mfaceVersion}");

			mSensorService.StopSleepMode();
		}
		
		/// <summary>
		/// This operation allow to download BigDump file generated during last sleep mode period.
		/// A BigDump is a set of binary data structured in a proprietary format allowing IDEMIA to obtain detailed data about a capture process.
		/// Those BigDump contains personal data (e.g. a face video stream) and must be carried with care for privacy purpose (e.g. data encryption).
		/// </summary>
		/// <param name="desiredFramesNumber">The number of frames one would like to retrieve. This value is 0 by default, in which case all the available frames are sent back.
		/// If the number exceed the number of frames available, all the frames are sent back as well.</param>
		/// <returns></returns>
		public Byte[] GetSleepModeBigDump(uint desiredFramesNumber)
		{
			if (mfaceVersion == 3)
				throw new NotImplementedException($"Function {Utility.GetCurrentMethodName()} is not available in MFace version {mfaceVersion}");

			return mSensorService.GetSleepModeBigDump(desiredFramesNumber);
		}

		#endregion V2

		#region V3
		
		// ToDo implémenter les méthodes quand les signatures seront connues dans le WSDL
		//UploadFile
		//StartPresenceDetection
		//StopPresenceDetection
		//GetPresenceDetectionBigDump

		#endregion V3

		#region Properties

		public dynamic GetSensorService
		{
			get
			{
				return mSensorService;
			}
		}
		private ushort VersionKit
		{
			get
			{
				return mfaceVersion;
			}
		}

		#endregion Properties

	}
}
