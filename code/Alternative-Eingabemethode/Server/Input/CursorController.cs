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
        private readonly MoteController mote;
        private readonly AnimationServer animator;

        //begin lock by lockCurrrent
        private Object lockCurrent = new Object();
        private State currentInputState;
        private Point currentPoint;
        private Client currentClient;
        private AnimationWindow currentWindow;
        private bool isActivated;
        //end lock

        public CursorController(MoteController mote, AnimationServer animator)
        {
            this.animator = animator;
            this.mote = mote;
            this.mote.MoteUpdated += new StateListener(moteListener);
        }

        /// <summary>
        /// Get the current cursorstate and at which client it points to.
        /// </summary>
        /// <param name="currentClient"></param>
        /// <param name="cursorState"></param>
        public void GetCursorState(out Client currentClient, out CursorState cursorState)
        {
            lock (lockCurrent)
            {
                currentClient = this.currentClient;
                cursorState = new CursorState() { cursorId = mote.Id, x = currentPoint.X, y = currentPoint.Y, activated = isActivated };
            }
        }

        #region input to user action translation
        /// <summary>
        /// Mote listener which translates mote input to user actions
        /// </summary>
        /// <param name="mote"></param>
        /// <param name="InputState"></param>
        private void moteListener(MoteController mote, MoteState state)
        {
            lock (this.lockCurrent)
            {
                this.currentClient = animator.GetClient(state.configuration);
                this.currentPoint = this.CalcNewPosition(state, currentClient);
                this.isActivated = state.buttonA | state.buttonB;

                switch (currentInputState)
                {
                    case State.NONE:
                        this.currentWindow = animator.GetWindow(currentPoint, currentPoint);

                        if (currentInputState.buttonA && window != null)
                            currentInputState = State.SCALE;

                        if (currentInputState.buttonB && window != null)
                        {
                            animator.StartMoveWindow(currentClient, currentWindow, currentPoint);
                            currentInputState = State.MOVE;
                        }
                        break;

                    case State.SCALE:
                        if (!currentInputState.buttonA)
                            currentInputState = State.NONE;

                        double factor = CalcFactor(state);
                        animator.ScaleWindow(currentClient, currentWindow, factor);
                        break;

                    case State.MOVE:
                        if (!currentInputState.buttonB)
                        {
                            animator.FinishMove(client, currentWindow, currentPoint);
                            currentInputState = State.NONE;
                        }
                        else
                            animator.MoveWindow(currentwindow, window, newPosition);
                        break;
                }
            }
        }
        
        private Point CalcNewPosition(MoteState state, Client client)
        {

            return new Point(0,0);
        }

        private double CalcFactor(MoteState state)
        {
            return 1.0;
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
