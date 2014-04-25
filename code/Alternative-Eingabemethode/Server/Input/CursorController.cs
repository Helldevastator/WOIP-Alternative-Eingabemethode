using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server
{
    public delegate void CursorEvent(CursorController sender);

    /// <summary>
    /// Is responsible for translating mote input to user action
    /// </summary>
    public class CursorController
    {
        private MoteController mote;
        private List<Client> clients;
        private State s;
        private Client lastClient;
        private MoteState lastState;

        public CursorController(MoteController mote, List<Client> clients)
        {
            this.clients = clients;
            this.mote = mote;
            this.mote.MoteUpdated += new StateListener(moteListener);
        }

        #region input to user action translation
        /// <summary>
        /// Mote listener which translates mote input to user actions
        /// </summary>
        /// <param name="mote"></param>
        /// <param name="state"></param>
        private void moteListener(MoteController mote, MoteState state)
        {
            //if Cursor has been removed from the screen
            if (state.configuration == IRBarConfiguration.NONE)
            {
                if (lastClient != null)
                {
                    lastClient.Cursors.Remove(mote.Id);
                    lastClient = null;
                }
            }

            else
            {
                Client c = clients[(int)state.configuration];

                //remove cursor from old client, because the client has changed now
                if (c != lastClient)
                    lastClient.Cursors.Remove(mote.Id);

                lastClient = c;
                Point p = CalcNewPosition(state, c);
                CursorState cursor;

                //add cursor if it isn't already present. also set cursor 
                if (c.Cursors.ContainsKey(mote.Id))
                {
                    cursor = c.Cursors[mote.Id];
                    cursor.x = p.X;
                    cursor.y = p.Y;
                    cursor.activated = state.buttonA | state.buttonB;
                }
                else
                {
                    cursor = new CursorState() { x = p.X, y = p.Y, activated = state.buttonA | state.buttonB };
                    c.Cursors.Add(mote.Id,cursor);
                }

                switch (s)
                {
                    case State.NONE:
                        if (state.buttonA)
                            s = State.SCALE;
                        if (state.buttonB)
                            s = State.MOVE;
                        break;
                    case State.SCALE:
                        ScaleWindow(state, c);
                        if (!state.buttonA)
                            s = State.NONE;
                        break;
                    case State.MOVE:
                        MoveWindow(state, c);
                        if (!state.buttonB)
                            s = State.NONE;
                        break;
                }

                lastState = state;
            }
        }
        
        private Point CalcNewPosition(MoteState state, Client client)
        {

            return new Point(0,0);
        }

        private void ScaleWindow(MoteState state, Client client)
        {

        }

        private void MoveWindow(MoteState state, Client client)
        {

        }

        private enum State
        {
            NONE,
            SCALE,
            MOVE
        }
        #endregion
    }
}
