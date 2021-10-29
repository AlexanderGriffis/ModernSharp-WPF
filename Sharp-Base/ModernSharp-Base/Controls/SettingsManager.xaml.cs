using FirstFloor.ModernUI.Presentation;
using ModernSharp_Modules.Application;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ModernSharp_Base.Controls {
    /// <summary>
    /// Interaction logic for SettingsManager.xaml
    /// </summary>
    public partial class SettingsManager : UserControl {
        public SettingsManager() {
            InitializeComponent();
            
            /// Load settings into the Link interface of MUI.
            List<AppSettings> settings = AppManager.SettingsContainers;
            foreach (AppSettings set in settings) {
                Link lnk = new Link();
                lnk.DisplayName = set.SettingsName;
                lnk.Source = set.ControlPath;
                SettingsLinks.Links.Add(lnk);
            }

            // Load up the very first registered settings page.
            if (SettingsLinks.Links.First() != null)
                SettingsLinks.SelectedSource = SettingsLinks.Links.First().Source;
        }
    }
}
