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
        private AnimationServer animator;
        private State inputState;
        private Point currentPoint;
        private Client currentClient;
        private AnimationWindow currentWindow;

        public CursorController(MoteController mote, AnimationServer animator)
        {
            this.animator = animator;
            this.mote = mote;
            this.mote.MoteUpdated += new StateListener(moteListener);
        }

        #region input to user action translation
        /// <summary>
        /// Mote listener which translates mote input to user actions
        /// </summary>
        /// <param name="mote"></param>
        /// <param name="InputState"></param>
        private void moteListener(MoteController mote, MoteState state)
        {
            this.currentClient = animator.GetClient(state.configuration);
            this.currentPoint = this.CalcNewPosition(state, currentClient);

            switch (inputState)
            {
                case State.NONE:
                    this.currentWindow = animator.GetWindow(currentPoint,currentPoint);

                    if (inputState.buttonA && window != null)
                        inputState = State.SCALE;

                    if (inputState.buttonB && window != null)
                    {
                        animator.StartMoveWindow(currentClient,currentWindow, currentPoint);
                        inputState = State.MOVE;
                    }
                    break;

                case State.SCALE:
                    if (!inputState.buttonA)
                        inputState = State.NONE;

                    double factor = CalcFactor(state);
                    animator.ScaleWindow(currentClient, currentWindow, factor);
                    break;

                case State.MOVE:
                    if (!inputState.buttonB)
                    {
                        animator.FinishMove(client,currentWindow,currentPoint);
                        inputState = State.NONE;
                    }
                    else
                        animator.MoveWindow(currentwindow, window, newPosition);
                    break;
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
