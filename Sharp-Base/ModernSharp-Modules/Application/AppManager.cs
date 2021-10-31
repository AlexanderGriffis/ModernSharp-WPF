using FirstFloor.ModernUI.Windows.Navigation;
using ModernSharp_Modules.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ModernSharp_Modules.Application {
    /// <summary>Information about the main application.</summary>
    public static class AppManager {
        /// <summary>Returns the main window's view model.</summary>
        public static object AppViewModel { get; internal set; }
        /// <summary>Gets the application directory.</summary>
        public static string AppDirectory { get; internal set; }
        /// <summary>Gets the settings directory in the application directory.</summary>
        public static string SettingsDirectory { get; internal set; }
        /// <summary>Gets the module directory in the application directory.</summary>
        public static string ModuleDirectory { get; internal set; }
        
        #region Internal Properties
        internal static List<AppShortcut> ShortcutContainers { get; set; } = new List<AppShortcut>();
        internal static List<AppSettings> SettingsContainers { get; set; } = new List<AppSettings>();
        internal static Dictionary<string, Tuple<string, Uri>> ModuleContainers { get; set; } = new Dictionary<string, Tuple<string, Uri>>();
        internal static XmlManager Settings { get; set; } = new XmlManager(@"Settings\", "AppSettings", "Settings", true);
        #endregion

        #region Public Application Interface
        /// <summary>Adds a new module to the interface</summary>
        /// <param name="moduleName"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool RegisterModule(string moduleName, string groupKey, Uri source) {
            bool moduleExists = ModuleContainers.ContainsKey(moduleName);
            if (!moduleExists)
                ModuleContainers.Add(moduleName, new Tuple<string, Uri>(groupKey, source));
            return moduleExists;
        }

        /// <summary>Registers a new shortcut with the specified key and modifiers if one doesn't already exist.</summary>
        /// <param name="shortcutName">Name of the shortcut.</param>
        /// <param name="accessKey">Main key for the shortcut.</param>
        /// <param name="modKeys">Modifier keys for the shortcut.</param>
        /// <param name="command">Command to run when the shortcut is pressed</param>
        /// <returns>True if the shortcut could be created, else false.</returns>
        public static bool RegisterShortcut(string shortcutName, string description, Key accessKey, ModifierKeys modKeys, Action command) {
            if (shortcutName.Contains(' ')) throw new Exception("ERROR: SPACE IN SHORTCUT SETTINGS NAME: " + shortcutName.ToString());
            bool shortcutExists = ShortcutContainers.Any(x => (x.Name == shortcutName && x.AccessKey == accessKey && x.ModKeys == modKeys));

            if (!shortcutExists)
                ShortcutContainers.Add(new AppShortcut(shortcutName, description, accessKey, modKeys, command));

            return shortcutExists;
        }

        /// <summary>Registers a new shortcut with the specified key and modifiers if one doesn't already exist.</summary>
        /// <param name="shortcutName">Name of the shortcut.</param>
        /// <param name="accessKey">Main key for the shortcut.</param>
        /// <param name="modKeys">Modifier keys for the shortcut.</param>
        /// <param name="source">Source URI to navigate to.</param>
        /// <returns>True if the shortcut could be created, else false.</returns>
        public static bool RegisterShortcut(string shortcutName, string description, Key accessKey, ModifierKeys modKeys, Uri source) {
            if (shortcutName.Contains(' ')) throw new Exception("ERROR: SPACE IN SHORTCUT SETTINGS NAME: " + shortcutName.ToString());
            bool shortcutExists = ShortcutContainers.Any(x => (x.Name == shortcutName && x.AccessKey == accessKey && x.ModKeys == modKeys));

            if (!shortcutExists)
                ShortcutContainers.Add(new AppShortcut(shortcutName, description, accessKey, modKeys, source));

            return shortcutExists;
        }

        /// <summary>Registers a new settings user control for modules.</summary>
        /// <param name="settingsName">Name of the settings module.</param>
        /// <param name="control">UserControl for the settings.</param>
        /// <returns></returns>
        public static bool RegisterSettings(string settingsName, Uri uri) {
            if (settingsName.Contains(' ')) throw new Exception("ERROR: SPACE IN SETTINGS NAME: " + settingsName.ToString());
            bool settingsExists = SettingsContainers.Any(x => (x.SettingsName == settingsName || x.ControlPath == uri));

            if (!settingsExists)
                SettingsContainers.Add(new AppSettings(settingsName, uri));

            return settingsExists;
        }

        /// <summary>Checks if a particular property in a section exists.</summary>
        /// <param name="section">Name of the section to check for.</param>
        /// <param name="propertyName">Name of the proeprty to check for.</param>
        /// <returns>True if the property in section exists, else false.</returns>
        public static bool SettingExists(string section, string propertyName) =>
            Settings.Exists(section, propertyName);

        /// <summary>Write a value to a property in a section.</summary>
        /// <typeparam name="T">Valid types: Bool, Int32, Single, Double or String.</typeparam>
        /// <param name="section">Section name to write the new property to.</param>
        /// <param name="propertyName">Name of the property to create.</param>
        /// <param name="value">Value to give the property.</param>
        /// <exception cref="NotImplementedException">Throw on invalid type.</exception>
        public static void SettingWrite<T>(string section, string propertyName, T value) where T : IComparable, IConvertible, IEquatable<T>, IComparable<T> =>
            Settings.Write(section, propertyName, value);

        /// <summary></summary>
        /// <typeparam name="T">Valid types: Bool, Int32, Single, Double or String.</typeparam>
        /// <param name="section">Section name to read the property from.</param>
        /// <param name="propertyName">Name of the property to read.</param>
        /// <param name="value">Out reference to write the value to.</param>
        /// <exception cref="NotImplementedException">Throw on invalid type.</exception>
        /// <returns>True if property can be read, else false.</returns>
        public static bool SettingRead<T>(string section, string propertyName, out T value) where T : IComparable, IConvertible, IEquatable<T>, IComparable<T> =>
            Settings.Read(section, propertyName, out value);

        /// <summary>Save settings to file.</summary>
        public static void SaveSettings() =>
            Settings.Serialize();

        /// <summary>Load settings from file.</summary>
        public static void LoadSettings() =>
            Settings.Deserialize();
        #endregion
    }
}
