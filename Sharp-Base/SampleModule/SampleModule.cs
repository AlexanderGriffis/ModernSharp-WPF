using System;
using ModernSharp_Modules.Modules;
using ModernSharp_Modules.Application;
using ModernSharp_Modules.Processes;
using ModernSharp_Modules.Utilities;
using ModernSharp_Modules.ViewModels;

namespace SampleModule {
    public class SampleModuleViewModel : ModuleViewModel {
        public SampleModuleViewModel() {
            Uri source = new Uri("/SampleModule;component/Controls/SampleModule.xaml", UriKind.RelativeOrAbsolute);
            AppManager.RegisterModule("Sample Module", "SOFTWARE", source);
            AppManager.RegisterShortcut("Sample", "Basic sample module", System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Control, source);
        }
    }
}
