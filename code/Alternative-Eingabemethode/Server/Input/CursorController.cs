using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public delegate void CursorEvent(Cursor sender);

    /// <summary>
    /// Is responsible for handling use cases like "user rotated window x"
    /// </summary>
    class CursorController
    {
        private MoteController mote;
        private List<Client> clients;
        private State s;
        public CursorController(Wiimote mote, List<Client> clients)
        {
            this.clients = clients;
            this.mote = new MoteController(mote);
            this.mote.MoteUpdated += new StateListener(moteUpdated);
        }

        private void moteUpdated(MoteController mote, MotionInfo state)
        {
            Client c = clients[state.configuration];
            switch (s)
            {
                case NONE:
                    break;
                case SCALE:
                    break;
                case MOVE:
                    break;
            }
        }


        #region state
        private enum State
        {
            NONE,
            SCALE,
            MOVE
        }
        #endregion

    }
}
