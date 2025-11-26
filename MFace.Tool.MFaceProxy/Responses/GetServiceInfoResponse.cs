using MFace.Tool.MFaceProxy.Types;
using System.Xml;

namespace MFace.Tool.MFaceProxy.Responses
{
	public struct GetServiceInfoResponse
	{
		private ServiceInfo serviceInfo;

		private Types.DisplayInfo displayInfo;

		public DisplayInfo DisplayInfo
		{
			get
			{
				return displayInfo;
			}
		}

		public ServiceInfo ServiceInfo
		{
			get
			{
				return serviceInfo;
			}
		}

		public GetServiceInfoResponse(Morpho.MFace.Sensor.Service.Entities.ServiceInfo _serviceInfo, Morpho.MFace.Sensor.Service.Entities.DisplayInfo _displayInfo)
		{
			serviceInfo = new ServiceInfo(_serviceInfo.SoftwareVersion, _serviceInfo.Initialized, XmlConvert.ToTimeSpan(_serviceInfo.ServiceUptime.ToString()), _serviceInfo.AlarmSeverity);
			displayInfo = new DisplayInfo(_displayInfo.SoftwareVersion);
		}

		public GetServiceInfoResponse(MFaceServiceV3Kit.ServiceInfo _serviceInfo)
		{
			serviceInfo = new ServiceInfo(_serviceInfo.SoftwareVersion, XmlConvert.ToTimeSpan(_serviceInfo.ServiceUptime.ToString()), _serviceInfo.AlarmSeverity);
			displayInfo = new DisplayInfo();
		}

	}
}
