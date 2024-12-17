using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDMEngine
{

	public delegate void SetProgressMessageEventHandler(string msg);
	public delegate void SetEngineResultEventHandler(string msg);
	public delegate void SetProcessFailedEventHandler(string msg);
	public delegate void SetProgressPositionEventHandler(int position);
	public delegate void SetProgressRangeEventHandler(int range);

	public class EqeSysTDMEvent
	{
		public event SetProgressMessageEventHandler SetProgressMessage;
		public event SetEngineResultEventHandler SetEngineResult;
		public event SetProcessFailedEventHandler SetProcessFailed;
		public event SetProgressPositionEventHandler SetProgressPosition;
		public event SetProgressRangeEventHandler SetProgressRange;
		public void FireProgressMessageEvent(string msg)
		{
			if (SetProgressMessage != null)
			{
				SetProgressMessage(msg);
			}			
		}

		public void FireSetEngineResultEvent(string msg)
		{
			if (SetEngineResult != null)
			{
				SetEngineResult(msg);
			}
		}

		public  void FireSetProcessFailedEvent(string msg)
		{
			if (SetProcessFailed != null)
			{
				SetProcessFailed(msg);
			}
		}

		private void FireSetProgressPositionEvent(int position)
		{

			if (SetProgressPosition != null)
			{
				SetProgressPosition(position);
			}
		}

		public void FiretSetProgressRangeEvent(int range)
		{

			if (SetProgressRange != null)
			{
				SetProgressRange(range);
			}
		}
	}
}
