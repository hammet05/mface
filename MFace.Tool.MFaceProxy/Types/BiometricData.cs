using MFace.Tool.MFaceProxy.Enumerations;

namespace MFace.Tool.MFaceProxy.Types
{
	/// <summary>
	/// Describe biometric data structure with the biometric type and the content.
	/// </summary>
	public struct BiometricData
	{
		/// <summary>
		/// Biometric type.
		/// </summary>
		public BiometricType BiometricType { get; }

		/// <summary>
		/// Source Type
		/// </summary>
		public SourceType SourceType { get; }

		/// <summary>
		/// Data content.
		/// </summary>
		public Content Content { get; }

		internal BiometricData(Morpho.MFace.Sensor.Service.Entities.BiometricData biometricData)
		{
			this.BiometricType = (BiometricType)biometricData.BiometricType;
			this.SourceType = (SourceType)biometricData.SourceType;
			this.Content = new Content(biometricData.Content);
		}
		internal BiometricData(MFaceServiceV3Kit.BiometricData biometricData)
		{
			this.BiometricType = (BiometricType)biometricData.BiometricType;
			this.SourceType = (SourceType)biometricData.SourceType;
			this.Content = new Content(biometricData.Content);
		}

	}
}
