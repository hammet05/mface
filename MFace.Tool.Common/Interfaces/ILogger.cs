
using System.Runtime.CompilerServices;

namespace MFace.Tool.Common.Interfaces
{
	public enum LogLevel
	{
		RESULT = 1,	
		DEBUG = 2,
		INFO = 4,
		SUCCESS = 8,
		WARNING = 16,
		ERROR = 32,
		CRITICAL = 64
	}

	public interface ILogger
	{
		void Log(LogLevel level, string message, [CallerMemberName] string methodName = "::");
	}
}
