using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Server
{
    /// <summary>
    /// Is responsible for animating the window movement on the clients. it is also responsible for sending the window InputState updates to the clients
    /// </summary>
    public class AnimationServer
    {
        private Dictionary<int, Client> clients;
        private List<CursorController> cursors;

        public AnimationServer()
        {

        }

        //

        public Client GetClient(IRBarConfiguration configuration)
        {
            return null;
        }

        public AnimationWindow GetWindow(Client client, Point atPoint)
        {
            return null;
        }

        /// <summary>
        /// Start movement of window 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="newPosition">first new position</param>
        public void StartMoveWindow(Client client,AnimationWindow window, Point newPosition)
        {

        }

        /// <summary>
        /// Move this window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="newPosition"></param>
        public void MoveWindow(Client client,AnimationWindow window, Point newPosition)
        {

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

        }

        /// <summary>
        /// Add Window to client.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="window"></param>
        /// <param name="startPosition"></param>
        public void AddWindowToClient(Client client, AnimationWindow window, Point startPosition)
        {

        }
    }
}
