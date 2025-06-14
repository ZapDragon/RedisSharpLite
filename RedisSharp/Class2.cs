using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisSharp
{
    internal class Class2
    {
        private static TcpClient tcpClient;
        private static NetworkStream networkStream;
        private static bool debugPrint = false;

        static void Main()
        {
            while (true)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("redis > ");
                string command = Console.ReadLine();
                if (string.IsNullOrEmpty(command)) { continue; }

                if (command == "$debug")
                {
                    debugPrint = !debugPrint;
                    Console.WriteLine("Debug print is " + debugPrint);
                    continue;
                }
                else if (command == "$clear")
                {
                    Console.Clear();
                    continue;
                }

                string response = RedisExecute(command);
                if (!debugPrint)
                {
                    Console.Write("\n" + response + "\n");
                }
            }
        }

        public static string Execute(string command)
        {
            return RedisExecute(command);
        }

        private static string RedisExecute(string command)
        {
            StringBuilder sb = new StringBuilder();

            using (tcpClient = new TcpClient())
            {
                
                tcpClient.NoDelay = true;
                tcpClient.ReceiveBufferSize = 10240;
                tcpClient.SendBufferSize = 10240;
                tcpClient.Connect("172.24.0.7", 6379);
                //tcpClient.Client.Blocking = false;
                networkStream = tcpClient.GetStream();

                // Better used for Text style protocols, especially ones /r/n terminated.
                StreamWriter writer = new StreamWriter(networkStream);
                StreamReader reader = new StreamReader(networkStream);

                // Sets auto-flush so we dont have to send our commands after writing to the socket every time.
                writer.AutoFlush = true;

                writer.WriteLine(command);

                bool gotData = false;
                bool gotAllRows = false;

                // We dont want to spin in the socket loops when the socket doesnt intend to send any more.
                // If we spin for too long
                Stopwatch sw = Stopwatch.StartNew();

                while (!gotData)
                {
                    int expectedRows = 0;
                    int returnedRows = 0;

                    while (tcpClient.Client.Available > 0)
                    {
                        // We got in here before the timeout. Reset it for the inner loops.
                        sw.Restart();

                        // A flag so we can specify that we did get data from the socket, and we can break out, once we leave this inner loop when no more data is to be read.
                        gotData = true; 

                        // Get our first row. Should always be the header.
                        string header = reader.ReadLine();
                        if (debugPrint) { Console.WriteLine(header); }

                        // The header contains astrisk (*) number 1+ (*1 or *5000)
                        // This defines how many rows we're to expect as a reply.
                        if (header.StartsWith("*"))
                        {
                            // Removes the * so we can count our expected rows.
                            header = header.Substring(1);

                            // Now we know how many rows we're getting back.
                            expectedRows = Convert.ToInt32(header);
                        }
                        // This is an error response from redis - Error is inline with the header. Just return the header.
                        else if (header.StartsWith("-"))
                        {
                            return header;
                        }
                        // The + character indicates a server status reply to a command like "select"
                        else if (header.StartsWith("+"))
                        {
                            // returns the whole header, since its a status message (There may be multiple lines)
                            return header;
                        }
                        // The $ character indicates that the next line is one larger string.
                        else if (header.StartsWith("$"))
                        {
                            header = header.Substring(1);
                            int expectedbytes = Convert.ToInt32(header);
                            int readBytes = 0;
                            char[] buff = new char[expectedbytes];

                            while (expectedbytes > readBytes)
                            {
                                readBytes += reader.ReadBlock(buff, readBytes, expectedbytes - readBytes);
                            }

                            foreach (char c in buff)
                            {
                                sb.Append(c);
                            }
                            if (debugPrint) { Console.WriteLine(sb.ToString()); }
                            return sb.ToString();
                        }

                        // While expected rows is not the number of rows we have.
                        while (expectedRows > returnedRows)
                            {
                                // If we spin in here for too long, Break out. Something is wrong.
                                if (sw.Elapsed.TotalSeconds > 5)
                                {
                                    sw.Stop();
                                    break;
                                }

                                // Get the next line.
                                string line = reader.ReadLine();
                                if (debugPrint) { Console.WriteLine(line); }

                                // If we're at the end, we need to wait for more data to come in.
                                if (string.IsNullOrEmpty(line)) { continue; }

                                // Lines that start with $ are byte lengths of the next line. Since these are \r\n terminated, we dont need this info.
                                // Also, this is valid data, restart our counter.
                                if (line.StartsWith("$")) { sw.Restart(); continue; }

                                // Increase our row counter.
                                returnedRows++;

                                // We got data, reset the counter.
                                sw.Restart();

                                // We have our next row.
                                sb.AppendLine(line);
                            }

                        // If we have the expected number of rows, our read was successful. Set our flag, and break out of this loop.
                        if (returnedRows == expectedRows)
                        {
                            sw.Stop();
                            gotAllRows = true;
                            break;
                        }
                    }

                    // If we spin in here for too long, Break out. Something is wrong.
                    if (sw.Elapsed.TotalSeconds >= 5)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Too much time waiting for the buffer to fill, and we havent read anything yet.");
                        Console.ResetColor();
                        sw.Stop();
                        break;
                    }
                }

                // After the !gotData loop
                Console.ForegroundColor = ConsoleColor.Red;
                if (!gotAllRows) { Console.WriteLine("We didnt get all rows before breaking out."); }
                if (!gotData) { Console.WriteLine("We didnt get any data, we must have timed out in the loop."); }
                Console.ResetColor();

            }
            return sb.ToString();
        }
    }
}
