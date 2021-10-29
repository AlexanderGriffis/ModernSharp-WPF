using System;
using System.Windows.Input;

namespace ModernSharp_Modules.Application {
    /// <summary>Container to represent an application shortcut.</summary>
    public class AppShortcut {
        #region Public Properties
        /// <summary>Name of the shortcut.</summary>
        public string Name { get; set; }
        /// <summary>Description of user functionality.</summary>
        public string Description { get; set; }
        /// <summary>Main key (non-modifier) for the shortcut.</summary>
        public Key AccessKey { get; set; }
        /// <summary>Set of modifier keys for the shortcut.</summary>
        public ModifierKeys ModKeys { get; set; }
        /// <summary>Command (method) to execute when this shortcut is pressed.</summary>
        public Action Command { get; set; }
        #endregion

        #region Private Fields
        /// <summary>Default key for restoring default shortcut state after rebinding.</summary>
        private Key defaultKey;
        /// <summary>Default modifiers for restoring default shortcut state after rebinding.</summary>
        private ModifierKeys defaultModifiers;
        #endregion

        #region Constructor
        /// <summary>Constructs a new application keyboard shortcut.</summary>
        /// <param name="shortcutName">Name of the shortcut.</param>
        /// <param name="accessKey">Main key (non-modifier) for the shortcut.</param>
        /// <param name="modKeys">Set of modifier keys for the shortcut.</param>
        /// <param name="command">Command (method) to execute when this shortcut is pressed.</param>
        public AppShortcut(string shortcutName, string description, Key accessKey, ModifierKeys modKeys, Action command) {
            Name = shortcutName;
            Description = description;
            AccessKey = accessKey;
            ModKeys = modKeys;
            Command = command;

            defaultKey = AccessKey;
            defaultModifiers = modKeys;
        }
        #endregion

        #region Shortcut Interface
        /// <summary>Restores the default state of the application shortcut.</summary>
        public void RestoreDefault() {
            AccessKey = defaultKey;
            ModKeys = defaultModifiers;
        }

        /// <summary>Invokes the shortcut's command if the pass access and modifier keys were passed.</summary>
        public void Invoke() =>
            Command?.Invoke();
        #endregion
    }
}
