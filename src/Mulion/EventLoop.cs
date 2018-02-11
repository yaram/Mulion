using System;
using System.Threading.Tasks;

namespace Mulion{
	public abstract class EventLoop{
		public static Task<EventLoop> Create(){
			throw new MulionException("No suitable event loop backend found");
		}

		public abstract Task<Window> CreateWindow();

		public abstract void PollEvents();

		public abstract void RunForever();

		public abstract void Quit();
	}
}