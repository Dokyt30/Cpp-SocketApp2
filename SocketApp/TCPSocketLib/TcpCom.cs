using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace TCPSocketLib
{

    public class TcpCom
    {
        
        private TcpClient tcp;
        private Thread threadReceive;
        private string ipString;
        private int portNo;

        public TcpCom(string ipString, int portNo)
        {
            this.ipString = ipString;
            this.portNo = portNo;

            this.tcp = new TcpClient();
            this.threadReceive = new Thread(OnReceive);
        }
        ~TcpCom()
        {
            Dispose();
        }
        public void Dispose()
        {
            this.tcp.Close();
            try
            {
                this.threadReceive.Join();
            }
            catch (ThreadStateException)
            {
            }
            GC.SuppressFinalize(this);
        }

        public bool Start()
        {
            try
            {
                this.tcp.Connect(ipString, portNo);
                Console.WriteLine("サーバー({0}:{1})と接続しました。", ((System.Net.IPEndPoint)this.tcp.Client.RemoteEndPoint).Address, ((System.Net.IPEndPoint)this.tcp.Client.RemoteEndPoint).Port);
                this.threadReceive.Start();
            }catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }


        public void OnSend(string msg)
        {
            NetworkStream ns = tcp.GetStream();
            ns.ReadTimeout = 1000;
            ns.WriteTimeout = 1000;

            var sw = new StreamWriter(ns, Encoding.UTF8, 1024, true);
            sw.Write(msg+'\n');
            sw.Flush();
        }

        public void OnReceive()
        {
            NetworkStream ns = tcp.GetStream();
            ns.ReadTimeout = 1000;
            ns.WriteTimeout = 1000;
            var sr = new StreamReader(ns, Encoding.UTF8, false, 1024, true);
            while (true)
            {
                try
	            {
	                var line = sr.ReadLine();
	                if (line == null)
	                {
	                    Console.WriteLine("ReceiveLine line:null");
	                    break;
	                }
	                else
	                {
	                    line = line.TrimEnd('\n');
	                    Console.WriteLine(line);
	                }

	            }
	            catch (EndOfStreamException)
	            {
	                Console.WriteLine("ReceiveLine EndOfStreamException");
	                break;
	            }
	            catch (IOException)
	            {
	                Console.WriteLine("ReceiveLine IOException");
	                break;
	            }
	            catch (ObjectDisposedException)
	            {
	                Console.WriteLine("ReceiveLine ObjectDisposedException");
	                break;
	            }
            }
        }

    }
}
