using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using FirstFloor.ModernUI.Presentation;
using ModernSharp_Base.ViewModels;
using ModernSharp_Modules.Application;
using ModernSharp_Modules.Modules;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernSharp_Base {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow {
        private readonly ModuleLoader<ModuleViewModel> loader;
        public MainWindowViewModel ViewModel { get; private set; }

        public MainWindow() {
            InitializeComponent();

            #region Register Application Events
            // Register shortcut key combination checking event.
            PreviewKeyDown += MainWindow_PreviewKeyDown;
            #endregion

            #region Register Application Settings
            // Add a modifable shortcut key to the settings.
            AppManager.RegisterShortcut("OpenSettings", "quick-nav to settings/options.", Key.Q, ModifierKeys.Control,
                NewNavigateLinkAction(new Uri("/Controls/SettingsManager.xaml", UriKind.Relative)));

            // Register the appearance settings module.
            // If the appearance changes register the MainWindow event to transition the background theme color.
            AppManager.RegisterSettings("Themes", new Uri("/Controls/ThemeSettings.xaml", UriKind.Relative));

            // Register the shortcuts settings module.
            AppManager.RegisterSettings("Keybindings", new Uri("/Controls/ShortcutSettings.xaml", UriKind.Relative));

            ViewModel = new MainWindowViewModel(this);
            DataContext = ViewModel;
            #endregion

            #region Initialize Module System
            loader = new ModuleLoader<ModuleViewModel>(AppManager.ModuleDirectory, false);
            loader.ImportModules(false);

            foreach (KeyValuePair<string, ModuleCore> pair in loader.ModuleCores)
                pair.Value.LoadAllModules();

            LoadModules();
            LoadShortcuts();
            this.ContentSource = MenuLinkGroups.First().Links.First().Source;
            #endregion
        }

        private void LoadShortcuts() {
            foreach (AppShortcut sc in AppManager.ShortcutContainers) {
                if (AppManager.SettingRead("KeysAccessKey", sc.Name, out string keyValue))
                    sc.AccessKey = (Key)Enum.Parse(typeof(Key), keyValue);

                if (AppManager.SettingRead("KeysModifierKey", sc.Name, out keyValue))
                    sc.ModKeys = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), keyValue);

                if (sc.Command == null)
                    sc.Command = NewNavigateLinkAction(sc.Source);
            }
        }

        private void LoadModules() {
            foreach(var module in AppManager.ModuleContainers) {
                var group = MenuLinkGroups.FirstOrDefault((x) => x.GroupKey == module.Value.Item1);

                if (group == null || group == MenuLinkGroups.DefaultIfEmpty()) {
                    group = new LinkGroup() { DisplayName = module.Value.Item1, GroupKey = module.Value.Item1 };
                    MenuLinkGroups.Add(group);
                }

                var link = new Link() { DisplayName = module.Key, Source = module.Value.Item2 };
                group.Links.Add(link);
            }
        }

        /// <summary>Check for keyboard shortcut inputs on Preview KeyDown.</summary>
        public void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e) {
            List<AppShortcut> shortcuts = AppManager.ShortcutContainers.FindAll(x => (((Key)x.AccessKey) == e.Key && x.ModKeys == e.KeyboardDevice.Modifiers));

            shortcuts.ForEach(x => x?.Invoke());

            if (shortcuts.Count > 0)
                e.Handled = true;
        }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call System.Windows.FrameworkElement.ApplyTemplate().</summary>
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            // retrieve BackgroundAnimation storyboard
            var border = GetTemplateChild("WindowBorder") as Border;
            if (border != null) {
                this.ViewModel.BackgroundAnimation = border.Resources["BackgroundAnimation"] as Storyboard;

                if (this.ViewModel.BackgroundAnimation != null)
                    this.ViewModel.BackgroundAnimation.Begin();
            }
        }

        #region ModernWindow Link Fix
        Action NewNavigateLinkAction(Uri source) =>
            new Action(() => { LinkCommands.NavigateLink.Execute(source, FindChild<ModernFrame>(this, "ContentFrame")); });
        //B: NavigationCommands.GoToPage.Execute("/Controls/SettingsManager.xaml", target);
        //C: this.LinkNavigator.Navigate(new Uri("/Controls/SettingsManager.xaml", UriKind.Relative), target as FrameworkElement);

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++) {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null) {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                } else if (!string.IsNullOrEmpty(childName)) {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName) {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                } else {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        #endregion
    }
}
