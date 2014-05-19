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
        public readonly Dictionary<int, DisplayWindow> windows;
        public readonly Dictionary<int, DisplayCursor> cursors;
        readonly Display display;
        readonly ResourceManager resources;

        public DisplayController(Display display, ResourceManager resources)
        {
            this.display = display;
            this.windows = new Dictionary<int, DisplayWindow>();
            this.cursors = new Dictionary<int, DisplayCursor>();
            this.resources = resources;
        }

        public void UpdateClient(ClientState state)
        {
            var windowStates = state.Windows;
            var cursorStates = state.Cursors;

            foreach (WindowState w in windowStates)
            {
                int id = w.WindowId;
                //remove
                if (w.RemovedFlag)
                    this.windows.Remove(id);

                if (this.windows.ContainsKey(id))
                {
                    //update
                    this.windows[id].Update(w);
                }
                else
                {
                    //add
                    DisplayWindow window = new DisplayWindow(resources.GetWaitResource());
                    resources.SetOrUpdateResource(new ResourceManager.ResourceLoadedCallback(window.ResourceLoadedCallback), w.ResourceId);
                    this.windows.Add(id, window);
                }
            }

            foreach (CursorState s in cursorStates)
            {
                //remove
                if (s.RemovedFlag)
                    this.cursors.Remove(s.CursorId);

                if (this.cursors.ContainsKey(s.CursorId))
                {
                    //update
                    this.cursors[s.CursorId].Update(s);
                }
                else
                {
                    //add
                    this.cursors.Add(s.CursorId, new DisplayCursor(s));
                }
            }

            display.UpdateDisplay(this.windows, this.cursors);
        }
    }
}
