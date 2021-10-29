# ModernSharp-WPF

A modable and themeable application base for WPF applications using .NET Framework 5.0. The theming base is using a Nuget package by [FirstFloor Software ModernUI.WPFCore](https://www.nuget.org/packages/ModernUI.WPFCore/).

ModernSharp-WPF comes pre-built with an application with plug and play modules API where modules can be loaded at runtime. This means that you can use the built in API to create new class libraries that will be loaded into your application to provide further extendability. Each module can register a single settings `ModuleViewModel` to represent the module as well as an `AppShortcut` with a rebindable shortcut key to access an action run by that module.
