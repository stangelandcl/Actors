using System;
using Xwt;

namespace Cls.Actors
{
	public class InputBox : HBox
	{
		public Label Label;
		public TextEntry TextEntry;
		public Button Button;

		public InputBox(string label, string button)
			: base()
		{
			TextEntry = new TextEntry ();
			Label = new Label (label);
			Button = new Button (button);
			PackStart (Label);
			PackStart (TextEntry);
			PackStart (Button);
			Button.Sensitive = true;
		}
	}
}

