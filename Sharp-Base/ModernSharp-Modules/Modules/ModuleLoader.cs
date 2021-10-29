using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ModernSharp_Modules.Modules {
    public class ModuleLoader<T> where T : ModuleCore {
        #region Properties
        public Dictionary<string, T> ModuleCores { get; internal set; }

        public HashSet<string> BlackListedModules { get; internal set; }

        public string ImportDirectory { get; internal set; }

        public bool RecursiveImport { get; internal set; }
        #endregion

        #region Constructor
        public ModuleLoader(string directory, bool recursiveImport) {
            ImportDirectory = directory;
            RecursiveImport = recursiveImport;

            ModuleCores = new Dictionary<string, T>();
            BlackListedModules = new HashSet<string>();
        }
        #endregion

        #region Module Loader
        private bool ImportDirectoryIsValid() {
            try {
                bool DirectoryExists = Directory.Exists(ImportDirectory);
                Path.GetFullPath(ImportDirectory);
                return DirectoryExists;
            } catch (InvalidDirectoryException) {
                return false;
            }
        }

        public void ImportModules(bool applyBlackList) {
            if (!ImportDirectoryIsValid())
                return;

            foreach (string file in Directory.GetFiles(ImportDirectory, "*.dll", (RecursiveImport) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)) {
                string assemblyName = AssemblyName.GetAssemblyName(file).FullName;

                if (assemblyName == Assembly.GetExecutingAssembly().FullName)
                    continue;
                if (applyBlackList && BlackListedModules.Contains(assemblyName))
                    continue;

                if (!ModuleCores.ContainsKey(assemblyName)) {
                    Assembly importedAssembly = Assembly.LoadFrom(file);
                    List<Type> verifiedModules = new List<Type>();

                    foreach (Type module in importedAssembly.GetTypes())
                        if (VerifyModule(module))
                            verifiedModules.Add(module);

                    T core = (T)Activator.CreateInstance(typeof(T), new object[] { assemblyName, Path.GetFullPath(file), importedAssembly, verifiedModules.AsReadOnly() });
                    ModuleCores.Add(assemblyName, core);
                }
            }
        }

        protected virtual bool VerifyModule(Type module) =>
            module.IsClass && module.BaseType == typeof(T);

        public void BlacklistModule(T module) =>
            BlackListedModules.Add(module.AssemblyName);

        public void WhitelistModule(T module) =>
            BlackListedModules.RemoveWhere(x => x == module.AssemblyName);
        #endregion

        #region Internal Exceptions
        internal class InvalidDirectoryException : Exception {
            public InvalidDirectoryException() { }
            public InvalidDirectoryException(string Directory) : base("Directory: " + Directory) { }
        }
        #endregion
    }
}