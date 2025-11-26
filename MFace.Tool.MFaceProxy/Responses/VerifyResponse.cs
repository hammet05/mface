using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MFace.Tool.MFaceProxy.Types;

namespace MFace.Tool.MFaceProxy.Responses
{
	public struct VerifyResponse
	{
		private BiometricData reference;
		private readonly short _referenceQuality;
		private readonly float _matchingScore;
		public BiometricData Reference
		{
			get
			{
				return Reference;
			}
		}

	
		public float MatchingScore
		{
			get
			{
				return _matchingScore;
			}
		}

		public short ReferenceQuality
		{
			get
			{
				return _referenceQuality;
			}
		}
		
		internal VerifyResponse(Morpho.MFace.Sensor.Service.Messages.VerifyResponse response)
		{
			this._matchingScore = response.MatchingScore;
			this._referenceQuality = response.ReferenceQuality;
			this.reference = new BiometricData(response.Reference);
		}
		internal VerifyResponse(MFaceServiceV3Kit.VerifyResponse response)
		{
			this._matchingScore = response.MatchingScore;
			this._referenceQuality = response.ReferenceQuality;
			this.reference = new BiometricData(response.Reference);
		}
	}
}
