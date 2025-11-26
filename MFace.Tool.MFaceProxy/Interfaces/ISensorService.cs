namespace MFace.Tool.MFaceProxy.Interfaces
{
	using Enumerations;
	using System;
	using System.Collections.Generic;
	using Types;

	public interface ISensorService
	{
		void Cancel();
		void ConfirmSecureMode(string confirmationToken);
		void DeployLicenseFile(Byte[] data, string contentType, string licenseFileName);
		string DisableSecureMode(TimeSpan confirmationTimeout);
		Byte[] GetBigDump(string captureToken, uint desiredFramesNumber);
		Responses.GetCaptureLiveDataResponse GetCaptureLiveData(bool getImage);
		Responses.GetICAOAnalysisResponse GetICAOAnalysis(Enumerations.SourceType sourceType, byte[] reference, string referenceFormat);
		Responses.GetLicensesInfoResponse GetLicenseInfo();
		bool GetPresence();
		List<Types.AlarmRecord> GetServiceAlarm();
		Responses.GetServiceInfoResponse GetServiceInfo();
		List<Types.LogRecord> GetServiceLog(Enumerations.LogType logType, DateTimeOffset startDate, DateTimeOffset endDate, string captureToken);
		Types.ServiceStatistics GetServicesStatistics();
		Enumerations.ServiceStatus GetServiceStatus();
		Responses.IdentifyResponse Identify(string captureToken, Guid personId, short databaseIndex);
		bool Initialize();
		void ResetFactoryConfiguration(string[] resetConfigurationKeys);
		List<Types.SelfTestStatus> SelfTest();
		void SetSystemTime(DateTimeOffset dateTime, string timeZoneName);
		bool StartCapture(int captureTimeout, Enumerations.CaptureMode captureMode, out string captureToken);
		void StopCapture(string captureToken);
		void Uninitialize();
		void UpdateConfigurationFile(byte[] data, string configurationFileName);
		Responses.VerifyResponse Verify(string captureToken, Guid personId, Enumerations.SourceType sourceType, byte[] reference, string referenceFormat);
		List<Types.PersonData> GetCaptureResult(string captureToken, int operationTimeout, int captureTimeout, bool autoStopCapture, bool getCroppedImage, bool getTemplate, bool getImage, out TimeSpan duration);

		// SUITE 

		void DataBaseClean(short databaseIndex);
		void DatabaseDelete(short databaseIndex, string referenceID);
		string[] DataBaseGetInfo(short index);
		short DataBaseInsert(short databaseIndex, string referenceID, byte[] data, string referenceFormat);
		void DatabasePersistence(bool isDatabasePersistenceEnable);
		string EnableSecureMode(TimeSpan confirmationTimeout, byte[] certificate, string certificateName, string certificatePassword);
		IDictionary<string, string> GetConfiguration();
		void SetConfiguration(Dictionary<string, string> configurations);
		void SetOperationMode(OperatingMode operatingMode);
	}
}
