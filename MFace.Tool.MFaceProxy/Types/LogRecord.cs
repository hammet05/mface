using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MFace.Tool.MFaceProxy.Enumerations;


namespace MFace.Tool.MFaceProxy.Types
{
	public struct LogRecord
	{
	    public LogSeverity Severity { get; }
	
		public string Source { get; }

		public LogType LogType { get; }

		public DateTimeOffset Timestamp { get; }

		public string Message { get; }

		public LogRecord(Morpho.MFace.Sensor.Service.Enumerations.LogSeverity severity, Morpho.MFace.Sensor.Service.Enumerations.LogType logType, string source, DateTimeOffset tmestamps, string message)
		{
			this.Severity = (LogSeverity)severity;
			this.LogType = (LogType)logType;
			this.Source = source;
			this.Timestamp = tmestamps;
			this.Message = message;
		}

		public LogRecord(string severity, MFaceServiceV3Kit.LogType logType, string source, string tmestamps, string message)
		{
			if (severity == "")
				this.Severity = LogSeverity.Info;
			else
				this.Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), $"{severity[0] + severity.Substring(1).ToLower()}");
			this.LogType = (LogType)logType;
			this.Source = source;
			this.Timestamp = DateTimeOffset.Now;
			this.Message = message;
		}


	}
}
