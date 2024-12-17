using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDMEngine
{
	enum EnumPos { TOP, MIDDLE, BOTTOM, CURRENT_TIME, TOTAL_TIME };
	class ProgressHandler
	{
		EqeSysTDMEvent _eqeSysTDMEvent;

		public ProgressHandler(EqeSysTDMEvent eqeSysTDMEvent)
		{
			_eqeSysTDMEvent = eqeSysTDMEvent;
		}

		public void SetProgressRange(EnumPos barPos, long range)
		{
			// "~n:range~<number>"
			string msg = "~" + barPos.ToString() + ":range~" + range.ToString();
			_eqeSysTDMEvent.FireProgressMessageEvent(msg);
		}

		public void SetProgresMessage(EnumPos barPos, string messsage)
		{
			// "~n:message~<text message>"
			string msg = "~" + barPos.ToString() + ":message~" + messsage;
			_eqeSysTDMEvent.FireProgressMessageEvent(msg);

		}

		public void SetProgressPosition(EnumPos barPos, long position)
		{

			// "~n:position~<number>
			string msg = "~" + barPos.ToString() + ":position~" + position.ToString();
			_eqeSysTDMEvent.FireProgressMessageEvent(msg);

		}

		public void SetFailedMessage(string msg)
		{
			_eqeSysTDMEvent.FireSetProcessFailedEvent(msg);
		}

		public void SetSuccessMessage(string msg)
		{
			_eqeSysTDMEvent.FireSetEngineResultEvent(msg);
		}
	}
}
