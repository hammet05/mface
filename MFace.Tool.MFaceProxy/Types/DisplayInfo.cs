using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFace.Tool.MFaceProxy.Types
{
	public struct DisplayInfo
	{
		/// <summary>
		/// MFace.Display installed version
		/// </summary>
		public string SoftwareVersion { get; }

		public DisplayInfo(string softwareVersion)
		{
			this.SoftwareVersion = softwareVersion;
		}

	}
}
