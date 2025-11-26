using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFace.Tool.MFaceProxy.Enumerations;

namespace MFace.Tool.MFaceProxy.Types
{
	public struct SelfTestStatus
	{

		public SelfTestLevel level { get; }
	    public SelfTestId id { get; }
		public SelfTestStatus(Morpho.MFace.Sensor.Service.Enumerations.SelfTestId _id , Morpho.MFace.Sensor.Service.Enumerations.SelfTestLevel _level)
		{
			this.id = (SelfTestId)_id;
			this.level = (SelfTestLevel)_level;
		}

	}
}
