namespace MFace.Tool.MFaceProxy.Interfaces
{
	public interface ISensorServiceV3
	{
		// nom des méthodes V3
		// https://confluence-mogl.build.sec.safran/pages/viewpage.action?spaceKey=MFACEFW&title=ICD+evolution

		void UploadFile();//ToDo /!\ signature temporaire (pas encore dans le WSDL) /!\
		void StartPresenceDetection();//ToDo /!\ signature temporaire (pas encore dans le WSDL) /!\
		void StopPresenceDetection();//ToDo /!\ signature temporaire (pas encore dans le WSDL) /!\
		void GetPresenceDetectionBigDump();//ToDo /!\ signature temporaire (pas encore dans le WSDL) /!\
	}
}
