using System;
using Xwt;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI.Xaml;

namespace Actors.UI
{
	public abstract class View<T> : Window, IViewFor<T> where T : class
	{
		public View(){
			RxApp.Register(typeof(CommandToXwtButton), typeof(ICreatesCommandBinding));
		}

		public T ViewModel { get; set; }

		object IViewFor.ViewModel {
			get { return ViewModel; }
			set { ViewModel = (T)value; }
		}

		protected IObservable<Unit> GetTextChangedForTextBox(TextEntry textBox)
		{
			return Observable.FromEventPattern<EventHandler, EventArgs>(
				x => textBox.Changed += x, 
				x => textBox.Changed -= x)
				.Select(_ => Unit.Default);
		}
	}
}

