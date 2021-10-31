using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace ModernSharp_Modules.Modules {
    public class ModuleCore {
        #region Properties
        public string AssemblyDirectory { get; internal set; }

        public string AssemblyName { get; internal set; }

        public Assembly LoadedAssembly { get; internal set; }

        public ReadOnlyCollection<Type> VerifiedModules { get; internal set; }

        public List<ModuleViewModel> LoadedModules { get; internal set; }
        #endregion

        #region Constructor
        public ModuleCore(string assemblyName, string assemblyDirectory, Assembly loadedAssembly, ReadOnlyCollection<Type> verifiedModules) {
            AssemblyName = assemblyName;
            AssemblyDirectory = assemblyDirectory;
            LoadedAssembly = loadedAssembly;
            VerifiedModules = verifiedModules;
            LoadedModules = new List<ModuleViewModel>();
        }
        #endregion

        #region Load Modules
        private void LoadModule(Type module) {
            if (!module.IsSubclassOf(typeof(ModuleViewModel)) || !VerifiedModules.Contains(module))
                return;

            ModuleViewModel moduleInstance = (ModuleViewModel)Activator.CreateInstance(module);
            LoadedModules.Add(moduleInstance);
            moduleInstance.OnAssemble();
        }

        public virtual void LoadAllModules() {
            foreach (Type module in VerifiedModules)
                LoadModule(module);
        }
        #endregion

        #region Find Attributes
        public List<ModuleAttribute> GetModuleAttributes(ModuleViewModel module) {
            List<ModuleAttribute> Attributes = new List<ModuleAttribute>();

            foreach (object attr in module.GetType().GetCustomAttributes(false))
                if (attr.GetType().BaseType == typeof(ModuleAttribute))
                    Attributes.Add((ModuleAttribute)attr);

            return Attributes;
        }
        #endregion
    }
}