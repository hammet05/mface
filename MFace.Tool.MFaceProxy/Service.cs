using MFace.Tool.MFaceProxy.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Media3D;
using System.ServiceModel;
using MFace.Tool.Common.Interfaces;
using MFace.Tool.MFaceProxy.Enumerations;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MFace.Tool.MFaceProxy
{
	public abstract class Service
	{
		protected ILogger logger;
		#region Private variables
		protected dynamic channelFactory;
		protected dynamic service;
		private object serviceLastestDateTimeLocker = new object();
		private DateTimeOffset serviceLastestDateTime = DateTimeOffset.Now;
		#endregion
		public DateTimeOffset ServiceLastestDateTime
		{
			get
			{				
				lock (serviceLastestDateTimeLocker)
				{
					return serviceLastestDateTime;
				}
			}
			set
			{
				lock(serviceLastestDateTimeLocker)
				{
					serviceLastestDateTime = value;
				}
			}
		}

		protected void commonResponseCheck(CommonResponse commonResponse, [CallerMemberName] string methodName = "")
		{
			if(methodName != "GetServiceAlarm")
				logger.Log(LogLevel.DEBUG, $"{methodName} Response, Status: {commonResponse.Status}, Message: {commonResponse.Message}, Duration: {commonResponse.Duration}");

			string oneMessage = commonResponse.Message == null ? string.Empty : commonResponse.Message;
			string message = $"{commonResponse.Status}: {oneMessage}";
			ServiceLastestDateTime = commonResponse.Timestamp;

			switch (commonResponse.Status)
			{
				case CommonResponseStatus.InvalidId:
					throw new InvalidIdException(message);
				case CommonResponseStatus.NoSuchParameter:
					throw new NoSuchParameterException(message);
				case CommonResponseStatus.BadValue:
					throw new BadValueException(message);
				case CommonResponseStatus.Unsupported:
					throw new UnsupportedException(message);
				case CommonResponseStatus.CanceledWithSensorFailure:
					throw new CanceledWithSensorFailureException(message);
				case CommonResponseStatus.Canceled:
					throw new CanceledException(message);
				case CommonResponseStatus.SensorBusy:
					throw new SensorBusyException(message);
				case CommonResponseStatus.SensorFailure:
					throw new SensorFailureException(message);
				case CommonResponseStatus.SensorTimeout:
					throw new SensorTimeoutException(message);
				case CommonResponseStatus.InitializationNeeded:
					throw new InitializationNeededException(message);
				case CommonResponseStatus.Failure:
					throw new FailureException(message);
				case CommonResponseStatus.NoDataAvailable:
					throw new NoDataAvailableException(message);
				case CommonResponseStatus.Success:
					break;
				default:
					break;
			}
		}

		public Service(ILogger logger)
		{
			this.logger = logger;
		}

		/// <summary>
		/// Create the channel
		/// </summary>
		/// <param name="name"></param>
		public void CreateChannel(string name)
		{
			try
			{
				// Close sensor client channel factory
				if (channelFactory?.State == CommunicationState.Opened)
				{
					this.channelFactory.Close();
					this.channelFactory = null;
				}
				channelFactory = GetChannelFactory(name);

				channelFactory.Open();
				if (channelFactory.State != CommunicationState.Opened)
					throw new ChannelException($"Failed to open Sensor Service Channel Factory, state:{channelFactory.State}", name);

				this.logger.Log(LogLevel.DEBUG, $"Sensor Service Channel Factory is {channelFactory.State}");

				this.service = channelFactory.CreateChannel();				
			}
			catch (ChannelException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new ChannelException($"CreateChannelToMFace exception: {ex.Message}", name);
			}
		}

		protected abstract dynamic GetChannelFactory(string name);

		public string GetMimeType(string extension)
		{
			return MimeTypes.MimeTypeMap.GetMimeType(extension);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		protected string GetCurrentMethodName()
		{
			var st = new StackTrace();
			var sf = st.GetFrame(1);

			return sf.GetMethod().Name;
		}
	}
}
