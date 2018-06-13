using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace TCPSocketLib
{
    public class TCPClientSend
    {
        public string hostName;
        public int portNo;
        public TCPClientSend(string hostName, int portNo)
        {
            this.hostName = hostName;
            this.portNo = portNo;
        }

        public Boolean send(long value)
        {
            try
            {
                // クライアント用のソケット作成
                TcpClient cl = new TcpClient();
                // サーバーへ接続
                cl.Connect(hostName, portNo);
                // 接続したソケットからneworkStreamを取得
                NetworkStream stream = cl.GetStream();
                Encoding encode = Encoding.Default;
                // 送信する文字列をバイト配列に変換
                // この際に、エンコードも同時に行う
                string s = value.ToString();
                byte[] bytData = encode.GetBytes(s);

                // 書き出しを行う
                stream.Write(bytData, 0, bytData.Length);
                // フラッシュ(強制書き出し)
                stream.Flush();
                // ソケットをクローズ
                cl.Close();
            }
            catch (SocketException eSocket)
            {
                System.Diagnostics.Debug.Write(eSocket.Message);
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
                return false;
            }
            return true;
        }
            
    }
}
