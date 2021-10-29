using System;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ModernSharp_Modules.Processes {
    /// <summary>Simple interface for </summary>
    public class InterProcComm : IDisposable {
        #region Private Fields
        /// <summary>The GUID of the executing assembly--acts as a unique identifier.</summary>
        private static string appGUID = Assembly.GetEntryAssembly().GetCustomAttributes<GuidAttribute>().ToString();

        /// <summary>The cancellation source for the listening communication task.</summary>
        private CancellationTokenSource cancellationSource;

        /// <summary>The current executing task.</summary>
        private Task procCommTask;
        #endregion

        #region Event Handling
        /// <summary>Delegate for creating the MessageReceived event.</summary>
        /// <param name="message">The MemoryBuffer which contains the received message.</param>
        public delegate void MessageReceivedEventHandler(MemoryBuffer message);

        /// <summary>event that is executed when we've received a communication message.</summary>
        public event MessageReceivedEventHandler MessageReceived;
        #endregion

        #region Constructor
        /// <summary>Creates and starts the inter-process communication task.</summary>
        /// <param name="start"></param>
        public InterProcComm(bool start) {
            if(start)
                Start();
        }
        #endregion

        #region InterProcComm Interface
        /// <summary>Listen for any incoming client communications and handle them accordingly.</summary>
        private void Listen() {
            while(!cancellationSource.IsCancellationRequested) {
                using(NamedPipeServerStream pipeServer = new NamedPipeServerStream(appGUID, PipeDirection.In, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte)) {
                    try {
                        pipeServer.WaitForConnection();

                        MemoryBuffer mb = new MemoryBuffer();
                        int data = 0;
                        while((data = pipeServer.ReadByte()) != -1)
                            mb.Write((byte)data);
                        mb.Memory.Seek(0, SeekOrigin.Begin);
                        MessageReceived?.Invoke(mb);

                        pipeServer.Flush();
                        pipeServer.Disconnect();
                    } catch(Exception) { }
                }
            }

            procCommTask?.Dispose();
            cancellationSource?.Dispose();
            procCommTask = null;
            cancellationSource = null;
        }

        /// <summary>Post information to a listening InterProcComm process--or timeout after 50ms.</summary>
        /// <param name="memory"></param>
        /// <returns></returns>
        public static bool Post(MemoryBuffer memory) {
            using(NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", appGUID, PipeDirection.Out)) {
                pipeClient.Connect(50);

                if(pipeClient.IsConnected) {
                    byte[] buffer = memory.Memory.GetBuffer();
                    pipeClient.Write(buffer, 0, buffer.Length);
                    pipeClient.Flush();
                }

                bool posted = pipeClient.IsConnected;
                pipeClient.Dispose();
                return posted;
            }
        }

        /// <summary>Starts the inter-process communication task if it hasn't started yet.</summary>
        public void Start() {
            if(cancellationSource == null && procCommTask == null) {
                cancellationSource = new CancellationTokenSource();
                procCommTask = new Task(() => Listen(), cancellationSource.Token, TaskCreationOptions.LongRunning);
                procCommTask.Start();
            }
        }
        #endregion

        #region Dispose Pattern
        /// <summary>Cleans up the InterProcComm interface without disposing possibly cleaned up managed objects.</summary>
        ~InterProcComm() {
            Dispose(false);
        }

        /// <summary>Safely disposes the InterProcComm interface.</summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Dispose the InterProcComm interface.</summary>
        /// <param name="disposeManaged">Whether to dispose managed objects or not.</param>
        protected virtual void Dispose(bool disposeManaged) {
            cancellationSource?.Cancel();
            procCommTask?.Wait();

            if(disposeManaged) {
                cancellationSource = null;
                procCommTask = null;
            }
        }
        #endregion
    }
}
