using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFace.Tool.MFaceProxy.Responses
{
	public struct IdentifyResponse
	{
		private readonly float _matchingScore;
		private readonly string _referenceID;

		public float MatchingScore
		{
			get
			{
				return _matchingScore;
			}
		}

		public string ReferenceID
		{
			get
			{
				return _referenceID;
			}
		}

		internal IdentifyResponse(Morpho.MFace.Sensor.Service.Messages.IdentifyResponse response)
		{
			this._matchingScore = response.MatchingScore;
			this._referenceID = response.ReferenceID;
		}

		internal IdentifyResponse(MFaceServiceV3Kit.IdentifyResponse response)
		{
			this._matchingScore = response.MatchingScore;
			this._referenceID = response.ReferenceID;
		}


	}
}
