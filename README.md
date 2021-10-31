# ModernSharp-WPF

A modable and themeable MVVM based application base for WPF applications using .NET Framework 5.0. The theming base is using a Nuget package by [FirstFloor Software ModernUI.WPFCore](https://www.nuget.org/packages/ModernUI.WPFCore/).

ModernSharp-WPF comes pre-built with an application with plug and play modules API where modules can be loaded at runtime. This means that you can use the built in API to create new class libraries that will be loaded into your application to provide further extendability. Each module can register a single settings `ModuleViewModel` to represent the module as well as an `AppShortcut` with a rebindable shortcut key to access an action run by that module.


## Module API

The `ModernSharp-Modules` project is the project API that you can reference either at buildtime as a project reference or as a DLL reference. This project contains all of the tools and utilities for setting up your module to interact with the main application.

### AppManager (namespace: odernSharp_Modules.Application)

The `AppManager` is a static class that allows you to register your module into the main application. Yo ucan access the API under the following namespace: `ModernSharp_Modules`. The following are a list of commands in the AppManager API that are available:

```C#
// Registers an XAML control as a Module into the menu interface: moduleName (name of your module), groupKey (menu group label/category), XAML uri link.
// Examples of URI links: new URI("/ModuleName;component/FolderName/ControlName.xaml", UriKind.RelativeOrAbsolute);
AppManager.RegisterModule(string moduleName, string groupKey, Uri source);

// Registers a shortcut key with a action/uri for your module: shortcutName (as shown in settings), descrion (as shown in settings), accessKey(main key, e.g. Z, X, P, etc.), modiferKey (ctrl, shift, etc.).
// Examples of Command: new Action(() => { this.SomeMethod(); });
// Examples of URI links: new URI("/ModuleName;component/FolderName/ControlName.xaml", UriKind.RelativeOrAbsolute);
AppManager.RegisterShortcut(shortcutName, description, accessKey, modiferKey, action);
AppManager.RegisterShortcut(shortcutName, description, accessKey, modiferKey, uri);

// Registers a control that changes your modules settings that can be accessed in the settings menu: settingsName (as shown in settings), uri (control to register).
// Examples of URI links: new URI("/ModuleName;component/FolderName/ControlName.xaml", UriKind.RelativeOrAbsolute);
AppManager.RegisterSettings(settingsName, uri);

// Registers a new section for tracking a setting.
AppManager.SettingsExists(section, propertyName);
// Writes a new value to the targeted settings section & property (propertyName).
AppManager.SettingsWrite(section, propertyName, <T> value);
// Reads a value loaded into the settings from the targeted section & property (propertyName).
AppManager.SettingsRead(section, propertyName, out <T> value);

// Saves/Loads all registered settings (settings are autoloaded upon application startup, but need to be read by modules when modules startup).
AppManager.SaveSettings();
AppManager.LoadSettings();
```

### ModuleViewModel (namespace: ModernSharp_Modules.ViewModels)

The `ModuleViewModel` is the model that represents your module in the main application and is not to be confused with/used as an MVVM control view model. This class will be instantiated/created when your module is loaded and contains two events for when your module is loaded and disposed:
```C#
event ModuleLoadedEventHandler OnModuleLoaded();
event ModuleUnloadedEventHandler OnModuleUnloaded();

// Example, register event methods:
OnModuleLoaded += SomeLoadMethod;
OnModuleUnloaded += SomeUnloadMethod;

// Example, event methods:
void SomeLoadMethod() { ... }
void SomeUnloadMethod() { ... }
```
You can create a new module project as a `WPF class library` and then create a new class `MyModuleClass.cs` and inherit the `ModuleViewModel` as the parent. When you're done writing your module you can build it as a DLL and save your module into the module directory of the main application (run the application and it'll generate the module folder, add your module and then rerun the application to load your module).

Here is an example module as a WPF class library: (this is the sample module included in the application).
```C#
using System;
using ModernSharp_Modules.Modules;
using ModernSharp_Modules.Application;

namespace SampleModule {
    public class SampleModuleViewModel : ModuleViewModel {
        [ModuleName("Sample Module")]
        public SampleModuleViewModel() {
            Uri source = new Uri("/SampleModule;component/Controls/SampleModule.xaml", UriKind.RelativeOrAbsolute);
            AppManager.RegisterModule("Sample Module", "SOFTWARE", source);
            AppManager.RegisterShortcut("Sample", "Basic sample module", System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Control, source);
        }
    }
    
    public class ModuleName : ModuleAttribute {
        public string Name { get; set; }
        public ModuleName(string name) => Name = name;
    }
}
```
This class will register your module into the main application menu and with a keybinding shortcut that can be rebound in the settings. When the keybinding is pressed the `SampleModule.xamml` control from the sample module will be targeted and navigated to. You can also manually navigate to the sample module within the menu.

There is also a `ModuleAttribute` class that can be inherited to create attributes that your main application can readout from modules when they're loaded. In the `MainWindow.xaml.cs` file within the MainWindow constructor is where modules are loaded:

```C#
#region Initialize Module System
loader = new ModuleLoader<ModuleViewModel>(AppManager.ModuleDirectory, false);
loader.ImportModules(false);

foreach (KeyValuePair<string, ModuleCore> pair in loader.ModuleCores)
    pair.Value.LoadAllModules();

LoadModules();
LoadShortcuts();
this.ContentSource = MenuLinkGroups.First().Links.First().Source;
#endregion
```
Here within the `foreach(...)` loop on each ModuleCore AFTER ModuleCore.LoadAllModules() is called you can loop through the ModuleCore's `LoadedModules` List of loaded `ModuleViewModel` objects and call `ModuleCore.GetModuleAttributes(ModuleViewModel)` to get a loaded modules custom module attributes.

### Utilities

The `ModernSharp-Modules` API contains several useful independant utilities that can be used by your module:

`NotifyableObject` [Handles the INotify interface and provides subscribable events PropertyChanged and PropertyChanging](https://github.com/FatalSleep/NotifyableObject).

`XmlManager` [Basic XML file reader/writer](https://github.com/FatalSleep/XmlSettings).

`Unsocial<T>` [A class that can only take one Enum value where T is the enum reference](https://github.com/FatalSleep/Unsocial).

`MemoryBuffer` [Simple array buffer that you can read/write data to](https://github.com/FatalSleep/MemoryBuffer) (useful for networking/communication between processes, etc.).

`InterProcComm` [Class that creates a communication channel between two processes with the same executing assembly GUID](https://github.com/FatalSleep/One-Way-IPC) (replace GUID with any unique GUID between processes you'd like to communicate between).

`AsyncServer<T>` [A scalable async TCP server](https://github.com/FatalSleep/AsyncAwait-Server).
