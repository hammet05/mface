using MFace.Tool.MFaceProxy.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MFace.Tool.MFaceProxy.Responses
{
	public struct GetCaptureLiveDataResponse
	{
		private readonly Size _liveImageSize;
		private readonly bool _faceDetected;
		private readonly bool _hasLiveImageSize;
		private readonly List<CaptureLiveData> _captureLiveDatas;
		private readonly byte[] _liveImage;

		public Size LiveImageSize
		{
			get { return _liveImageSize; }
		}
		public bool FaceDetected
		{
			get { return _faceDetected; }
		}
		public bool HasLiveImageSize
		{
			get { return _hasLiveImageSize; }
		}
		public List<CaptureLiveData> CaptureLiveDatas
		{
			get { return _captureLiveDatas; }
		}
		public byte[] LiveImage
		{
			get { return _liveImage; }
		}

		internal GetCaptureLiveDataResponse(Morpho.MFace.Sensor.Service.Messages.GetCaptureLiveDataResponse response)
		{
			_captureLiveDatas = new List<CaptureLiveData>();
			foreach(Morpho.MFace.Sensor.Service.Entities.CaptureLiveData data in response.CaptureLiveDataList)
			{
				_captureLiveDatas.Add(new CaptureLiveData(data));
			}
			this._faceDetected = response.FaceDetected;
			this._liveImage = response.CaptureLiveImage.Content.Data;
			this._hasLiveImageSize = response.HasLiveImageSize;
			double Height = response.LiveImageSize.Height;
			double Width = response.LiveImageSize.Width;
			if(Height > 0 && Width > 0)
			{
				this._liveImageSize = new Size(Height, Width);
			}
			else
			{
				this._liveImageSize = new Size();
			}
		}
		internal GetCaptureLiveDataResponse(MFaceServiceV3Kit.GetCaptureLiveDataResponse response)
		{
			_captureLiveDatas = new List<CaptureLiveData>();
			if(response.CaptureLiveDataList != null)
			{
				foreach (MFaceServiceV3Kit.CaptureLiveData data in response.CaptureLiveDataList)
				{
					_captureLiveDatas.Add(new CaptureLiveData(data));
				}
			}

			this._faceDetected = response.FaceDetected;			
			this._hasLiveImageSize = response.HasLiveImageSize;

			if (response.CaptureLiveImage != null)
				this._liveImage = response.CaptureLiveImage.Content.Data;
			else
				this._liveImage = new byte[0];

			if (response.LiveImageSize != null)
            {
				double Width = response.LiveImageSize._width;
				double Height = response.LiveImageSize._height;

				if (Height > 0 && Width > 0)
				{
					this._liveImageSize = new Size(Height, Width);
				}
				else
					this._liveImageSize = new Size();
			}
			else
				this._liveImageSize = new Size();

		}
	}
}
