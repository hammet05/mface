using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MFace.Tool.MFaceProxy
{
	internal static class Utility
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		static internal string GetCurrentMethodName()
		{
			var st = new StackTrace();
			var sf = st.GetFrame(1);
			return sf.GetMethod().Name;
		}
	}
}
