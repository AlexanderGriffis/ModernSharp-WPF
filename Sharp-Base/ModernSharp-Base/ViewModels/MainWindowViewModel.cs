using FirstFloor.ModernUI.Presentation;
using ModernSharp_Modules.Application;
using ModernSharp_Modules.Modules;
using ModernSharp_Modules.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernSharp_Base.ViewModels {
    public class MainWindowViewModel : NotifyableObject {
        private readonly ModuleLoader<ModuleCore> loader;
        public Storyboard BackgroundAnimation { get; set; }

        public MainWindowViewModel(MainWindow window) {
            #region Set Application Properties
            AppManager.AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
            AppManager.SettingsDirectory = Path.Combine(AppManager.AppDirectory, "Settings");
            AppManager.ModuleDirectory = Path.Combine(AppManager.AppDirectory, "Modules");
            AppManager.AppViewModel = this;

            if (!Directory.Exists(AppManager.SettingsDirectory))
                Directory.CreateDirectory(AppManager.SettingsDirectory);
            if (!Directory.Exists(AppManager.ModuleDirectory))
                Directory.CreateDirectory(AppManager.ModuleDirectory);

            Themes.Add(new Link { DisplayName = "Light", Source = new Uri("/Themes/LightTheme.xaml", UriKind.Relative) });
            Themes.Add(new Link { DisplayName = "Dark", Source = new Uri("/Themes/DarkTheme.xaml", UriKind.Relative) });
            SelectedTheme = Themes.First();
            SelectedFontSize = FontSizes.First();

            SetFontSize();
            LoadSettings();
            LoadShortcuts();

            PropertyChanged += MainWindow_AppearanceChanged;
            PropertyChanged += MainWindow_KeybindingsChanged;
            UpdateVisualTheme();

            #region Initialize Module System
            loader = new ModuleLoader<ModuleCore>(AppManager.ModuleDirectory, false);
            loader.ImportModules(false);

            foreach (KeyValuePair<string, ModuleCore> pair in loader.ModuleCores)
                pair.Value.LoadAllModules();
            #endregion
            #endregion
        }

        #region Application Events
        /// <summary>When a property changes in the appearance manager, begin background transition.</summary>
        private void MainWindow_AppearanceChanged(object sender, PropertyChangedEventArgs e) {
            // start background animation if theme has changed
            if (e.PropertyName == "ThemeSource" && this.BackgroundAnimation != null)
                this.BackgroundAnimation.Begin();

            UpdateVisualTheme();
        }

        private void MainWindow_KeybindingsChanged(object sender, PropertyChangedEventArgs e) =>
            AppManager.SaveSettings();
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Theme Handling
        private bool isNotLoadingDefaultTheme = false;
        public double[] FontSizes { get => new double[] { 12D, 14D, 18D, 24D }; }
        private double fontSize;
        public double SelectedFontSize {
            get { return fontSize; }
            set { fontSize = value; OnPropertyChanged(); }
        }

        /// <summary>List of colors for the theme.</summary>
        public Color[] AccentColors {
            get => new Color[] {
                Color.FromRgb(0x33, 0x99, 0xff), // blue
                Color.FromRgb(0x00, 0xab, 0xa9), // teal
                Color.FromRgb(0x33, 0x99, 0x33), // green
                Color.FromRgb(0x8c, 0xbf, 0x26), // lime
                Color.FromRgb(0xf0, 0x96, 0x09), // orange
                Color.FromRgb(0xff, 0x45, 0x00), // orange red
                Color.FromRgb(0xe5, 0x14, 0x00), // red
                Color.FromRgb(0xff, 0x00, 0x97), // magenta
                Color.FromRgb(0xa2, 0x00, 0xff), // purple
            };
        }
        private Color selectedAccent;
        public Color SelectedAccent {
            get { return selectedAccent; }
            set { selectedAccent = value; OnPropertyChanged(); }
        }

        private Link selectedTheme;
        public Link SelectedTheme {
            get { return selectedTheme; }
            set { selectedTheme = value; OnPropertyChanged(); OnPropertyChanged("ThemeSource"); }
        }

        /// <summary>Collection of active selectable themes.</summary>
        public LinkCollection Themes { get; } = new LinkCollection();
        
        /// <summary>Set application font size.</summary>
        private void SetFontSize() {
            Application.Current.Resources["DefaultFontSize"] = SelectedFontSize;
            Application.Current.Resources["FixedFontSize"] = SelectedFontSize;
        }

        /// <summary>Set the new application theme.</summary>
        /// <param name="source">Theme source Uri Link.</param>
        private static void SetThemeSource(Uri source) {
            if (source == null)
                throw new ArgumentNullException("Source");

            var oldThemeDict = GetThemeDictionary();
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var themeDict = new ResourceDictionary { Source = source };

            dictionaries.Add(themeDict);
            if (oldThemeDict != null)
                dictionaries.Remove(oldThemeDict);
        }

        /// <summary>Returns the theme's source Uri Link.</summary>
        private static Uri GetThemeSource() => GetThemeDictionary()?.Source;

        /// <summary>Gets or sets the current theme source.</summary>
        public Uri ThemeSource {
            get { return GetThemeSource(); }
            set { SetThemeSource(value); }
        }

        /// <summary>Sync the new selected accent color to the live theme.</summary>
        private void SyncAccent() {
            Application.Current.Resources["AccentColor"] = SelectedAccent;
            Application.Current.Resources["Accent"] = new SolidColorBrush(SelectedAccent);
            ThemeSource = SelectedTheme.Source;
        }

        /// <summary>Reads out the theme from the application ResourceDictionary.</summary>
        private static ResourceDictionary GetThemeDictionary() {
            // determine the current theme by looking at the app resources and return the first dictionary having the resource key 'WindowBackground' defined.
            return (from dict in Application.Current.Resources.MergedDictionaries
                    where dict.Contains("WindowBackground")
                    select dict).FirstOrDefault();
        }

        /// <summary>Load theme settings from file.</summary>
        private void LoadSettings() {
            AppManager.LoadSettings();
            bool hasAccent = AppManager.SettingRead("Appearance", "Accent", out string accent);
            bool hasFontSize = AppManager.SettingRead("Appearance", "FontSize", out string fontSize);
            bool hasTheme = AppManager.SettingRead("Appearance", "Theme", out string theme);

            if (hasTheme)
                SelectedTheme = Themes.Where(x => x.DisplayName == theme).FirstOrDefault();

            if (hasFontSize) SelectedFontSize = double.Parse(fontSize);

            if (hasAccent)
                try { SelectedAccent = (Color)ColorConverter.ConvertFromString(accent); } catch (Exception) { SelectedAccent = AccentColors.FirstOrDefault(); }
        }

        private void LoadShortcuts() {
            foreach (AppShortcut sc in AppManager.ShortcutContainers) {
                if (AppManager.SettingRead("KeysAccessKey", sc.Name, out string keyValue))
                    sc.AccessKey = (Key)Enum.Parse(typeof(Key), keyValue);

                if (AppManager.SettingRead("KeysModifierKey", sc.Name, out keyValue))
                    sc.ModKeys = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), keyValue);
            }
        }

        private void UpdateVisualTheme() {
            ThemeSource = SelectedTheme.Source;
            if (!AccentColors.Contains(SelectedAccent))
                SelectedAccent = AccentColors.FirstOrDefault();

            SyncAccent();
            SetFontSize();

            if (isNotLoadingDefaultTheme) {
                AppManager.SettingWrite("Appearance", "Accent", SelectedAccent.ToString());
                AppManager.SettingWrite("Appearance", "FontSize", SelectedFontSize);
                AppManager.SettingWrite("Appearance", "Theme", SelectedTheme.DisplayName);
                AppManager.SaveSettings();
            } else {
                isNotLoadingDefaultTheme = true;
            }
        }
        #endregion
    }
}
