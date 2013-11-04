using System;
using Xwt;
using System.Linq;
using System.Threading.Tasks;

namespace Cls.Actors.UI
{
	public class MainWindow : Window
	{
		StatusIcon status;
		DataField<string> nameCol = new DataField<string> ();

		MainWindowViewModel vm = new MainWindowViewModel();

		public MainWindow ()
		{
			Title = "DHT";
			Width = 800;
			Height = 600;
			this.InitialLocation = WindowLocation.CenterScreen;				
			status = Application.CreateStatusIcon();

			CreateMenu ();

			var panel = new HPaned();
			var panel2 = panel.Panel2;
			var box = new VBox();
			panel2.Content = box;
			 CreateInput(box, "Connect to node", "Connect");

			var store = new TreeStore(nameCol);
			var nodes = new TreeView(store);
			nodes.Columns.Add("Name", nameCol);
			panel.Panel1.Content = nodes;
			var conns = store.AddNode(null).SetValue(nameCol, "Nodes").CurrentPosition;
			vm.ConnectionAdded += ()=>
				store.AddNode(conns).SetValue(nameCol, vm.NewConnection);

			Content = panel;
			CloseRequested += (sender, args) => 
				Application.Exit();
		}

		void CreateInput(Box w, string label, string button){
			var hbox = new HBox();
			var text = new TextEntry();		
			text.Changed += (sender, e) => vm.NewConnection = text.Text;
			var l = new Label(label);
			hbox.PackStart(l);
			hbox.PackStart(text);
			var connect = new Button(button);	
			hbox.PackStart(connect);
			w.PackStart(hbox);
			connect.Clicked += (s,e) => 
				vm.Connect();
			connect.Sensitive = true;
					
		}

		static MenuItem CreateFileMenu ()
		{
			var file = new MenuItem ("_File");
			file.SubMenu = new Menu ();
			var mi = new MenuItem ("_Close");
			file.SubMenu.Items.Add (mi);
			mi.Clicked += (s, e) => Application.Exit ();
			return file;
		}

		void CreateMenu ()
		{
			var menu = new Menu ();
			var file = CreateFileMenu ();
			menu.Items.Add (file);
			status.Menu = menu;
			MainMenu = menu;
		}
	}
}

