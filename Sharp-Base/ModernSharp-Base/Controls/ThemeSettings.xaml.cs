using ModernSharp_Base.ViewModels;
using ModernSharp_Modules.Application;
using System.Windows.Controls;

namespace ModernSharp_Base.Controls {
    public partial class ThemeSettings : UserControl {
        public MainWindowViewModel ViewModel { get; set; }

        public ThemeSettings() {
            InitializeComponent();
            ViewModel = (MainWindowViewModel) AppManager.AppViewModel;
            DataContext = ViewModel;
        }
    }
}
