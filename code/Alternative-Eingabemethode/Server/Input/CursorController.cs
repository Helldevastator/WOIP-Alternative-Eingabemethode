using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Server.Input
{
    /// <summary>
    /// Is responsible for translating mote input to user action
    /// </summary>
    public class CursorController
    {
        private const double barSizeCM = 20;
        private const double scaleFactor = 3.0 / 90.0;  //3 times for a 90 degrees pixel rotation
        private readonly WiimoteAdapter mote;
        private readonly AnimationServer animator;

        //begin lock by lockCurrrent
        private Object lockCurrent = new Object();
        private State currentInputState;
        private Point currentPoint;
        private Client currentClient;
        private AnimationWindow currentWindow;
        private bool isActivated;
        private float currentRoll;

        //HACK!! used to correctly implement rotation. it isn't nice because it's a fucking workaround for the fucking gyroscope not fucking working.
        private Client lastClient;
        private Point lastPoint;
        private AnimationWindow lastWindow;
        //end lock

        public CursorController(WiimoteAdapter mote, AnimationServer animator)
        {
            this.animator = animator;
            this.mote = mote;
            this.mote.MoteUpdatedEvent += new StateListener(moteListener);
        }

        /// <summary>
        /// Get the current cursorstate and at which client it points to.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="cursorState"></param>
        public void GetCursorState(out Client currentClient, out CursorState cursorState)
        {
            lock (lockCurrent)
            {
                currentClient = this.currentClient;
                cursorState = new CursorState() { CursorId = mote.Id, X = currentPoint.X, Y = currentPoint.Y, Activated = isActivated };
            }
        }

        #region input to user action translation
        /// <summary>
        /// Mote listener which translates mote input to user actions
        /// </summary>
        /// <param name="mote"></param>
        /// <param name="InputState"></param>
        private void moteListener(WiimoteAdapter mote, MoteState state)
        {
            lock (this.lockCurrent)
            {
                this.currentClient = animator.GetClient(state.configuration);
                this.currentPoint = this.CalculateScreenPosition(state, currentClient);
                this.isActivated = state.buttonA || state.buttonB;

                switch (currentInputState)
                {
                    case State.NONE:
                        this.currentWindow = null;

                        if(this.isActivated)
                            this.currentWindow = animator.GetWindow(currentClient, currentPoint);

                        if (state.buttonA && currentWindow != null)
                        {
                            animator.StartMoveWindow(currentClient, currentWindow);
                            currentInputState = State.SCALE;
                            this.lastClient = currentClient;
                            this.lastPoint = currentPoint;
                            this.lastWindow = currentWindow;
                            this.currentRoll = state.roll;
                        }

                        if (state.buttonB && currentWindow != null)
                        {
                            animator.StartMoveWindow(currentClient, currentWindow);
                            animator.MoveWindow(currentClient, currentWindow, currentPoint);
                            currentInputState = State.MOVE;
                        }
                        break;

                    case State.SCALE:
                        if (!state.buttonA)
                        {
                            currentInputState = State.NONE;
                            animator.FinishMove(this.lastClient, this.lastWindow,this.lastPoint);
                        }

                        //begin hack
                        currentPoint = lastPoint;
                        currentClient = lastClient;
                        currentWindow = lastWindow;
                        //end hack
                        double factor = CalculateScaleFactor(state);
                        System.Console.WriteLine(factor.ToString("0.000000000"));
                        animator.ScaleWindow(currentClient, currentWindow, factor);
                        break;

                    case State.MOVE:
                        if (!state.buttonB)
                        {
                            animator.MoveWindow(currentClient, currentWindow, currentPoint);
                            animator.FinishMove(currentClient, currentWindow,currentPoint);
                            currentInputState = State.NONE;
                        }
                        else
                        {
                            animator.MoveWindow(currentClient, currentWindow, currentPoint);
                        }
                        break;
                }
            }
        }
        
        private Point CalculateScreenPosition(MoteState state, Client client)
        {
            if (client != null && state.horizontal != null)
            {
                int moteWidth = WiimoteAdapter.IR_PIXEL_WIDTH;
                int moteHeight = WiimoteAdapter.IR_PIXEL_HEIGHT;
                double xPointAt = 0;    // centimeters of where the wiimote is pointing. (0,0) is in the upper left corner
                double yPointAt = 0;

                //calculate distance in pixels between the ir points;
                double dx = (state.horizontal.p2.X - state.horizontal.p1.X) * moteWidth;
                double dy = (state.horizontal.p2.Y - state.horizontal.p1.Y) * moteHeight;
                double distance = Math.Sqrt(dx * dx + dy * dy);

                InputPoint centerDistance = CalcDistanceToCenter(state.horizontal);
                xPointAt = centerDistance.X * moteWidth / distance * barSizeCM + client.CmWidth / 2.0;
                if (state.configuration == IRBarConfiguration.LEFT_TOP || state.configuration == IRBarConfiguration.RIGHT_TOP)
                    yPointAt = (centerDistance.Y * moteHeight) / distance * barSizeCM;

                else
                    yPointAt = client.CmHeight - (centerDistance.Y * moteHeight / distance * barSizeCM);

                int xPixel = (int)(xPointAt / client.CmWidth * client.PixelWidth);
                int yPixel = (int)(yPointAt / client.CmHeight * client.PixelHeight);
                if (xPixel >= 0 && xPixel < client.PixelWidth && yPixel >= 0 && yPixel < client.PixelHeight)
                    return new Point(xPixel, yPixel);
            }
          
            return new Point(-1,-1);
        }

        private InputPoint CalcDistanceToCenter(BarPoints bar)
        {
            double x = (bar.p2.X-bar.p1.X)/2;
            double y = (bar.p2.Y-bar.p1.Y)/2;

            double centerX = bar.p1.X + x;
            double centerY = bar.p1.Y+y;

            return new InputPoint(0.5- centerX, 1-centerY);
        }

        private double CalculateScaleFactor(MoteState state)
        {
            double factor = (state.roll - currentRoll) * scaleFactor;
            factor = 1 + factor;
            return factor;
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
