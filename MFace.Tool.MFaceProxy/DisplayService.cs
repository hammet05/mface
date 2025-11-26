using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MFace.Tool.Common.Interfaces;
using Morpho.MFace.Sensor.Service.Entities;
using Morpho.MFace.Sensor.Service.Messages;
using Morpho.MFace.Sensor.Service.Enumerations;
using Morpho.MFace.Sensor.Service;

namespace MFace.Tool.MFaceProxy
{
	public class DisplayService : Service
	{
		#region Private Variables
		#endregion

		public DisplayService(ILogger logger) : base(logger)
		{
		}

		#region Utility
		/// <summary>
		/// Check reponse and throw correponding Exceptions
		/// </summary>
		/// <param name="commonResponse"></param>
		private void commonResponseCheck(CommonResponse commonResponse, [CallerMemberName] string callerName = "")
		{
			try
			{
				Types.CommonResponse internalCommonResponse = new Types.CommonResponse(commonResponse);
				base.commonResponseCheck(internalCommonResponse,callerName);
			}
			catch (Exception)
			{
				logger.Log(LogLevel.DEBUG, $"{callerName} Response, Status: {commonResponse.Status}, Message: {commonResponse.Message}, Duration: {commonResponse.Duration}");
				throw;
			}
		}

		#endregion

		#region Display operations
		public bool DisplayPage(Enumerations.MediaEndAction endAction, string pageName, int timeout)
		{
			logger.Log(LogLevel.DEBUG, $"Send DisplayPage, MediaEndAction: {endAction}, PageName: {pageName}, Timeout: {timeout}");
			var request = new DisplayPageRequest()
			{
				MediaEndAction = (MediaEndAction)endAction,
				PageName = pageName,
				Timeout = TimeSpan.FromMilliseconds(timeout)
			};

			var response = service.DisplayPage(request);
			commonResponseCheck(response.CommonResponse);

			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				logger.Log(LogLevel.DEBUG, $"Result DisplayPage succeed.");
				return true;
			}else
			{
				logger.Log(LogLevel.DEBUG, $"Result DisplayPage failed.");
				return false;
			}
		}

		public bool DisplayLiveCapture(Enumerations.LiveDisplayMode liveDisplayMode)
		{
			logger.Log(LogLevel.DEBUG, $"Send DisplayLiveCapture");

			var request = new DisplayLiveCaptureRequest()
			{
				LiveDisplayMode = (LiveDisplayMode)liveDisplayMode
			};

			var response = service.DisplayLiveCapture(request);
			commonResponseCheck(response.CommonResponse);

			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				logger.Log(LogLevel.DEBUG, $"Result DisplayLiveCapture succeed.");
				return true;
			}
			else
			{
				logger.Log(LogLevel.DEBUG, $"Result DisplayLiveCapture failed.");
				return false;
			}
		}

		public void DisplayTextOverlay()
		{
			DisplayTextOverlayRequest request = new DisplayTextOverlayRequest();
			request.Color = "Red";
			request.Enable = true;
			request.Text = "NIG";
			var response = service.DisplayTextOverlay(request);

			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				logger.Log(LogLevel.DEBUG, $"Result DisplayTextOverlay succeed.");
			}
			else
			{
				logger.Log(LogLevel.DEBUG, $"Result DisplayTextOverlay failed.");
			}
		}

		#endregion

		#region Configuration operations
		public bool UploadMediaFile(byte[] media, string mediaName, string extension)
		{
			logger.Log(LogLevel.DEBUG, $"Send UploadMediaFile, Media size : {media.Length}, Media Name: {mediaName}, Extension: {extension}");
			string mediaMimeType = GetMimeType(extension);
			var request = new UploadMediaFileRequest()
			{				
				Media = new Content() { Data = media,ContentType = mediaMimeType },
				MediaName = mediaName
			};

			 var response = service.UploadMediaFile(request);
			commonResponseCheck(response.CommonResponse);

			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				logger.Log(LogLevel.DEBUG, $"Result UploadMediaFile succeed.");
				return true;
			}
			else
			{
				logger.Log(LogLevel.DEBUG, $"Result UploadMediaFile failed.");
				return false;
			}
		}

		public bool SetPageMedia(string mediaName, string pageName)
		{
			logger.Log(LogLevel.DEBUG, $"Send SetPageMedia, Media name : {mediaName}, Page Name: {pageName}");

			var request = new SetPageMediaRequest()
			{
				MediaName = mediaName,
				PageName = pageName
			};

			var response = service.SetPageMedia(request);
			commonResponseCheck(response.CommonResponse);

			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				logger.Log(LogLevel.DEBUG, $"Result SetPageMedia succeed.");
				return true;
			}
			else
			{
				logger.Log(LogLevel.DEBUG, $"Result SetPageMedia failed.");
				return false;
			}
		}

		public bool DeleteMediaFile(string mediaName)
		{
			logger.Log(LogLevel.DEBUG, $"Send DeleteMediaFile, Media name : {mediaName}");

			var request = new DeleteMediaFileRequest()
			{
				MediaName = mediaName
			};

			var response = service.DeleteMediaFile(request);
			commonResponseCheck(response.CommonResponse);

			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				logger.Log(LogLevel.DEBUG, $"Result DeleteMediaFile succeed.");
				return true;
			}
			else
			{
				logger.Log(LogLevel.DEBUG, $"Result DeleteMediaFile failed.");
				return false;
			}
		}

		public List<Types.MediaInfo> GetMediaInfo()
		{
			logger.Log(LogLevel.DEBUG, $"Send GetMediaInfo");
			List<Types.MediaInfo> result = new List<Types.MediaInfo>();

			GetMediaInfoRequest request = new GetMediaInfoRequest();

			GetMediaInfoResponse response = service.GetMediaInfo(request);
			commonResponseCheck(response.CommonResponse);

			if (response.CommonResponse.Status == CommonResponseStatus.Success)
			{
				logger.Log(LogLevel.DEBUG, $"Result DeleteMediaFile succeed.");
				foreach(MediaInfo mInfo in response.MediaInfo)
				{
					result.Add(new Types.MediaInfo(mInfo));
				}
			}
			else
			{
				logger.Log(LogLevel.DEBUG, $"Result DeleteMediaFile failed.");
			}

			return result;
		}

		protected override dynamic GetChannelFactory(string name)
		{
			return new ChannelFactory<IDisplayService>(name);
		}
		#endregion
	}
}
