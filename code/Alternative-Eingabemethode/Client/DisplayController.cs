using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Client
{
    class DisplayController
    {
        readonly Dictionary<int, DisplayWindow> windows;
        readonly Dictionary<int, DisplayCursor> cursors;
        readonly Display display;
        readonly ResourceManager resources;

        public DisplayController(Display display, Dictionary<int, DisplayWindow> windows, Dictionary<int, DisplayCursor> cursors, ResourceManager resources)
        {
            this.display = display;
            this.windows = windows;
            this.cursors = cursors;
            this.resources = resources;
        }

        public void UpdateClient(ClientState state)
        {
            //remove?

            display.UpdateDisplay(this.windows, this.cursors);
        }
    }
}
