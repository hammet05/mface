using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFace.Tool.MFaceProxy.Enumerations
{

		/// <summary>
		/// CommonResponse.Status member possible values.
		/// </summary>
		/// <remarks>
		/// The status values below are ranked in priority order: a status higher on the list will be
		/// reported if the corresponding situation is detected, even if situations lower on the list
		/// may also have occurred.
		/// </remarks>
		/// <seealso cref="Entities.CommonResponse.Status"/>
		
		public enum CommonResponseStatus
		{
			/// <summary>
			/// The specified token is invalid.
			/// </summary>		
			InvalidId,

			/// <summary>
			/// The service did not recognize a parameter.
			/// </summary>		
			NoSuchParameter,

			/// <summary>
			/// A parameter has an incompatible type or is outside its acceptable range.
			/// </summary>		
			BadValue,

			/// <summary>
			/// The service does not support the requested operation.
			/// </summary>			
			Unsupported,

			/// <summary>
			/// The operation was canceled and a sensor failure occurred.
			/// </summary>			
			CanceledWithSensorFailure,

			/// <summary>
			/// The operation was canceled.
			/// </summary>			
			Canceled,

			/// <summary>
			/// The biometric sensor is busy.
			/// </summary>			
			SensorBusy,

			/// <summary>
			/// The biometric sensor is in sleep mode.
			/// </summary>			
			SensorSleep,

			/// <summary>
			/// The operation failed due to a sensor failure.
			/// </summary>			
			SensorFailure,

			/// <summary>
			/// The biometric sensor encountered a timeout.
			/// </summary>			
			SensorTimeout,

			/// <summary>
			/// The sensor requires initialization.
			/// </summary>			
			InitializationNeeded,

			/// <summary>
			/// The operation failed due to a service error.
			/// </summary>			
			Failure,

			/// <summary>
			/// No data available that meets the request criteria.
			/// </summary>			
			NoDataAvailable,

			/// <summary>
			/// The operation completed successfully.
			/// </summary>			
			Success
		}

		/// <summary>
		/// The operating modes to enter.
		/// </summary>		
		public enum OperatingMode
		{
			/// <summary>
			/// Shut the MFace computer down.
			/// </summary>			
			Shutdown,

			/// <summary>
			/// Reboot the MFace computer.
			/// </summary>			
			Reboot,
		}

		/// <summary>
		/// Action type to be performed on media playback end when displaying a page.
		/// </summary>		
		public enum MediaEndAction
		{
			/// <summary>
			/// Stop the media playback when media end is reached.
			/// </summary>
			/// <remarks>When the media playback ends, its last image remain displayed.</remarks>		
			Stop,

			/// <summary>
			/// Hide the media when media end is reached.
			/// </summary>		
			Hide,

			/// <summary>
			/// Loop the media playback.
			/// </summary>
			/// <remarks>
			/// When the media playback ends, rewind it to the beginning and play it again.
			/// </remarks>			
			Loop
		}

		/// <summary>
		/// Live display mode indicating whether it should be disabled, enabled or enabled with overlay.
		/// </summary>		
		public enum LiveDisplayMode
		{
			/// <summary>
			/// Disable the live display.
			/// </summary>			
			Disable,

			/// <summary>
			/// Enable the live display (live image only).
			/// </summary>			
			Enable,

			/// <summary>
			/// Enable the live display with live overlay.
			/// </summary>		
			EnableWithOverlay,

			/// <summary>
			/// Enable the live display with live Ellipse overlay.
			/// </summary>			
			EnableWithEllipseOverlay
		}

		/// <summary>
		/// Log type corresponding to the different event log.
		/// </summary>	
		public enum LogType
		{
			/// <summary>
			/// Operational event log
			/// </summary>			
			Operational,

			/// <summary>
			/// Technical event log
			/// </summary>		
			Technical,

			/// <summary>
			/// Event log for log coming from libMorphoWay
			/// </summary>		
			LibMorphoWay,

			/// <summary>
			/// Event log for log coming from MorphoLite
			/// </summary>		
			MorphoLite
		}

		/// <summary>
		/// List of log severity
		/// </summary>
		
		public enum LogSeverity
		{			
			Debug,
			
			Info,
			
			Warning,
			
			Error
		}

		/// <summary>
		/// List of BeaconMode
		/// </summary>
		
		public enum LightMode
		{
			
			Off,
			
			Blink,
			
			Steady
		}

		/// <summary>
		/// List of Alarm severity
		/// </summary>
		
		public enum AlarmSeverity
		{
			/// <summary>
			/// Minor alarm
			/// </summary>
			Minor,
			/// <summary>
			/// Major alarm
			/// </summary>
			Major,
			/// <summary>
			/// Critical alarm
			/// </summary>
			Critical
		}

		public enum AlarmType
		{
			/// <summary>
			/// Unknown alarm type
			/// </summary>
			Unknown,
			/// <summary>
			/// Acquisition uniqueness alarm type
			/// </summary>
			Uniqueness,
			/// <summary>
			/// Acquisition has gathered sufficient data for enroll purpose
			/// </summary>
			ReadyForEnroll,
			//Acquisition has gathered sufficient data for matching purpose
			ReadyForMatch,
			/// <summary>
			/// Camera issue alarm type (e.g. camera disconnected)
			/// </summary>
			CameraIssue
		}

		/// <summary>
		/// List of biometric type.
		/// </summary>
		
		public enum BiometricType
		{
			/// <summary>
			/// Unknown biometric type.
			/// </summary>		
			Unknown,

			/// <summary>
			/// Face biometric type.
			/// </summary>		
			Face,

			////Fingerprint,
			////Iris,
		}

		/// <summary>
		/// List of algorithm file type.
		/// </summary>
		
		public enum AlgorithmFileType
		{
			/// <summary>
			/// Algorithm file type.
			/// </summary>		
			Algorithm,

			/// <summary>
			/// Normalization file type.
			/// </summary>		
			Normalization
		}

		/// <summary>
		/// Color selection for color correction coefficients.
		/// </summary>
		public enum ColorComponent
		{
			/// <summary>
			/// Apply correction only on red color.
			/// </summary>			
			Red,

			/// <summary>
			/// Apply correction only on green color.
			/// </summary>			
			Green,

			/// <summary>
			/// Apply correction only on blue color.
			/// </summary>		
			Blue,

			/// <summary>
			/// Apply correction on all 3 colors.
			/// </summary>			
			RGB
		}

		/// <summary>
		/// P.
		/// </summary>
		public enum PersonStatus
		{
			/// <summary>
			/// Detected.
			/// </summary>			
			Detected,

			/// <summary>
			/// ReadyForVerifDelayed.
			/// </summary>			
			ReadyForVerificationDelayed,

				/// <summary>
				/// R4V.
				/// </summary>			
			ReadyForVerification,

			/// <summary>
			/// Gone.
			/// </summary>			
			Gone
	}
	/// <summary>
	///  List of Status Service
	/// </summary>
	public enum ServiceStatus
	{
		/// <summary>
		/// Status: Unknown.
		/// </summary>
		UnkownStatus,

		/// <summary>
		/// Status: Started.
		/// </summary>
		Started,

		/// <summary>
		/// Status: Initialized.
		/// </summary>
		Initialized,

		/// <summary>
		/// Status: Capturing.
		/// </summary>
		Capturing
	}

		/// <summary>
		/// Capture mode.
		/// </summary>
		public enum CaptureMode
		{
			/// <summary>
			/// Single face capture mode.
			/// </summary>			
			SingleClosestFace,

			/// <summary>
			/// Multiface capture mode.
			/// </summary>			
			MultipleFace
		}

		/// <summary>
		/// NormalizationAlgorithm
		/// </summary>
		public enum SourceType
		{
			
			Unknown,
			
			Chip,
			
			Scan,
			
			Live
		}
		public enum SelfTestLevel
		{
		
		NOK,
		
		DANGER,
		
		WARNING,
	
		OK
	}
	public enum SelfTestId
	{
		
		UnkownStatus,
		
		DiskCStatus,

		DiskDStatus,
		
		RamStatus,
		
		CpuTemperatureStatus,

		CpuUsageStatus,
		
		BeaconStatus,
		
		CameraStatus,
		
		CameraBandwithStatus,
		
		License

	}


}

