using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib;

namespace Server
{
    public delegate void CursorEvent(CursorController sender);

    /// <summary>
    /// Is responsible for handling use cases like "user rotated window x"
    /// </summary>
    public class CursorController
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

        private void moteUpdated(MoteController mote, MoteState state)
        {
            Client c = clients[(int)state.configuration];
            switch (s)
            {
                case State.NONE:

                    break;
                case State.SCALE:
                    break;
                case State.MOVE:
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
