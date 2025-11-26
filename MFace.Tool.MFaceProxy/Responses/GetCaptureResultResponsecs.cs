using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MFace.Tool.MFaceProxy.Types;

namespace MFace.Tool.MFaceProxy.Responses
{
	public struct GetCaptureResultResponse
	{
		private readonly List<PersonData> _captureResultDatas;
	 
		public List<PersonData> PersonDataList
		{
			get { return _captureResultDatas; }
		}
		
		internal GetCaptureResultResponse(Morpho.MFace.Sensor.Service.Messages.GetCaptureResultResponse response)
		{
			_captureResultDatas = new List<PersonData>();
			foreach (Morpho.MFace.Sensor.Service.Entities.PersonData data in response.PersonDataList)
			{
				_captureResultDatas.Add(new PersonData(data));
			}
		}
		internal GetCaptureResultResponse(MFaceServiceV3Kit.GetCaptureResultResponse response)
		{
			_captureResultDatas = new List<PersonData>();
			foreach (MFaceServiceV3Kit.PersonData data in response.PersonDataList)
			{
				_captureResultDatas.Add(new PersonData(data));
			}
		}
	}
}


