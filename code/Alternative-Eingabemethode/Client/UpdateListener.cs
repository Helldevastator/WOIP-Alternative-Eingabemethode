﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace Client
{
    /// <summary>
    /// This class listens for Update messages over the TCP/IP network stack.
    /// </summary>
    sealed class UpdateListener : IDisposable
    {
        public delegate void UpdateClientListener(ClientState state);

        public event UpdateClientListener UpdateEvent;
        private readonly Socket listenerSocket;
        private readonly Thread listenerThread;

        public UpdateListener(EndPoint adress)
        {
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(adress);
            listenerThread = new Thread(new ThreadStart(ListenerMethod));
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        /// <summary>
        /// Start Thread for 
        /// </summary>
        private void ListenerMethod()
        {
            listenerSocket.Listen(2);
            Socket handler = listenerSocket.Accept();

            while (true)
            {
                ClientState state = (ClientState) NetworkIO.ReceiveObject(handler);
                if (UpdateEvent != null)
                    UpdateEvent.BeginInvoke(state, null, null);
            }
        }

        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disp)
        {
            if (disp)
            {
                if (this.listenerSocket != null) listenerSocket.Dispose();
            }
        }
        #endregion
    }
}
