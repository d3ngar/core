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
    
    
    public partial class PluginConfigurationWindow {
        
        private Gtk.VBox vbox2;
        
        private Gtk.Notebook notebook;
        
        private Gtk.Label label2;
        
        private Gtk.HButtonBox hbuttonbox2;
        
        private Gtk.Button btn_close;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget Do.UI.PluginConfigurationWindow
            this.WidthRequest = 230;
            this.HeightRequest = 360;
            this.Name = "Do.UI.PluginConfigurationWindow";
            this.Title = Mono.Unix.Catalog.GetString("PluginConfigurationWindow");
            this.Icon = Stetic.IconLoader.LoadIcon(this, "gtk-preferences", Gtk.IconSize.Menu, 16);
            this.WindowPosition = ((Gtk.WindowPosition)(1));
            this.BorderWidth = ((uint)(6));
            // Container child Do.UI.PluginConfigurationWindow.Gtk.Container+ContainerChild
            this.vbox2 = new Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            // Container child vbox2.Gtk.Box+BoxChild
            this.notebook = new Gtk.Notebook();
            this.notebook.CanFocus = true;
            this.notebook.Name = "notebook";
            this.notebook.CurrentPage = 0;
            // Notebook tab
            Gtk.Label w1 = new Gtk.Label();
            w1.Visible = true;
            this.notebook.Add(w1);
            this.label2 = new Gtk.Label();
            this.label2.Name = "label2";
            this.label2.LabelProp = Mono.Unix.Catalog.GetString("page1");
            this.notebook.SetTabLabel(w1, this.label2);
            this.label2.ShowAll();
            this.vbox2.Add(this.notebook);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.vbox2[this.notebook]));
            w2.Position = 0;
            // Container child vbox2.Gtk.Box+BoxChild
            this.hbuttonbox2 = new Gtk.HButtonBox();
            this.hbuttonbox2.Name = "hbuttonbox2";
            this.hbuttonbox2.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child hbuttonbox2.Gtk.ButtonBox+ButtonBoxChild
            this.btn_close = new Gtk.Button();
            this.btn_close.CanFocus = true;
            this.btn_close.Name = "btn_close";
            this.btn_close.UseStock = true;
            this.btn_close.UseUnderline = true;
            this.btn_close.Label = "gtk-close";
            this.hbuttonbox2.Add(this.btn_close);
            Gtk.ButtonBox.ButtonBoxChild w3 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox2[this.btn_close]));
            w3.Expand = false;
            w3.Fill = false;
            this.vbox2.Add(this.hbuttonbox2);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.vbox2[this.hbuttonbox2]));
            w4.Position = 1;
            w4.Expand = false;
            w4.Fill = false;
            this.Add(this.vbox2);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 400;
            this.DefaultHeight = 386;
            this.Show();
            this.btn_close.Clicked += new System.EventHandler(this.OnBtnCloseClicked);
        }
    }
}
