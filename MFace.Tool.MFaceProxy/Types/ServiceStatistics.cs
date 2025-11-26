using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFace.Tool.MFaceProxy.Types
{
	public struct ServiceStatistics
	{
		public int TotalCaptureCount { get ; }

		public int TotalVerifyCount { get; }

		public ServiceStatistics(int totalCaptureTimeCount, int totalVerifyCount)
		{
			this.TotalCaptureCount = totalCaptureTimeCount;
			this.TotalVerifyCount = totalVerifyCount;
		}
	}
	

}