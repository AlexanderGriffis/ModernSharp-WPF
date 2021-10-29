using ModernSharp_Modules.Application;
using System.Windows.Controls;

namespace ModernSharp_Base.Controls {
    /// <summary>
    /// Interaction logic for ShortcutSettings.xaml
    /// </summary>
    public partial class ShortcutSettings : UserControl {
        public ShortcutSettings() {
            InitializeComponent();

            foreach (AppShortcut sc in AppManager.ShortcutContainers) {
                ShortcutControl shortcut = new ShortcutControl(sc);
                RegisteredShortcutsPanel.Children.Add(shortcut);
            }
        }
    }
}
