using ModernSharp_Modules.Processes;
using System;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace ModernSharp_Base {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        #region Public Properties
        /// <summary>Gets the Inter-Process Communicaiton Interface.</summary>
        public InterProcComm IPC { get; private set; }
        #endregion

        #region Constructor
        /// <summary>Initializes the application.</summary>
        public App() {
            IPC = new InterProcComm(true);
            IPC.MessageReceived += ReceivedMessage;
            InitializeComponent();
        }

        /// <summary>Startup event.</summary>
        private void Application_Startup(object sender, StartupEventArgs e) { }
        #endregion

        #region Main Method
        /// <summary>Replacement for the implicit App main method.</summary>
        /// <param name="args">Array of command-line parameters that were passed.</param>
        [STAThread]
        public static void MainSingleton(params string[] args) {
            string appGUID = Assembly.GetExecutingAssembly().GetType().GUID.ToString();

            MemoryBuffer memory = new MemoryBuffer();

            foreach (string str in args)
                memory.Write(str);

            using (Mutex mut = new Mutex(false, appGUID)) {
                if (!mut.WaitOne(0, false)) {
                    InterProcComm.Post(memory);
                    return;
                }

                GC.Collect();
                App singleton = new App();
                singleton.Run();

                if (args.Length > 0)
                    singleton.CommandLine(memory);
            }
        }
        #endregion

        #region Commandline Interface
        /// <summary>Executes the command-line interface with the contents of the received MemoryBuffer.</summary>
        /// <param name="buffer">MemoryBuffer with the command-line parameters.</param>
        protected virtual void ReceivedMessage(MemoryBuffer buffer) {
            CommandLine(buffer);
        }

        /// <summary>Processes the command-line arguments within the received MemoryBuffer.</summary>
        /// <param name="buffer">MemoryBuffer with the command-line parameters.</param>
        protected virtual void CommandLine(MemoryBuffer buffer) {
            throw new NotImplementedException("Handling command-line arguments not implemented.");
        }
        #endregion
    }
}
