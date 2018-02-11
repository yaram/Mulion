using System;
using System.Threading.Tasks;
using Mulion.Windows;

namespace Mulion{
	public abstract class EventLoop{
		public event Action Quit;

		public static Task<EventLoop> Create(){
			if(Environment.OSVersion.Platform == PlatformID.Win32NT){
				return Task.FromResult<EventLoop>(new Win32EventLoop());
			}

			throw new MulionException("No suitable event loop backend found");
		}

		public abstract Task<Window> CreateWindow();

		public abstract void PollEvents();

		public abstract void RunForever();

		public abstract void PostQuit();

		protected void OnQuit(){
			Quit?.Invoke();
		}
	}
}