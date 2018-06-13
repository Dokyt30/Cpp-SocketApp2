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
    public class TcpConnectReceive
    {

        private string ipString;
        private int portNo;
        private TcpClient tcp;
        // スレッド停止命令用
        private bool stop_flg = false;
        public TcpConnectReceive(string ipString, int portNo)
        {
            this.ipString   = ipString;
            this.portNo     = portNo;
            Thread thread   = new Thread(new ThreadStart(ListenStart));
            thread.Start();
        }

        public void Dispose()
        {
            stop_flg = true;

            GC.SuppressFinalize(this);
        }

        // 接続待ちスレッド本体
        private void ListenStart()
        {
            System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(ipString);

            TcpListener listener = new TcpListener(ipAdd, this.portNo);
            listener.Start();
            Console.WriteLine("Listenを開始しました({0}:{1})。", ((System.Net.IPEndPoint)listener.LocalEndpoint).Address, ((System.Net.IPEndPoint)listener.LocalEndpoint).Port);

            while (!stop_flg)
            {
                // 接続待ちあるか？
                if (listener.Pending() == true)
                {
                    // 接続要求があったら受け入れる
                    tcp = listener.AcceptTcpClient();
                    Console.WriteLine("クライアント({0}:{1})と接続しました。", ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Address, ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Port);
                    TCPClientReceive tcpcr = new TCPClientReceive(tcp, this);

                }
                else
                {
                    Thread.Sleep(0);
                }
            }
            listener.Stop();
        }



        public void OnSend(string msg)
        {
            NetworkStream ns = tcp.GetStream();
            ns.ReadTimeout = 1000;
            ns.WriteTimeout = 1000;

            var sw = new StreamWriter(ns, Encoding.UTF8, 1024, true);
            sw.Write(msg);
            sw.Flush();
        }

 
        // 各イベント？
        public void OnReceive(long ret)
        {
            Console.WriteLine("receive:{0}", ret.ToString());
            if (ret == 0)
            {
                stop_flg = true;
            }
        }

        internal void ClientGetByte(long ret)
        {
            OnReceive(ret);
        }
    }
}
