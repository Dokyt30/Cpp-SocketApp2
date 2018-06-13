using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace TCPServer
{
    class Program
    {
        // #5
        //static void Main(string[] args)
        //{
        //    //string ipString = "127.0.0.1";
        //    string ipString = "localhost";
        //    int port = 2002;

        //    TCPSocketLib.TcpCom tccr = new TCPSocketLib.TcpCom(ipString, port);
        //    tccr.Start();
        //    Console.ReadLine();

        //}


//#4
        static void Main(string[] args)
        {
            string ipString = "127.0.0.1";
            int port = 2001;

            TCPSocketLib.TcpConnectReceive tccr = new TCPSocketLib.TcpConnectReceive(ipString, port);
            while (true)
            {
                var msg = Console.ReadLine();
                tccr.OnSend(msg);
                Thread.Sleep(0);
            }

        }


        /*
        // #3
        static void Main(string[] args)
        {
            string ipString = "127.0.0.1";
            System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(ipString);


            int port = 2001;

            System.Net.Sockets.TcpListener listener = new System.Net.Sockets.TcpListener(ipAdd, port);

            //Listenするポート番号
            listener.Start();
            Console.WriteLine("Listenを開始しました({0}:{1})。", ((System.Net.IPEndPoint)listener.LocalEndpoint).Address, ((System.Net.IPEndPoint)listener.LocalEndpoint).Port);

            // 接続要求があったら受け入れる
            System.Net.Sockets.TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("クライアント({0}:{1})と接続しました。", ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address, ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Port);

            // NetworkStreamを取得
            System.Net.Sockets.NetworkStream ns = client.GetStream();
            //読み取り、書き込みのタイムアウトを10秒にする
            //デフォルトはInfiniteで、タイムアウトしない
            //(.NET Framework 2.0以上が必要)
            ns.ReadTimeout = 10000;
            ns.WriteTimeout = 10000;

            string resMsg = "";
            
            //クライアントから送られたデータを受信する
            bool disconnected = false;

            StreamReader sr = new StreamReader(ns, Encoding.UTF8, true);
            StreamWriter sw = new StreamWriter(ns, Encoding.UTF8, 128, true);

            {
                try
                {
                    //データの一部を受信する
                    var line = sr.ReadLine();
                    if (line == null)
                    {
                        disconnected = true;
                        //break;
                    }
                    else
                    {
                        line = line.TrimEnd('\n');
                        Console.WriteLine(line);
                    }

                    resMsg = line;

                }
                catch (EndOfStreamException)
                {
                    Console.WriteLine("Disconnected .");
                }
                catch (IOException)
                {
                    Console.WriteLine("Disconnected .");
                }
                catch (ObjectDisposedException)
                {
                    Console.WriteLine("Disconnected .");
                }

            }

            if (!disconnected)
            {
                {
                    //クライアントにデータを送信する
                    //クライアントに送信する文字列を作成
                    //文字列をByte型配列に変換
                    sw.Write(resMsg+'\n');
                    sw.Flush();
                    //データを送信する
                    Console.WriteLine(resMsg);
                }
            }


            //stream close
            sr.Close();
            sw.Close();
            //閉じる
            ns.Close();
            client.Close();
            Console.WriteLine("クライアントとの接続を閉じました。");

            //リスナを閉じる
            listener.Stop();
            Console.WriteLine("Listenerを閉じました。");

            Console.ReadLine();
        }
        */

        /* #2
        static void Main(string[] args)
        {
            string ipString = "127.0.0.1";
            System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(ipString);


            int port = 2001;

            System.Net.Sockets.TcpListener listener = new System.Net.Sockets.TcpListener(ipAdd, port);

            //Listenするポート番号
            listener.Start();
            Console.WriteLine("Listenを開始しました({0}:{1})。", ((System.Net.IPEndPoint)listener.LocalEndpoint).Address, ((System.Net.IPEndPoint)listener.LocalEndpoint).Port);

            // 接続要求があったら受け入れる
            System.Net.Sockets.TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("クライアント({0}:{1})と接続しました。", ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address, ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Port);

            // NetworkStreamを取得
            System.Net.Sockets.NetworkStream ns = client.GetStream();

            // 読み取り、書き込みのタイムアウトを10秒にする
            // デフォルトはInfiniteで、タイムアウトしない
            //(.NET Framework 2.0以上が必要)
            ns.ReadTimeout = 10000;
            ns.WriteTimeout = 10000;

            //クライアントから送られたデータを受信する
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            bool disconnected = false;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[256];
            int resSize = 0;

            do
            {
                //データの一部を受信する
                resSize = ns.Read(resBytes, 0, resBytes.Length);
                if(resSize == 0)
                {
                    disconnected = true;
                    Console.WriteLine("クライアントが切断しました");
                }
                //受信したデータを蓄積する
                ms.Write(resBytes, 0, resSize);
                //まだ読み取れるデータがあるか、データの最後が\nでない時は、
                // 受信を続ける
            } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');

            //受信したデータを文字列に変換
            string resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            ms.Close();
            //末尾の\nを削除
            resMsg = resMsg.TrimEnd('\n');
            Console.WriteLine(resMsg);

            if (!disconnected)
            {
                //クライアントにデータを送信する
                //クライアントに送信する文字列を作成
                string sendMsg = resMsg.Length.ToString();
                //文字列をByte型配列に変換
                byte[] sendBytes = enc.GetBytes(sendMsg + '\n');
                //データを送信する
                ns.Write(sendBytes, 0, sendBytes.Length);
                Console.WriteLine(sendMsg);
            }

            //閉じる
            ns.Close();
            client.Close();
            Console.WriteLine("クライアントとの接続を閉じました。");

            //リスナを閉じる
            listener.Stop();
            Console.WriteLine("Listenerを閉じました。");

            Console.ReadLine();
        }
        */
        /*
         * 
         * First Commit 初回接続と切断のみ #1
                static void Main(string[] args)
                {
                    string ipString = "127.0.0.1";
                    System.Net.IPAddress ipAdd = System.Net.IPAddress.Parse(ipString);


                    int port = 2001;

                    System.Net.Sockets.TcpListener listener = new System.Net.Sockets.TcpListener(ipAdd, port);

                    //Listenするポート番号
                    listener.Start();
                    Console.WriteLine("Listenを開始しました({0}:{1})。", ((System.Net.IPEndPoint)listener.LocalEndpoint).Address,      ((System.Net.IPEndPoint)listener.LocalEndpoint).Port);

                    // 接続要求があったら受け入れる
                    System.Net.Sockets.TcpClient client = listener.AcceptTcpClient();
                    Console.WriteLine("クライアント({0}:{1})と接続しました。", ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Address, ((System.Net.IPEndPoint)client.Client.RemoteEndPoint).Port);

                    // NetworkStreamを取得
                    System.Net.Sockets.NetworkStream ns = client.GetStream();

                    // 読み取り、書き込みのタイムアウトを10秒にする
                    // デフォルトはInfiniteで、タイムアウトしない
                    //(.NET Framework 2.0以上が必要)
                    ns.ReadTimeout = 10000;
                    ns.WriteTimeout = 10000;

                    // 


                    //閉じる
                    ns.Close();
                    client.Close();
                    Console.WriteLine("クライアントとの接続を閉じました。");

                    //リスナを閉じる
                    listener.Stop();
                    Console.WriteLine("Listenerを閉じました。");

                    Console.ReadLine();
                }
        */
    }
}
