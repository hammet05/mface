using System.Windows;
using System.Windows.Media.Media3D;

namespace MFace.Tool.MFaceProxy.Types
{
	public struct CaptureLiveData 
	{
		public Vector3D Face3DAngle { get; }
		public Point3D Face3DPosition { get; }
		public bool HasFace3DPositionAndAngle { get; }
		public bool HasLiveImageFacePosition { get; }
		public Point LiveImageFacePosition { get; }

		internal CaptureLiveData(Morpho.MFace.Sensor.Service.Entities.CaptureLiveData captureData)
		{
			this.Face3DAngle = new Vector3D(captureData.Face3DAngle.X,captureData.Face3DAngle.Y, captureData.Face3DAngle.Z);
			this.Face3DPosition = new  Point3D(captureData.Face3DPosition.X, captureData.Face3DPosition.Y, captureData.Face3DPosition.Z);
			this.HasFace3DPositionAndAngle = captureData.HasFace3DPositionAndAngle;
			this.HasLiveImageFacePosition = captureData.HasLiveImageFacePosition;
			this.LiveImageFacePosition = new Point (captureData.LiveImageFacePosition.X, captureData.LiveImageFacePosition.Y);
		}
		internal CaptureLiveData(MFaceServiceV3Kit.CaptureLiveData captureData)
		{
			if (captureData.Face3DAngle != null)
				this.Face3DAngle = new Vector3D(captureData.Face3DAngle._x, captureData.Face3DAngle._y, captureData.Face3DAngle._z);
			else
				this.Face3DAngle = new Vector3D();

			if (captureData.Face3DPosition != null)
				this.Face3DPosition = new Point3D(captureData.Face3DPosition._x, captureData.Face3DPosition._y, captureData.Face3DPosition._z);
			else
				this.Face3DPosition = new Point3D();

			if (captureData.LiveImageFacePosition != null)
				this.LiveImageFacePosition = new Point(captureData.LiveImageFacePosition._x, captureData.LiveImageFacePosition._y);
			else
				this.LiveImageFacePosition = new Point();

			this.HasFace3DPositionAndAngle = captureData.HasFace3DPositionAndAngle;
			this.HasLiveImageFacePosition = captureData.HasLiveImageFacePosition;
		}
	}
}
