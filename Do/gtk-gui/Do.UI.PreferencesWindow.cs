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
        
        private Gtk.ScrolledWindow scroll_window;
        
        private Gtk.Notebook notebook;
        
        private Gtk.Fixed fixed5;
        
        private Gtk.Image image153;
        
        private Gtk.Label label8;
        
        private Gtk.Label label1;
        
        private Gtk.Fixed fixed3;
        
        private Gtk.Button btn_manage_plugins;
        
        private Gtk.Label label2;
        
        private Gtk.Label label6;
        
        private Gtk.HSeparator hseparator1;
        
        private Gtk.Fixed fixed1;
        
        private Gtk.Button btn_close;
        
        private Gtk.Button btn_help;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget Do.UI.PreferencesWindow
            this.WidthRequest = 580;
            this.HeightRequest = 500;
            this.Name = "Do.UI.PreferencesWindow";
            this.Title = Mono.Unix.Catalog.GetString("Preferences");
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
            this.scroll_window = new Gtk.ScrolledWindow();
            this.scroll_window.CanFocus = true;
            this.scroll_window.Name = "scroll_window";
            this.scroll_window.ShadowType = ((Gtk.ShadowType)(1));
            this.hbox1.Add(this.scroll_window);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.hbox1[this.scroll_window]));
            w1.Position = 0;
            // Container child hbox1.Gtk.Box+BoxChild
            this.notebook = new Gtk.Notebook();
            this.notebook.WidthRequest = 300;
            this.notebook.CanFocus = true;
            this.notebook.Name = "notebook";
            this.notebook.CurrentPage = 0;
            this.notebook.ShowTabs = false;
            // Container child notebook.Gtk.Notebook+NotebookChild
            this.fixed5 = new Gtk.Fixed();
            this.fixed5.Name = "fixed5";
            this.fixed5.HasWindow = false;
            // Container child fixed5.Gtk.Fixed+FixedChild
            this.image153 = new Gtk.Image();
            this.image153.Name = "image153";
            this.image153.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-dialog-info", Gtk.IconSize.Dialog, 48);
            this.fixed5.Add(this.image153);
            Gtk.Fixed.FixedChild w2 = ((Gtk.Fixed.FixedChild)(this.fixed5[this.image153]));
            w2.X = 160;
            w2.Y = 140;
            // Container child fixed5.Gtk.Fixed+FixedChild
            this.label8 = new Gtk.Label();
            this.label8.Name = "label8";
            this.label8.LabelProp = Mono.Unix.Catalog.GetString("Coming soon...");
            this.fixed5.Add(this.label8);
            Gtk.Fixed.FixedChild w3 = ((Gtk.Fixed.FixedChild)(this.fixed5[this.label8]));
            w3.X = 143;
            w3.Y = 200;
            this.notebook.Add(this.fixed5);
            // Notebook tab
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("General Preferences");
            this.notebook.SetTabLabel(this.fixed5, this.label1);
            this.label1.ShowAll();
            // Container child notebook.Gtk.Notebook+NotebookChild
            this.fixed3 = new Gtk.Fixed();
            this.fixed3.Name = "fixed3";
            this.fixed3.HasWindow = false;
            // Container child fixed3.Gtk.Fixed+FixedChild
            this.btn_manage_plugins = new Gtk.Button();
            this.btn_manage_plugins.CanFocus = true;
            this.btn_manage_plugins.Name = "btn_manage_plugins";
            this.btn_manage_plugins.UseUnderline = true;
            // Container child btn_manage_plugins.Gtk.Container+ContainerChild
            Gtk.Alignment w5 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            Gtk.HBox w6 = new Gtk.HBox();
            w6.Spacing = 2;
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Image w7 = new Gtk.Image();
            w7.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-execute", Gtk.IconSize.Button, 20);
            w6.Add(w7);
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Label w9 = new Gtk.Label();
            w9.LabelProp = Mono.Unix.Catalog.GetString("Add/Remove Plugins...");
            w9.UseUnderline = true;
            w6.Add(w9);
            w5.Add(w6);
            this.btn_manage_plugins.Add(w5);
            this.fixed3.Add(this.btn_manage_plugins);
            Gtk.Fixed.FixedChild w13 = ((Gtk.Fixed.FixedChild)(this.fixed3[this.btn_manage_plugins]));
            w13.X = 110;
            w13.Y = 100;
            this.notebook.Add(this.fixed3);
            Gtk.Notebook.NotebookChild w14 = ((Gtk.Notebook.NotebookChild)(this.notebook[this.fixed3]));
            w14.Position = 1;
            // Notebook tab
            this.label2 = new Gtk.Label();
            this.label2.Name = "label2";
            this.label2.LabelProp = Mono.Unix.Catalog.GetString("Manage Plugins");
            this.notebook.SetTabLabel(this.fixed3, this.label2);
            this.label2.ShowAll();
            // Notebook tab
            Gtk.Label w15 = new Gtk.Label();
            w15.Visible = true;
            this.notebook.Add(w15);
            this.label6 = new Gtk.Label();
            this.label6.Name = "label6";
            this.label6.LabelProp = Mono.Unix.Catalog.GetString("More...");
            this.notebook.SetTabLabel(w15, this.label6);
            this.label6.ShowAll();
            this.hbox1.Add(this.notebook);
            Gtk.Box.BoxChild w16 = ((Gtk.Box.BoxChild)(this.hbox1[this.notebook]));
            w16.Position = 1;
            this.vbox1.Add(this.hbox1);
            Gtk.Box.BoxChild w17 = ((Gtk.Box.BoxChild)(this.vbox1[this.hbox1]));
            w17.Position = 0;
            // Container child vbox1.Gtk.Box+BoxChild
            this.hseparator1 = new Gtk.HSeparator();
            this.hseparator1.Name = "hseparator1";
            this.vbox1.Add(this.hseparator1);
            Gtk.Box.BoxChild w18 = ((Gtk.Box.BoxChild)(this.vbox1[this.hseparator1]));
            w18.Position = 1;
            w18.Expand = false;
            w18.Fill = false;
            // Container child vbox1.Gtk.Box+BoxChild
            this.fixed1 = new Gtk.Fixed();
            this.fixed1.HeightRequest = 36;
            this.fixed1.Name = "fixed1";
            this.fixed1.HasWindow = false;
            // Container child fixed1.Gtk.Fixed+FixedChild
            this.btn_close = new Gtk.Button();
            this.btn_close.WidthRequest = 80;
            this.btn_close.CanFocus = true;
            this.btn_close.Name = "btn_close";
            this.btn_close.UseUnderline = true;
            // Container child btn_close.Gtk.Container+ContainerChild
            Gtk.Alignment w19 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            Gtk.HBox w20 = new Gtk.HBox();
            w20.Spacing = 2;
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Image w21 = new Gtk.Image();
            w21.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-close", Gtk.IconSize.Button, 20);
            w20.Add(w21);
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Label w23 = new Gtk.Label();
            w23.LabelProp = Mono.Unix.Catalog.GetString("_Close");
            w23.UseUnderline = true;
            w20.Add(w23);
            w19.Add(w20);
            this.btn_close.Add(w19);
            this.fixed1.Add(this.btn_close);
            Gtk.Fixed.FixedChild w27 = ((Gtk.Fixed.FixedChild)(this.fixed1[this.btn_close]));
            w27.X = 488;
            // Container child fixed1.Gtk.Fixed+FixedChild
            this.btn_help = new Gtk.Button();
            this.btn_help.WidthRequest = 80;
            this.btn_help.CanFocus = true;
            this.btn_help.Name = "btn_help";
            this.btn_help.UseUnderline = true;
            // Container child btn_help.Gtk.Container+ContainerChild
            Gtk.Alignment w28 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            Gtk.HBox w29 = new Gtk.HBox();
            w29.Spacing = 2;
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Image w30 = new Gtk.Image();
            w30.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-help", Gtk.IconSize.Button, 20);
            w29.Add(w30);
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Label w32 = new Gtk.Label();
            w32.LabelProp = Mono.Unix.Catalog.GetString("Help");
            w32.UseUnderline = true;
            w29.Add(w32);
            w28.Add(w29);
            this.btn_help.Add(w28);
            this.fixed1.Add(this.btn_help);
            this.vbox1.Add(this.fixed1);
            Gtk.Box.BoxChild w37 = ((Gtk.Box.BoxChild)(this.vbox1[this.fixed1]));
            w37.Position = 2;
            w37.Expand = false;
            w37.Fill = false;
            this.Add(this.vbox1);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.DefaultWidth = 588;
            this.DefaultHeight = 526;
            this.Show();
            this.btn_manage_plugins.Clicked += new System.EventHandler(this.OnBtnManagePluginsClicked);
            this.btn_close.Clicked += new System.EventHandler(this.OnBtnCloseClicked);
            this.btn_help.Clicked += new System.EventHandler(this.OnBtnHelpClicked);
        }
    }
}