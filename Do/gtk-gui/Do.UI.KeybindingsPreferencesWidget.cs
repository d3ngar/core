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
    
    
    public partial class KeybindingsPreferencesWidget {
        
        private Gtk.VBox vbox2;
        
        private Gtk.ScrolledWindow action_scroll;
        
        private Gtk.HBox hbox1;
        
        private Gtk.Image help_icn;
        
        private Gtk.Label help_lbl;
        
        protected virtual void Build() {
            Stetic.Gui.Initialize(this);
            // Widget Do.UI.KeybindingsPreferencesWidget
            Stetic.BinContainer.Attach(this);
            this.Name = "Do.UI.KeybindingsPreferencesWidget";
            // Container child Do.UI.KeybindingsPreferencesWidget.Gtk.Container+ContainerChild
            this.vbox2 = new Gtk.VBox();
            this.vbox2.Name = "vbox2";
            this.vbox2.Spacing = 6;
            // Container child vbox2.Gtk.Box+BoxChild
            this.action_scroll = new Gtk.ScrolledWindow();
            this.action_scroll.CanFocus = true;
            this.action_scroll.Name = "action_scroll";
            this.action_scroll.ShadowType = ((Gtk.ShadowType)(1));
            this.action_scroll.BorderWidth = ((uint)(5));
            this.vbox2.Add(this.action_scroll);
            Gtk.Box.BoxChild w1 = ((Gtk.Box.BoxChild)(this.vbox2[this.action_scroll]));
            w1.Position = 0;
            w1.Padding = ((uint)(5));
            // Container child vbox2.Gtk.Box+BoxChild
            this.hbox1 = new Gtk.HBox();
            this.hbox1.Name = "hbox1";
            this.hbox1.Spacing = 6;
            // Container child hbox1.Gtk.Box+BoxChild
            this.help_icn = new Gtk.Image();
            this.help_icn.Name = "help_icn";
            this.help_icn.Yalign = 0F;
            this.hbox1.Add(this.help_icn);
            Gtk.Box.BoxChild w2 = ((Gtk.Box.BoxChild)(this.hbox1[this.help_icn]));
            w2.Position = 0;
            w2.Expand = false;
            // Container child hbox1.Gtk.Box+BoxChild
            this.help_lbl = new Gtk.Label();
            this.help_lbl.Name = "help_lbl";
            this.help_lbl.Ypad = 1;
            this.help_lbl.Xalign = 0F;
            this.help_lbl.LabelProp = Mono.Unix.Catalog.GetString("To edit a shortcut key, click on the corresponding row and type a new accelerator, or press backspace to clear.\n");
            this.help_lbl.Wrap = true;
            this.hbox1.Add(this.help_lbl);
            Gtk.Box.BoxChild w3 = ((Gtk.Box.BoxChild)(this.hbox1[this.help_lbl]));
            w3.Position = 1;
            this.vbox2.Add(this.hbox1);
            Gtk.Box.BoxChild w4 = ((Gtk.Box.BoxChild)(this.vbox2[this.hbox1]));
            w4.Position = 1;
            w4.Expand = false;
            w4.Fill = false;
            w4.Padding = ((uint)(5));
            this.Add(this.vbox2);
            if ((this.Child != null)) {
                this.Child.ShowAll();
            }
            this.Show();
        }
    }
}
