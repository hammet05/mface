using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFace.Tool.Common
{
	public static class Utility
	{
		#region Utility
		public static string GetMimeType(string fileExtension)
		{
			return MimeTypes.MimeTypeMap.GetMimeType(fileExtension);
		}
		#endregion
	}
}
