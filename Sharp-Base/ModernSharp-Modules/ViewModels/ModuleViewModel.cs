using ModernSharp_Modules.ViewModels;
using System;

namespace ModernSharp_Modules.Modules {
    public abstract class ModuleViewModel : NotifyableObject, IDisposable {
        #region Event Definitions
        public delegate void ModuleLoadedEventHandler();
        public delegate void ModuleUnloadedEventHandler();
        public event ModuleLoadedEventHandler OnModuleLoaded;
        public event ModuleUnloadedEventHandler OnModuleUnloaded;
        #endregion

        #region Destructor
        ~ModuleViewModel() =>
            Dispose(false);
        #endregion

        #region Dispose Pattern
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposeManaged) { }
        #endregion

        #region Module Assembler Events
        public virtual void OnAssemble() =>
            OnModuleLoaded?.Invoke();

        public virtual void OnDisassemble() =>
            OnModuleUnloaded?.Invoke();
        #endregion
    }
}
