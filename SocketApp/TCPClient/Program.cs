using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.IO;


namespace SocketApp
{
    class Program
    {
        //#5
        static void Main(string[] args)
        {
            string ipString = "127.0.0.1";
            //string ipString = "localhost";
            int port = 2001;

            TCPSocketLib.TcpCom tccr = new TCPSocketLib.TcpCom(ipString, port);
            tccr.Start();
            

            while (true)
            {
                var msg = Console.ReadLine();
                tccr.OnSend(msg);
                Thread.Sleep(0);
            }

        }

        /*
    // #4
    static void Main(string[] args)
    {
        Console.WriteLine("文字列Enter");
        string sendMsg = Console.ReadLine();

        if (sendMsg == null || sendMsg.Length == 0)
        {
            return;
        }

        //サーバーのIPアドレス（または、ホスト名）とポート番号
        string ipOrHost = "127.0.0.1";
        //string ipOrHost = "localhost";
        int port = 2002;

        { 
            TCPSocketLib.TCPClientSend tcpsend = new TCPSocketLib.TCPClientSend(ipOrHost, port);
            tcpsend.send(0);
        }

        Console.WriteLine("切断しました。");

        Console.ReadLine();
    }
    */

        /*
        // #3
        static void Main(string[] args)
        {
            Console.WriteLine("文字列Enter");
            string sendMsg = Console.ReadLine();

            if (sendMsg == null || sendMsg.Length == 0)
            {
                return;
            }

            //サーバーのIPアドレス（または、ホスト名）とポート番号
            string ipOrHost = "127.0.0.1";
            //string ipOrHost = "localhost";
            int port = 2001;

            //TcpClientを作成し、サーバーと接続する
            System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient(ipOrHost, port);
            Console.WriteLine("サーバー({0}:{1})と接続しました({2}:{3})。",
                ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Address,
                ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Port,
                ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Address,
                ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Port
                );

            //NetworkStreamを取得する
            System.Net.Sockets.NetworkStream ns = tcp.GetStream();
            //読み取り、書き込みのタイムアウトを10秒にする
            //デフォルトはInfiniteで、タイムアウトしない
            //(.NET Framework 2.0以上が必要)
            ns.ReadTimeout = 10000;
            ns.WriteTimeout = 10000;

            StreamWriter sw = new StreamWriter(ns, Encoding.UTF8, 128, true);
            StreamReader sr = new StreamReader(ns, Encoding.UTF8, true);

            {
                //クライアントにデータを送信する
                //クライアントに送信する文字列を作成
                //文字列をByte型配列に変換
                sw.Write(sendMsg + '\n');
                sw.Flush();
                //データを送信する
                Console.WriteLine(sendMsg);
            }


            // サーバーから送られたデータを受信する
            {
                try
                {
                    //データの一部を受信する
                    var line = sr.ReadLine();
                    if (line == null)
                    {
                        //break;
                    }
                    else
                    {
                        // 末尾の\nを削除
                        line = line.TrimEnd('\n');
                        Console.WriteLine(line);
                    }

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

            //stream close
            sr.Close();
            sw.Close();

            //閉じる
            ns.Close();
            tcp.Close();
            Console.WriteLine("切断しました。");

            Console.ReadLine();
        }

        */

        /* #2

                static void Main(string[] args)
                {
                    Console.WriteLine("文字列Enter");
                    string sendMsg = Console.ReadLine();

                    if (sendMsg == null || sendMsg.Length == 0)
                    {
                        return;
                    }

                    //サーバーのIPアドレス（または、ホスト名）とポート番号
                    string ipOrHost = "127.0.0.1";
                    //string ipOrHost = "localhost";
                    int port = 2001;

                    //TcpClientを作成し、サーバーと接続する
                    System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient(ipOrHost, port);
                    Console.WriteLine("サーバー({0}:{1})と接続しました({2}:{3})。",
                        ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Address,
                        ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Port,
                        ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Address,
                        ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Port
                        );

                    //NetworkStreamを取得する
                    System.Net.Sockets.NetworkStream ns = tcp.GetStream();
                    //読み取り、書き込みのタイムアウトを10秒にする
                    //デフォルトはInfiniteで、タイムアウトしない
                    //(.NET Framework 2.0以上が必要)
                    ns.ReadTimeout = 10000;
                    ns.WriteTimeout = 10000;

                    // サーバーにデータを送信する
                    System.Text.Encoding enc = System.Text.Encoding.UTF8;
                    byte[] sendBytes = enc.GetBytes(sendMsg + '\n');
                    //データを送信する
                    ns.Write(sendBytes, 0, sendBytes.Length);
                    Console.WriteLine(sendMsg);

                    // サーバーから送られたデータを受信する
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    byte[] resBytes = new byte[256];
                    int resSize = 0;
                    do
                    {
                        //データの一部を受信する
                        resSize = ns.Read(resBytes, 0, resBytes.Length);
                        if(resSize == 0)
                        {
                            Console.WriteLine("サーバーが切断しました");
                            break;
                        }
                        //受信したデータを蓄積する
                        ms.Write(resBytes, 0, resSize);
                        //まだ読み取れるデータがあるか、データの最後が\nでない時は、
                        // 受信を続け
                    } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');

                    //受信したデータを文字列に変換
                    string resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                    ms.Close();

                    // 末尾の\nを削除
                    resMsg = resMsg.TrimEnd('\n');
                    Console.WriteLine(resMsg);


                    //閉じる
                    ns.Close();
                    tcp.Close();
                    Console.WriteLine("切断しました。");

                    Console.ReadLine();
                }

                */


        /*
         * 
         * First Commit 初回接続と切断のみ #1

        static void Main(string[] args)
        {
            Console.WriteLine("文字列Enter");
            string sendMsg = Console.ReadLine();

            if(sendMsg == null || sendMsg.Length == 0)
            {
                return;
            }

            //サーバーのIPアドレス（または、ホスト名）とポート番号
            string ipOrHost = "127.0.0.1";
            //string ipOrHost = "localhost";
            int port = 2001;

            //TcpClientを作成し、サーバーと接続する
            System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient(ipOrHost, port);
            Console.WriteLine("サーバー({0}:{1})と接続しました({2}:{3})。",
                ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Address,
                ((System.Net.IPEndPoint)tcp.Client.RemoteEndPoint).Port,
                ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Address,
                ((System.Net.IPEndPoint)tcp.Client.LocalEndPoint).Port
                );

            //NetworkStreamを取得する
            System.Net.Sockets.NetworkStream ns = tcp.GetStream();
            //読み取り、書き込みのタイムアウトを10秒にする
            //デフォルトはInfiniteで、タイムアウトしない
            //(.NET Framework 2.0以上が必要)
            ns.ReadTimeout = 10000;
            ns.WriteTimeout = 10000;

            //閉じる
            ns.Close();
            tcp.Close();
            Console.WriteLine("切断しました。");

            Console.ReadLine();
        }
        */
    }
}
