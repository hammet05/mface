using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFace.Tool.MFaceProxy.Types
{
	/// <summary>
	/// Describe a content with type (MIME type) and data.
	/// </summary>
	public struct Content
	{
		/// <summary>
		/// The type of the content (MIME type).
		/// </summary>
		public string ContentType { get; }

		/// <summary>
		/// The content's data.
		/// </summary>
		public byte[] Data { get; }

		internal Content(Morpho.MFace.Sensor.Service.Entities.Content content)
		{
			this.ContentType = content.ContentType;
			this.Data = content.Data;
		}

		internal Content(MFaceServiceV3Kit.Content content)
		{
			this.ContentType = content.ContentType;
			this.Data = content.Data;
		}
	}
}
