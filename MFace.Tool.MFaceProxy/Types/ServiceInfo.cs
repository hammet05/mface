using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFace.Tool.MFaceProxy.Types
{
	public class ServiceInfo
	{
		public string SoftwareVersion {get;}
		public bool Initialized { get;}
		public TimeSpan ServiceUptime { get;}
		public string AlarmSeverity { get; }

		public ServiceInfo(string softwareVersion, bool initialized, TimeSpan serviceUptime, string alarmSeverity)
		{
			this.SoftwareVersion = softwareVersion;
			this.Initialized = initialized;
			this.ServiceUptime = serviceUptime;
			this.AlarmSeverity = alarmSeverity;
		}
		public ServiceInfo(string softwareVersion, TimeSpan serviceUptime, string alarmSeverity)
		{
			this.SoftwareVersion = softwareVersion;
			this.ServiceUptime = serviceUptime;
			this.AlarmSeverity = alarmSeverity;
		}
	}
}
