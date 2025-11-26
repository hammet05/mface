using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MFace.Tool.MFaceProxy.Responses
{
	public struct GetICAOAnalysisResponse
	{
		public readonly bool _compliant;
		public readonly List<string> _nonCompliantCriteria;

		public List<string> NonCompliantCriteria
		{
			get { return _nonCompliantCriteria; }
		}

		public GetICAOAnalysisResponse(Morpho.MFace.Sensor.Service.Messages.GetICAOAnalysisResponse response)
		{
			_compliant = response.Compliant;
			_nonCompliantCriteria = response.NonCompliantCriteria.ToList();
		}

		public GetICAOAnalysisResponse(MFaceServiceV3Kit.GetICAOAnalysisResponse response)
		{
			_compliant = response.Compliant;
			_nonCompliantCriteria = response.NonCompliantCriteria.ToList();
		}
	}
}
