using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Project_Cyanide {
	class ProcessHandler {

		private Process _process;

		private Dictionary<String, Action<Process>> _actions;

		public ProcessHandler( Process process ) {
			_actions = new Dictionary<string, Action<Process>>();

			_process = process;
		}

		public Boolean InvokeAction( String name ) {
			if ( !_actions.TryGetValue(name, out Action<Process> action) )
				return false;

			action.Invoke(_process);
			return true;
		}
		public void AddAction( String name, Action<Process> action ) {
			_actions.Add(name, action);
		}
	}
}
