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
        private Object updateLock = new Object();

        public DisplayController(Display display, ResourceManager resources)
        {
            this.display = display;
            this.windows = new Dictionary<int, DisplayWindow>();
            this.cursors = new Dictionary<int, DisplayCursor>();
            this.resources = resources;
        }

        public void UpdateClient(ClientState state)
        {
            lock (updateLock)
            {
                System.Console.WriteLine("update received");
                var windowStates = state.Windows;
                var cursorStates = state.Cursors;
                foreach (WindowState w in windowStates)
                {
                    int id = w.WindowId;
                    //remove
                    if (w.RemovedFlag)
                    {
                        System.Console.WriteLine("Removed");
                        this.windows.Remove(id);
                    }

                    if (this.windows.ContainsKey(id))
                    {
                        //update
                        this.windows[id].Update(w);
                    }
                    else
                    {
                        System.Console.WriteLine("ADD NEW WINDOW");
                        //add
                        DisplayWindow window = new DisplayWindow(resources.GetWaitResource());
                        resources.SetOrUpdateResource(new ResourceManager.ResourceLoadedCallback(window.ResourceLoadedCallback), w.ResourceId);
                        this.windows.Add(id, window);
                    }

                }

                //HACK: simply remove all cursors if the count isn't the same anymore
                if (cursorStates.Count != this.cursors.Count)
                    this.cursors.Clear();

                foreach (CursorState s in cursorStates)
                {
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
}
