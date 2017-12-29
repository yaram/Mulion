using System;

namespace Mulion{
	interface IEventLoopBackend{
		Action Quit{set;}

		void PollEvents();
		void RunForever();
		void OnQuit();
	}
}