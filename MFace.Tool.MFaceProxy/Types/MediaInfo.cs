using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFace.Tool.MFaceProxy.Types
{
	public class MediaInfo
	{
		public string MediaContentType { get;}
		public string Name { get; }
		public string[] Pages { get; }
		public MediaInfo(Morpho.MFace.Sensor.Service.Entities.MediaInfo mediaInfo)
		{
			MediaContentType = mediaInfo.MediaContentType;
			Name = mediaInfo.Name;
			Pages = mediaInfo.Pages;			
		}
	}
}
