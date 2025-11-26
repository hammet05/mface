using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Xml;
using MFace.Tool.MFaceProxy.Enumerations;

namespace MFace.Tool.MFaceProxy.Types
{
	public class PersonData
	{
		public string PersonID { get; }
		public PersonStatus PersonStatus { get; }
		public BiometricData Image { get; }
		public BiometricData CroppedImage { get; }
		public Point LeftEyePosition { get; }
		public Point RightEyePosition { get; }
		public Point3D Position3D { get; }
		public BiometricData Template { get; }
		public float AcquisitionQuality { get; }
		public bool? Spoof { get; }
		public float LivenessScore { get; }
		public TimeSpan? CaptureTime { get; }

		private PersonData(string personID, PersonStatus personStatus, float acquisitionQuality)
		{
			this.PersonID = personID;
			this.PersonStatus = personStatus;
			this.AcquisitionQuality = acquisitionQuality;
		}
		internal PersonData(Morpho.MFace.Sensor.Service.Entities.PersonData personData) : this(personData.PersonID.ToString(), (PersonStatus)personData.PersonStatus, personData.AcquisitionQuality)
		{
			this.Image = new BiometricData(personData.Image);
			this.CroppedImage = new BiometricData(personData.CroppedImage);
			this.Template = new BiometricData(personData.Template);
			this.LeftEyePosition = new Point(personData.LeftEyePosition.X, personData.LeftEyePosition.Y);
			this.RightEyePosition = new Point (personData.RightEyePosition.X, personData.RightEyePosition.Y);
			this.Position3D = new Point3D(personData.Position3D.X, personData.Position3D.Y, personData.Position3D.Z);
			this.CaptureTime = personData.CaptureTime;
			this.Spoof = personData.Spoof;
			this.LivenessScore = personData.SpoofingScore == null ? 0 : (float)personData.SpoofingScore;
		}

		internal PersonData(MFaceServiceV3Kit.PersonData personData) : this(personData.PersonID, (PersonStatus)personData.PersonStatus, personData.AcquisitionQuality)
		{
			if (personData.Image != null)
				this.Image = new BiometricData(personData.Image);
			else
				this.Image = new BiometricData();

			if (personData.CroppedImage != null)
				this.CroppedImage = new BiometricData(personData.CroppedImage);
			else
				this.CroppedImage = new BiometricData();

			if (personData.Template != null)
				this.Template = new BiometricData(personData.Template);
			else
				this.Template = new BiometricData();

			if (personData.Position3D != null)
			{
				this.Position3D = new Point3D(personData.Position3D._x, personData.Position3D._y, personData.Position3D._z);
			}
			else
				this.Position3D = new Point3D();


			this.LeftEyePosition = new Point(personData.LeftEyePosition._x, personData.LeftEyePosition._x);
			this.RightEyePosition = new Point(personData.RightEyePosition._x, personData.RightEyePosition._y);

			if(personData.Spoof != null)
            {
				this.Spoof = personData.Spoof.SpoofingStatus;
				this.LivenessScore = personData.Spoof.SpoofingScore == null ? 0 : (float)personData.Spoof.SpoofingScore;
            }
			else
            {
				this.Spoof = null;
				this.LivenessScore = 0;
			}

			if(personData.CaptureTime != "")
				this.CaptureTime = XmlConvert.ToTimeSpan(personData.CaptureTime);
				
			
		}
	}
}
