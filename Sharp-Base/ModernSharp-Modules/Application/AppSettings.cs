using System;

namespace ModernSharp_Modules.Application {
    /// <summary>Container to represent an application setting.</summary>
    public class AppSettings {
        #region Public Properties
        /// <summary>Name of the shortcut.</summary>
        public string SettingsName { get; set; }
        /// <summary></summary>
        public Uri ControlPath { get; set; }
        #endregion
        /// <summary>Constructs a new settings control.</summary>
        /// <param name="settingsName">Name of the settings.</param>
        /// <param name="control">UserControl to load.</param>
        public AppSettings(string settingsName, Uri uri) {
            SettingsName = settingsName;
            ControlPath = uri;
        }
    }
}
