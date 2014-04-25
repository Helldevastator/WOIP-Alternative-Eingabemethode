using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Server
{
    /// <summary>
    /// Is responsible for animating the window movement on the clients. it is also responsible for sending the window state updates to the clients
    /// </summary>
    public class AnimationServer
    {
        private Dictionary<int, Client> clients;

        public AnimationServer()
        {

        }

        public Client GetClient()
        {
            return null;
        }

        public AnimationWindow GetWindow(Point atPoint)
        {
            return null;
        }

        /// <summary>
        /// Start movement of window 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="newPosition">first new position</param>
        public void StartMoveWindow(AnimationWindow window, Point newPosition)
        {

        }

        /// <summary>
        /// Move this window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="newPosition"></param>
        public void MoveWindow(AnimationWindow window, Point newPosition)
        {
        }

        /// <summary>
        /// This method has to be called! Always!
        /// It finishes up the window movement and handles corner cases
        /// </summary>
        /// <param name="client">can be null, if the window has been moved out of the original client's window and not inside a new one</param>
        /// <param name="window"></param>
        /// <param name="finalPosition"></param>
        public void FinishMove(Client client, AnimationWindow window, Point finalPosition)
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
