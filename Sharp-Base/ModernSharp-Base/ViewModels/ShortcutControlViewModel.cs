using ModernSharp_Modules.Application;
using ModernSharp_Modules.ViewModels;
using System.ComponentModel;
using System.Windows.Input;

namespace ModernSharp_Base.ViewModels {
    public class ShortcutControlViewModel : NotifyableObject {
        private bool accessKeyIsChanging = false;
        private bool modifierKeyIsChanging = false;
        public bool AccessKeyIsChanging { get => accessKeyIsChanging; set { accessKeyIsChanging = value; OnPropertyChanged(); OnPropertyChanged("AccessCurrent"); } }
        public bool ModifierKeyIsChanging { get => modifierKeyIsChanging; set { modifierKeyIsChanging = value; OnPropertyChanged(); OnPropertyChanged("ModifierCurrent"); } }
        public AppShortcut Shortcut { get; set; }

        public string ShortcutName { get => Shortcut.Name; }
        public string ShortcutDescription { get => Shortcut.Description; }

        public string AccessCurrent { get => (accessKeyIsChanging) ? "<>" : ((Key)Shortcut.AccessKey).ToString(); }
        public string ModifierCurrent { get => (modifierKeyIsChanging) ? "<>" : Shortcut.ModKeys.ToString(); }

        public Key AccessKey { get => (Key)UnsocialAccessKey; }

        public Key UnsocialAccessKey {
            get => Shortcut.AccessKey;
            set {
                Shortcut.AccessKey = value;
                OnPropertyChanged();
                OnPropertyChanged("AccessKey");
                OnPropertyChanged("AccessCurrent");
            }
        }

        public ModifierKeys ModifierKey {
            get => Shortcut.ModKeys;
            set {
                Shortcut.ModKeys = value;
                OnPropertyChanged();
                OnPropertyChanged("ModifierCurrent");
            }
        }

        public ShortcutControlViewModel() =>
            PropertyChanged += Shortcut_Changed;

        void Shortcut_Changed(object sender, PropertyChangedEventArgs e) {
            AppManager.SettingWrite("KeysAccessKey", ShortcutName, ((Key)Shortcut.AccessKey).ToString());
            AppManager.SettingWrite("KeysModifierKey", ShortcutName, Shortcut.ModKeys.ToString());
            AppManager.SaveSettings();
        }
    }
}
