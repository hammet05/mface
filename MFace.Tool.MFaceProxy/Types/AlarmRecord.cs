using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFace.Tool.MFaceProxy.Enumerations;

namespace MFace.Tool.MFaceProxy.Types
{
	public struct AlarmRecord
	{
		/// <summary>
		///  Gets a value indicating the Alarm Severity.
		/// </summary>
		public AlarmSeverity Severity { get; }

		/// <summary>
		/// Gets a value indicating the source of the alarm.
		/// </summary>
		public string Source { get; }

		/// <summary>
		/// Gets a value indicating the Alarm Type.
		/// </summary>
		public AlarmType Type { get; }

		/// <summary>
		/// Gets a value indicating the raise date of alarm.
		/// </summary>
		public DateTimeOffset Timestamp { get; }

		/// <summary>
		/// Gets a value indicating the alarm message
		/// </summary>
		public string Message { get; }


		internal AlarmRecord(Morpho.MFace.Sensor.Service.Entities.AlarmRecord alarmRecord)
		{
			this.Severity = (AlarmSeverity)alarmRecord.Severity;
			this.Source = alarmRecord.Source;
			this.Type = (AlarmType)alarmRecord.Type;
			this.Timestamp = DateTimeOffset.Parse(alarmRecord.Timestamp.ToString()); ;
			this.Message = alarmRecord.Message;
		}
		internal AlarmRecord(MFaceServiceV3Kit.AlarmRecord alarmRecord)
		{

			this.Severity = (AlarmSeverity)alarmRecord.Severity;
			this.Source = alarmRecord.Source;
			this.Type = (AlarmType)alarmRecord.Type;
			this.Timestamp = DateTimeOffset.Parse(alarmRecord.Timestamp.ToString()); ;
			this.Message = alarmRecord.Message;
		}
	}
}
