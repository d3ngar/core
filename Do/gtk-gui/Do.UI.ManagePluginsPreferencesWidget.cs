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
    
    
    public partial class ManagePluginsPreferencesWidget {
        
        private Gtk.VBox vbox3;
        
        private Gtk.HBox hbox1;
        
        private Gtk.Alignment alignment1;
        
        private Gtk.Label label1;
        
        private Gtk.ComboBox show_combo;
        
        private Gtk.Alignment alignment2;
        
        private Gtk.Label label2;
        
        private Gtk.Entry search_entry;
        
        private Gtk.ScrolledWindow scrollw;
        
        private Gtk.HBox hbox2;
        
        private Gtk.HButtonBox hbuttonbox1;
        
        private Gtk.Button btn_configure;
        
        private Gtk.Button btn_about;
        
        private Gtk.HButtonBox hbuttonbox2;
        
        private Gtk.Button btn_refresh;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget Do.UI.ManagePluginsPreferencesWidget
            Stetic.BinContainer.Attach(this);
            this.Name = "Do.UI.ManagePluginsPreferencesWidget";
            // Container child Do.UI.ManagePluginsPreferencesWidget.Gtk.Container+ContainerChild
            this.vbox3 = new Gtk.VBox();
            this.vbox3.Name = "vbox3";
            this.vbox3.Spacing = 6;
            this.vbox3.BorderWidth = ((uint)(6));
            // Container child vbox3.Gtk.Box+BoxChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.alignment1 = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.alignment1.Name = "alignment1";
            // Container child alignment1.Gtk.Container+ContainerChild
            this.label1 = new Gtk.Label();
            this.label1.Name = "label1";
            this.label1.LabelProp = Mono.Unix.Catalog.GetString("Show:");
            this.alignment1.Add(this.label1);
            this.hbox1.Add(this.alignment1);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.hbox1[this.alignment1]));
            w2.Position = 0;
            w2.Expand = false;
            w2.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.show_combo = Gtk.ComboBox.NewText();
            this.show_combo.Name = "show_combo";
            this.show_combo.Active = 0;
            this.hbox1.Add(this.show_combo);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.hbox1[this.show_combo]));
            w3.Position = 1;
            w3.Expand = false;
            w3.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.alignment2 = new Gtk.Alignment(0.5F, 0.5F, 1F, 1F);
            this.alignment2.Name = "alignment2";
            this.alignment2.LeftPadding = ((uint)(15));
            // Container child alignment2.Gtk.Container+ContainerChild
            this.label2 = new Gtk.Label();
            this.label2.Name = "label2";
            this.label2.LabelProp = Mono.Unix.Catalog.GetString("Search:");
            this.alignment2.Add(this.label2);
            this.hbox1.Add(this.alignment2);
            Gtk.Box.BoxChild w5 = ((Gtk.Box.BoxChild)(this.hbox1[this.alignment2]));
            w5.Position = 2;
            w5.Expand = false;
            w5.Fill = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.search_entry = new Gtk.Entry();
            this.search_entry.CanDefault = true;
            this.search_entry.CanFocus = true;
            this.search_entry.Name = "search_entry";
            this.search_entry.IsEditable = true;
            this.search_entry.InvisibleChar = '●';
            this.hbox1.Add(this.search_entry);
            Gtk.Box.BoxChild w6 = ((Gtk.Box.BoxChild)(this.hbox1[this.search_entry]));
            w6.Position = 3;
            this.vbox3.Add(this.hbox1);
            Gtk.Box.BoxChild w7 = ((Gtk.Box.BoxChild)(this.vbox3[this.hbox1]));
            w7.Position = 0;
            w7.Expand = false;
            w7.Fill = false;
            // Container child vbox3.Gtk.Box+BoxChild
            this.scrollw = new Gtk.ScrolledWindow();
            this.scrollw.CanFocus = true;
            this.scrollw.Name = "scrollw";
            this.scrollw.ShadowType = ((Gtk.ShadowType)(1));
            this.vbox3.Add(this.scrollw);
            Gtk.Box.BoxChild w8 = ((Gtk.Box.BoxChild)(this.vbox3[this.scrollw]));
            w8.Position = 1;
            // Container child vbox3.Gtk.Box+BoxChild
            this.hbox2 = new Gtk.HBox();
            this.hbox2.Name = "hbox2";
            this.hbox2.Spacing = 6;
            // Container child hbox2.Gtk.Box+BoxChild
            this.hbuttonbox1 = new Gtk.HButtonBox();
            this.hbuttonbox1.Name = "hbuttonbox1";
            this.hbuttonbox1.Spacing = 6;
            this.hbuttonbox1.LayoutStyle = ((Gtk.ButtonBoxStyle)(3));
            // Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
            this.btn_configure = new Gtk.Button();
            this.btn_configure.Sensitive = false;
            this.btn_configure.CanFocus = true;
            this.btn_configure.Name = "btn_configure";
            this.btn_configure.UseUnderline = true;
            // Container child btn_configure.Gtk.Container+ContainerChild
            Gtk.Alignment w9 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            Gtk.HBox w10 = new Gtk.HBox();
            w10.Spacing = 2;
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Image w11 = new Gtk.Image();
            w11.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-preferences", Gtk.IconSize.Button, 20);
            w10.Add(w11);
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Label w13 = new Gtk.Label();
            w13.LabelProp = Mono.Unix.Catalog.GetString("_Configure");
            w13.UseUnderline = true;
            w10.Add(w13);
            w9.Add(w10);
            this.btn_configure.Add(w9);
            this.hbuttonbox1.Add(this.btn_configure);
            Gtk.ButtonBox.ButtonBoxChild w17 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1[this.btn_configure]));
            w17.Expand = false;
            w17.Fill = false;
            // Container child hbuttonbox1.Gtk.ButtonBox+ButtonBoxChild
            this.btn_about = new Gtk.Button();
            this.btn_about.Sensitive = false;
            this.btn_about.CanFocus = true;
            this.btn_about.Name = "btn_about";
            this.btn_about.UseUnderline = true;
            // Container child btn_about.Gtk.Container+ContainerChild
            Gtk.Alignment w18 = new Gtk.Alignment(0.5F, 0.5F, 0F, 0F);
            // Container child GtkAlignment.Gtk.Container+ContainerChild
            Gtk.HBox w19 = new Gtk.HBox();
            w19.Spacing = 2;
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Image w20 = new Gtk.Image();
            w20.Pixbuf = Stetic.IconLoader.LoadIcon(this, "gtk-about", Gtk.IconSize.Menu, 16);
            w19.Add(w20);
            // Container child GtkHBox.Gtk.Container+ContainerChild
            Gtk.Label w22 = new Gtk.Label();
            w22.LabelProp = Mono.Unix.Catalog.GetString("_About");
            w22.UseUnderline = true;
            w19.Add(w22);
            w18.Add(w19);
            this.btn_about.Add(w18);
            this.hbuttonbox1.Add(this.btn_about);
            Gtk.ButtonBox.ButtonBoxChild w26 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox1[this.btn_about]));
            w26.Position = 1;
            w26.Expand = false;
            w26.Fill = false;
            this.hbox2.Add(this.hbuttonbox1);
            Gtk.Box.BoxChild w27 = ((Gtk.Box.BoxChild)(this.hbox2[this.hbuttonbox1]));
            w27.Position = 0;
            // Container child hbox2.Gtk.Box+BoxChild
            this.hbuttonbox2 = new Gtk.HButtonBox();
            this.hbuttonbox2.Name = "hbuttonbox2";
            this.hbuttonbox2.LayoutStyle = ((Gtk.ButtonBoxStyle)(4));
            // Container child hbuttonbox2.Gtk.ButtonBox+ButtonBoxChild
            this.btn_refresh = new Gtk.Button();
            this.btn_refresh.CanFocus = true;
            this.btn_refresh.Name = "btn_refresh";
            this.btn_refresh.UseStock = true;
            this.btn_refresh.UseUnderline = true;
            this.btn_refresh.Label = "gtk-refresh";
            this.hbuttonbox2.Add(this.btn_refresh);
            Gtk.ButtonBox.ButtonBoxChild w28 = ((Gtk.ButtonBox.ButtonBoxChild)(this.hbuttonbox2[this.btn_refresh]));
            w28.Expand = false;
            w28.Fill = false;
            this.hbox2.Add(this.hbuttonbox2);
            Gtk.Box.BoxChild w29 = ((Gtk.Box.BoxChild)(this.hbox2[this.hbuttonbox2]));
            w29.Position = 1;
            this.vbox3.Add(this.hbox2);
            Gtk.Box.BoxChild w30 = ((Gtk.Box.BoxChild)(this.vbox3[this.hbox2]));
            w30.Position = 2;
            w30.Expand = false;
            w30.Fill = false;
            this.Add(this.vbox3);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
            this.show_combo.Changed += new System.EventHandler(this.OnShowComboChanged);
            this.search_entry.Changed += new System.EventHandler(this.OnSearchEntryChanged);
            this.btn_configure.Clicked += new System.EventHandler(this.OnBtnConfigurePluginClicked);
            this.btn_about.Clicked += new System.EventHandler(this.OnBtnAboutClicked);
            this.btn_refresh.Clicked += new System.EventHandler(this.OnBtnRefreshClicked);
        }
    }
}
