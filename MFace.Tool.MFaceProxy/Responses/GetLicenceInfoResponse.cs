using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFace.Tool.MFaceProxy.Responses
{
	public struct GetLicensesInfoResponse
	{
		private readonly string _serial;
		private readonly string[] _installedLicenses;
		public string Serial
		{ 
			get { return _serial;}
		}
		public string[] Installedicences
		{
			get { return _installedLicenses; }
		}

		public GetLicensesInfoResponse(string serial, string[] installedLicenses)
		{
			this._serial = serial;

			this._installedLicenses = installedLicenses;

		}
		
	}
}
