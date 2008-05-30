// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.42
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Do.UI {
    
    
    public partial class PreferencesWindow {
        
        private Gtk.VBox vbox1;
        
        private Gtk.HBox hbox1;
        
        private Gtk.Notebook notebook;
        
        private Gtk.HButtonBox hbuttonbox2;
        
        private Gtk.Button btn_help;
        
        private Gtk.Button btn_close;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget Do.UI.PreferencesWindow
            this.WidthRequest = 430;
            this.HeightRequest = 500;
            this.Name = "Do.UI.PreferencesWindow";
            this.Title = Mono.Unix.Catalog.GetString("GNOME Do Preferences");
            this.Icon = Stetic.IconLoader.LoadIcon(this, "gtk-preferences", Gtk.IconSize.Menu, 16);
            this.WindowPosition = ((Gtk.WindowPosition)(1));
            this.BorderWidth = ((uint)(6));
            this.Resizable = false;
            this.AllowGrow = false;
            // Container child Do.UI.PreferencesWindow.Gtk.Container+ContainerChild
            this.vbox1 = new Gtk.VBox();
            this.vbox1.Name = "vbox1";
            this.vbox1.Spacing = 6;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.HeightRequest = 440;
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.notebook = new Gtk.Notebook();
            this.notebook.WidthRequest = 300;
            this.notebook.CanFocus = true;
            this.notebook.Name = "notebook";
            this.notebook.CurrentPage = 0;
            this.hbox1.Add(this.notebook);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.hbox1[this.notebook]));
            w1.Position = 0;
            this.vbox1.Add(this.hbox1);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox1]));
            w2.Position = 0;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hbuttonbox2 = new Gtk.HButtonBox();
            this.hbuttonbox2.Name = "hbuttonbox2";
            // Container child hbuttonbox2.Gtk.ButtonBox+ButtonBoxChild
            this.btn_help = new Gtk.Button();
            this.btn_help.CanFocus = true;
            this.btn_help.Name = "btn_help";
            this.btn_help.UseStock = true;
            this.btn_help.UseUnderline = true;
            this.btn_help.Label = "gtk-help";
            this.hbuttonbox2.Add(this.btn_help);
            Gtk.ButtonBox.ButtonBoxChild w3 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox2[this.btn_help]));
            w3.Expand = false;
            w3.Fill = false;
            // Container child hbuttonbox2.Gtk.ButtonBox+ButtonBoxChild
            this.btn_close = new Gtk.Button();
            this.btn_close.CanDefault = true;
            this.btn_close.CanFocus = true;
            this.btn_close.Name = "btn_close";
            this.btn_close.UseStock = true;
            this.btn_close.UseUnderline = true;
            this.btn_close.Label = "gtk-close";
            this.hbuttonbox2.Add(this.btn_close);
            Gtk.ButtonBox.ButtonBoxChild w4 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox2[this.btn_close]));
            w4.Position = 1;
            w4.Expand = false;
            w4.Fill = false;
            this.vbox1.Add(this.hbuttonbox2);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbuttonbox2]));
            w5.Position = 1;
            w5.Expand = false;
            w5.Fill = false;
            this.Add(this.vbox1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 510;
            this.DefaultHeight = 526;
            this.btn_close.HasDefault = true;
            this.Show();
            this.btn_help.Clicked += new System.EventHandler(this.OnBtnHelpClicked);
            this.btn_close.Clicked += new System.EventHandler(this.OnBtnCloseClicked);
        }
    }
}
