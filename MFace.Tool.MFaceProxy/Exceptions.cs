using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MFace.Tool.MFaceProxy.Enumerations;

namespace MFace.Tool.MFaceProxy
{
	[Serializable]
	public class SensorException : Exception
	{
		public SensorException(string message) : base(message)
		{ }

		protected SensorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{

		}
	}

	[Serializable]
	public class ChannelException : SensorException
	{
		private readonly string _channelName = null;

		public ChannelException(string message, string channelName) : base(message)
		{
			this._channelName = channelName;
		}

		public string getSensorChannelName()
		{
			return this._channelName;
		}
	}

	[Serializable]
	public class InvalidIdException : SensorException
	{
		public InvalidIdException(string message) : base(message)
		{ }
	}

	[Serializable]
	public class NoSuchParameterException : SensorException
	{
		public NoSuchParameterException(string message) : base(message)
		{ }
	}

	[Serializable]
	public class BadValueException : SensorException
	{
		public BadValueException(string message) : base(message)
		{ }
	}

	[Serializable]
	public class UnsupportedException : SensorException
	{
		public UnsupportedException(string message) : base(message)
		{ }
	}

	[Serializable]
	public class CanceledWithSensorFailureException : SensorException
	{
		public CanceledWithSensorFailureException(string message) : base(message)
		{ }
	}

	[Serializable]
	public class CanceledException : SensorException
	{
		public CanceledException(string message) : base(message)
		{ }
	}

	[Serializable]
	public class SensorBusyException : SensorException
	{
		public SensorBusyException(string message) : base(message)
		{ }
	}

	[Serializable]
	public class SensorFailureException : SensorException
	{
		public SensorFailureException(string message) : base(message)
		{ }
	}

	[Serializable]
	public class SensorTimeoutException : SensorException
	{
		public SensorTimeoutException(string message) : base(message)
		{ }
	}

	[Serializable]
	public class InitializationNeededException : SensorException
	{
		public InitializationNeededException(string message) : base(message)
		{ }
	}

	[Serializable]
	public class FailureException : SensorException
	{
		public FailureException(string message) : base(message)
		{ }
	}

	public class NoDataAvailableException : SensorException
	{
		public NoDataAvailableException(string message) : base(message)
		{ }
	}
}
