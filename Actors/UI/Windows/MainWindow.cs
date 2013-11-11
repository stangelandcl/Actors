using System;
using Xwt;
using System.Linq;
using System.Threading.Tasks;
using Cls.Extensions;

namespace Cls.Actors.UI
{
	public class MainWindow : Window
	{
		public MainWindow ()
		{
			SetSize ();
			CreateMenu ();

			var panel = new HPaned();
			CreateTreeView (panel);
			CreateButtons (panel);

			Content = panel;
			CloseRequested += (sender, args) => 
				Application.Exit();
		}

		StatusIcon status;
		DataField<string> nameCol = new DataField<string> ();
		MainWindowViewModel vm = new MainWindowViewModel();
		TreeView tree;
		TreeStore store;

		void SetSize ()
		{
			Title = "DHT";
			Width = 800;
			Height = 600;
			this.InitialLocation = WindowLocation.CenterScreen;
			status = Application.CreateStatusIcon ();
		}

		void CreateButtons (HPaned panel)
		{
			var panel2 = panel.Panel2;
			var box = new VBox ();
			panel2.Content = box;
			box.PackStart (CreateConnectButton ());
			box.PackStart (CreateListenButton ());
		}

		void CreateTreeView (HPaned panel)
		{
			store = new TreeStore (nameCol);
			tree = new TreeView (store);
			tree.Columns.Add ("Name", nameCol);
			panel.Panel1.Content = tree;
			var conns = store.AddNode (null).SetValue (nameCol, "Nodes").CurrentPosition;
			vm.ConnectionAdded += (a, b) => 
                store.AddNode (conns).SetValue (nameCol, a + "-" + b.Id);
			tree.SelectionChanged += 
                HandleSelectionChanged;

		}

		void HandleSelectionChanged (object sender, EventArgs e)
		{
			if (tree.SelectedRow == null)
				return;
			var name = store.GetNavigatorAt (tree.SelectedRow).GetValue (nameCol);
			if (name == "Nodes")
				return;
			var nodeWindow = new NodeWindow (name);
			nodeWindow.Show ();
		}

		InputBox CreateConnectButton ()
		{
			var input = new InputBox ("Connect to node", "Connect");
			input.Button.Clicked += (s, e) => vm.Connect (input.TextEntry.Text);
			return input;
		}
		InputBox CreateListenButton(){
			var input = new InputBox ("Create node on port", "Listen");
			input.Button.Clicked += (s, e) => vm.Listen (input.TextEntry.Text.Convert<int> ());
			return input;
		}
		void CreateNode(InputBox b){
			var port = b.TextEntry.Text.Convert<int> ();
			var node = new TcpNode (port);
			node.Listen (port);
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
            try
            {
                status.Menu = menu;
            }
            catch { }
			MainMenu = menu;
		}
	}
}

