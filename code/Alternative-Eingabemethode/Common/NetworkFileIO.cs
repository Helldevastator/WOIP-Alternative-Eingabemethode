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
    /// Helper functions for sending Files over the network
    /// </summary>
    public static class NetworkFileIO
    {
        /// <summary>
        ///Receives a File from a Websocket and saves it on the local disk.
        /// Binary Format expected:
        ///     -Data Length in Bytes (int)
        ///     -Data
        /// </summary>
        /// <param name="file">File to save data to</param>
        /// <param name="receiver"></param>
        /// <param name="buffer">has to be able to contain the Filename String in UTF-8</param>
        public static void Receive(FileInfo file, Socket receiver, byte[] buffer) {
            if (!file.Directory.Exists)
                file.Directory.Create();

            //readTotal header
            ReadExact(receiver, 4, buffer, 0);
            int dataLen = BitConverter.ToInt32(buffer, 0);

            //readTotal binary and save to file
            BinaryWriter w = new BinaryWriter(file);
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

        public static void SendFile(Socket sender, FileInfo file, byte[] buffer)
        {

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
