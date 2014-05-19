using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Server.Input;

namespace Server
{
    /// <summary>
    /// Is responsible for animating the window movement on the clients. it is also responsible for sending the window InputState updates to the clients.
    /// 
    /// Threadsafe
    /// </summary>
    public class AnimationServer
    {
        #region static factory method
        /// <summary>
        /// Factory method for this class. Creates an AnimationServer Object
        /// </summary>
        /// <param name="controllers"></param>
        /// <param name="clients"></param>
        /// <returns></returns>
        internal static AnimationServer AnimationServerFactory(List<MoteController> controllers, Dictionary<int, Client> clients, ResourceServer resServer)
        {
            AnimationServer server = new AnimationServer(clients,resServer);

            foreach (MoteController mote in controllers)
                server.cursors.Add(new CursorController(mote, server));

            return server;
        }
        #endregion

        private readonly Dictionary<int, Client> clients;
        private readonly ResourceServer resourceServer;

        //lock?
        private readonly List<CursorController> cursors;

        /// <summary>
        /// Private Constructor, otherwise the this reference would escape the constructor, which would be a problem with multithreaded apps.
        /// </summary>
        /// <param name="clients"></param>
        private AnimationServer(Dictionary<int, Client> clients,ResourceServer resServer)
        {
            this.clients = clients;
            cursors = new List<CursorController>(4);
            this.resourceServer = resServer;
        }

        /// <summary>
        /// Returns the client which has specified IRBarConfiguration
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns>null if there is no configuration</returns>
        public Client GetClient(IRBarConfiguration configuration)
        {
            if(configuration == IRBarConfiguration.NONE)
                return null;

            return clients[(int)configuration];
        }

        public AnimationWindow GetWindow(Client client, Point atPoint)
        {
            if (client != null)
                return client.GetWindowAt(atPoint);
            else
                return null;
        }

        /// <summary>
        /// Start moving window 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="newPosition">first new position</param>
        public void StartMoveWindow(Client client,AnimationWindow window, Point newPosition)
        {
            window.startMove();
            window.move(newPosition);
        }

        /// <summary>
        /// Move this window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="newPosition"></param>
        public void MoveWindow(Client client,AnimationWindow window, Point newPosition)
        {
            window.move(newPosition);
        }

        /// <summary>
        /// This method has to be called! Always!
        /// It finishes up the window movement and handles corner cases
        /// </summary>
        /// <param name="client">can be null, if the window has been moved out of the original client'InputState window and not inside a new one</param>
        /// <param name="window"></param>
        /// <param name="finalPosition"></param>
        public void FinishMove(Client client, AnimationWindow window, Point finalPosition)
        {
            if (client != null)
            {
                window.move(finalPosition);
                window.finishMove();
            }
            else
            {
                window.resetMove();
                window.Client.AddWindow(window);
            }
        }

        public void ScaleWindow(Client client, AnimationWindow window, double factor)
        {

        }

        /// <summary>
        /// Remove window from client. If this is called inside a StartMove() ... FinishMove(), the Window might be restored
        /// </summary>
        /// <param name="client"></param>
        /// <param name="window"></param>
        public void RemoveWindowFromClient(Client client, AnimationWindow window)
        {
            client.RemoveWindow(window);
        }

        /// <summary>
        /// Add Window to client.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="window"></param>
        /// <param name="startPosition"></param>
        public void AddWindowToClient(Client client, AnimationWindow window, Point startPosition)
        {
            client.AddWindow(window);
            window.resetSlide();
            window.move(startPosition);
            resourceServer.SendResource(client, window.ResourceId);  
        }
    }
}
