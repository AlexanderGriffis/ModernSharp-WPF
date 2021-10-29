using ModernSharp_Base.ViewModels;
using ModernSharp_Modules.Application;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernSharp_Base.Controls {
    /// <summary>
    /// Interaction logic for ShortcutControl.xaml
    /// </summary>
    public partial class ShortcutControl : UserControl {
        private Key[] Modifiers = { Key.LeftAlt, Key.RightAlt, Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl };
        public ShortcutControlViewModel ViewModel { get; set; }

        public ShortcutControl(AppShortcut shortcut) {
            InitializeComponent();
            ViewModel = new ShortcutControlViewModel();
            DataContext = ViewModel;
            ViewModel.Shortcut = shortcut;
            
            string keyValue;
            if (AppManager.SettingRead("KeysAccessKey", ViewModel.Shortcut.Name, out keyValue))
                ViewModel.Shortcut.AccessKey = (Key)Enum.Parse(typeof(Key), keyValue);

            if (AppManager.SettingRead("KeysModifierKey", ViewModel.Shortcut.Name, out keyValue))
                ViewModel.Shortcut.ModKeys = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), keyValue);
        }

        private void AccessKeyButton_Click(object sender, RoutedEventArgs e) {
            ViewModel.AccessKeyIsChanging = true;
            ViewModel.ModifierKeyIsChanging = false;
        }

        private void ModifierKeyButton_Click(object sender, RoutedEventArgs e) {
            ViewModel.ModifierKeyIsChanging = true;
            ViewModel.AccessKeyIsChanging = false;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (ViewModel.AccessKeyIsChanging) {
                if ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                    || (e.Key >= Key.F && e.Key <= Key.F19))
                    ViewModel.UnsocialAccessKey = e.Key;
            }

            if (ViewModel.ModifierKeyIsChanging) {
                bool getModifier = false;
                foreach (Key k in Modifiers) {
                    switch (e.Key) {
                        case Key.LeftCtrl:
                        case Key.RightCtrl:
                            ViewModel.ModifierKey = ModifierKeys.Control;
                            getModifier = true;
                            break;
                        case Key.LeftShift:
                        case Key.RightShift:
                            ViewModel.ModifierKey = ModifierKeys.Shift;
                            getModifier = true;
                            break;
                    }
                    if (getModifier) break;
                }
            }

            ViewModel.AccessKeyIsChanging = false;
            ViewModel.ModifierKeyIsChanging = false;
            e.Handled = true;
        }

        private void ResetKeysButton_Click(object sender, RoutedEventArgs e) {
            ViewModel.AccessKeyIsChanging = false;
            ViewModel.ModifierKeyIsChanging = false;

            ViewModel.Shortcut.RestoreDefault();
            ViewModel.ModifierKey = ViewModel.ModifierKey;
            ViewModel.UnsocialAccessKey = ViewModel.UnsocialAccessKey;
        }
    }
}
