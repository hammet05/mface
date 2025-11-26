namespace MFace.Tool.MFaceProxy.Interfaces
{
	using System.Collections.Generic;
	using System.Drawing;
	using System;
	using System.IO;

	public interface ISensorServiceV2
	{
		void SetIndicatorLight(Color color1, Color color2, TimeSpan time1, TimeSpan time2, Enumerations.LightMode lightMode);
		Responses.GetSoftwareUpgradeResultResponse GetSoftwareUpgradeResult();
		string DeployAlgorithmFile(Stream algorithmFile, string fileName, Enumerations.AlgorithmFileType fileType);
	    void StartSoftwareUpgrade(string confirmationToken);
		string UploadSoftwareUpgrade(Stream upgradePackage);
		void StartSystemCalibration();
		void ActivateAutoStopCapture();
		void StartSleepMode();
		void StopSleepMode();
		Byte[] GetSleepModeBigDump(uint desiredFramesNumber);
	}
}
