using System;
using ReactiveUI.Xaml;
using Xwt;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace Actors.UI
{
	public class CommandToXwtButton : ICreatesCommandBinding
	{
		public int GetAffinityForObject(Type type, bool hasEventTarget)
		{
			if (type != typeof(Button)) return 0;
			if (hasEventTarget) return 0;
			return 10;
		}

		public IDisposable BindCommandToObject<TEventArgs>(System.Windows.Input.ICommand command, object target, IObservable<object> commandParameter, string eventName) 
			where TEventArgs : EventArgs
		{
			throw new NotImplementedException();
		}

		public IDisposable BindCommandToObject(System.Windows.Input.ICommand command, object target, IObservable<object> commandParameter)
		{
			var button = target as Button;

			var clicked = Observable.FromEventPattern<EventHandler, EventArgs>(x => button.Clicked += x, x => button.Clicked -= x);
			var canExecute = Observable.FromEventPattern<EventHandler, EventArgs>(x => command.CanExecuteChanged += x, x => command.CanExecuteChanged -= x);

			return new CompositeDisposable(
				clicked.InvokeCommand(command),
				canExecute
				.Select(_ => command.CanExecute(null))
				.StartWith(command.CanExecute(null))
				.Subscribe(x => button.Sensitive = x));
		}
	}
}

