using MFace.Tool.MFaceProxy.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFace.Tool.MFaceProxy.Responses
{
	public class GetSoftwareUpgradeResultResponse
	{
		private readonly DateTimeOffset _timestamp;
		private readonly string _oldVersion;
		private readonly string _newVersion;
		private readonly string _status;
		private readonly string _details;

		
		private DisplayInfo displayInfo;
		public DisplayInfo DisplayInfo
		{
			get
			{
				return displayInfo;
			}
		}

		public  DateTimeOffset Timestamp
		{
			get
			{
				return _timestamp;
			}
		}

		public string OldVersion
		{
			get
			{
				return _oldVersion;
			}
		}

		public string Status
		{
			get
			{
				return _status;
			}
		}

		public string Details
		{
			get
			{
				return _details;
			}
		}

		public string NewVersion
		{
			get
			{
				return _newVersion;
			}
		}

		public GetSoftwareUpgradeResultResponse(Morpho.MFace.Sensor.Service.Messages.GetSoftwareUpgradeResultResponse response)
		{
			displayInfo = new DisplayInfo(response.DisplayInfo.SoftwareVersion);
			_oldVersion = response.OldVersion;
			_newVersion = response.NewVersion;
			_status = response.Status;
			_details = response.Details;
			_timestamp = response.Timestamp;
		}
	}
}
