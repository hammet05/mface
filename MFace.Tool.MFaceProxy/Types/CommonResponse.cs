using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFace.Tool.MFaceProxy.Enumerations;
using System.Xml;

namespace MFace.Tool.MFaceProxy.Types
{
	//struct
	public struct CommonResponse
	{
		/// <summary>
		/// Current system time of the server in ISO8601 format.
		/// </summary>
		public DateTimeOffset Timestamp { get; }

		/// <summary>
		/// The status of the operation.
		/// </summary>
		public CommonResponseStatus Status { get; }

		/// <summary>
		/// Informative message describing the status.
		/// </summary>
		public string Message { get; }

		/// <summary>
		/// Operation processing time in milliseconds.
		/// </summary>
		public TimeSpan Duration { get; }

		//internal
		internal CommonResponse(Morpho.MFace.Sensor.Service.Entities.CommonResponse commonResponse)
		{
			this.Timestamp = DateTimeOffset.Parse(commonResponse.Timestamp.ToString());
			this.Status = (CommonResponseStatus)commonResponse.Status;

			string oneMessage = commonResponse.Message == null ? string.Empty : commonResponse.Message;
			this.Message = oneMessage;

			this.Duration = commonResponse.Duration;
		}

		internal CommonResponse(MFaceServiceV3Kit.CommonResponse commonResponse)
		{
			this.Timestamp = DateTimeOffset.Parse(commonResponse.Timestamp.ToString());
			this.Status = (CommonResponseStatus)commonResponse.Status;

			string oneMessage = commonResponse.Message == null ? string.Empty : commonResponse.Message;
			this.Message = oneMessage;

			this.Duration = XmlConvert.ToTimeSpan(commonResponse.Duration.ToString());
		
		}
	}
}
