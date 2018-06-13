using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace TCPSocketLib
{
    public class TCPClientReceive
    {
        private TcpClient tcp = null;
        TcpConnectReceive tcpConnectReceive;
        public TCPClientReceive(TcpClient tcp, TcpConnectReceive tcpConnectReceive)
        {
            this.tcp = tcp;
            this.tcpConnectReceive = tcpConnectReceive;

            Thread thread = new Thread(new ThreadStart(Proc));
            thread.Start();
        }
        public void Proc()
        {
            NetworkStream ns = tcp.GetStream();
            ns.ReadTimeout = 1000;
            ns.WriteTimeout = 1000;
            StreamReader sr = new StreamReader(ns, Encoding.UTF8, false, 1024, true);

            while (true)
            {
                try
                {
                    //データの一部を受信する
                    var line = sr.ReadLine();
                    if (line == null)
                    {

                        //break;
                        Console.WriteLine(line);
                        return;
                    }
                    else
                    {
                        line = line.TrimEnd('\n');
                        Console.WriteLine(line);
                    }
                    Thread.Sleep(0);
                }
                catch (EndOfStreamException)
                {
                    Console.WriteLine("Disconnected .");
                    return;
                }
                catch (IOException)
                {
                    Console.WriteLine("Disconnected .");
                    return;
                }
                catch (ObjectDisposedException)
                {
                    Console.WriteLine("Disconnected .");
                    return;
                }

            }


        }
    }
}
