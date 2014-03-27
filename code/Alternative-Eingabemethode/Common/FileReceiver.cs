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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveDir"></param>
        /// <param name="receiver"></param>
        /// <param name="buffer">has to be able to contain the Filename String in UTF-8</param>
        public static void Receive(DirectoryInfo saveDir, Socket receiver, byte[] buffer) {
            if (!saveDir.Exists)
                saveDir.Create();

            string name;

            //readTotal header
            ReadExact(receiver, 8, buffer, 0);
            int stringLen = BitConverter.ToInt32(buffer, 0);
            int dataLen = BitConverter.ToInt32(buffer, 4);

            //readTotal string
            ReadExact(receiver, stringLen, buffer, 0);
            name = Encoding.UTF8.GetString(buffer, 0, stringLen);

            //readTotal binary and save to file
            BinaryWriter w = new BinaryWriter(File.OpenWrite(Path.Combine(saveDir.FullName, name)));
            try
            {
                int readTotal = 0;

                while (readTotal < dataLen)
                {
                    //ensures that no more bytes are read than needed.
                    int byesToRead = dataLen - readTotal > buffer.Length ? buffer.Length : dataLen - readTotal;
                    int read = receiver.Receive(buffer, byesToRead, SocketFlags.None);

                    w.Write(buffer, 0, read);
                    readTotal += read;
                }

            }
            finally
            {
                w.Close();
            }
           
        }

        /// <summary>
        /// Reads an exact number of bytes and writes them in the buffer
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="bytecount">minimum Number of Bytes to readTotal</param>
        /// <param name="buffer">to write in</param>
        /// <param name="offset">offset in buffer, starts writing at specified index</param>
        public static void ReadExact(Socket receiver, int bytecount, byte[] buffer, int offset)
        {
            int read = 0;

            while (read < bytecount)
            {
                read += receiver.Receive(buffer, offset+read, bytecount-read, SocketFlags.None);
            }
        }

    }
}
