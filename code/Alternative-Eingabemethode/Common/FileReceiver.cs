using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace Common
{
    /// <summary>
    /// Receives a File from a Websocket and saves it on the local disk.
    /// Binary Format expected:
    ///     -Filename String Lenth (int)
    ///     -Data Length in Bytes (int)
    ///     -String (encoded utf8)
    ///     -Data
    /// </summary>
    public class FileReceiver
    {
        public static void Receive(DirectoryInfo saveDir, Socket sender, byte[] buffer) {
            if (!saveDir.Exists)
                saveDir.Create();

            int read = sender.Receive(buffer,8,SocketFlags.None);
            int stringLen = BitConverter.ToInt32(buffer, 0);
            int dataLen = BitConverter.ToInt32(buffer, 4);

            //read string
            StringBuilder name = new StringBuilder(); 
        }

    }
}
