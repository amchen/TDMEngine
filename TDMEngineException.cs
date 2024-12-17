using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDMEngine
{
	public class TDMEngineException: System.Exception
	{
		public TDMEngineException() : base() { }
		public TDMEngineException(string message) : base(message) { LogWriter.LogIt(message); }
	}


	public class MigrationFailedException : System.Exception
	{
		public MigrationFailedException() : base() { }
		public MigrationFailedException(string message) : base(message) { LogWriter.LogIt(message); }
	}
}
