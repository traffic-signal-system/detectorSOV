using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace detectorSOV
{
        class Udp
        {

            static Socket _socket;
            static IPEndPoint _local = new IPEndPoint(IPAddress.Parse("192.168.0.2"), 5238);
            static Thread _receiveThread = null;

            #region 开启接收线程
            public static void StartReceive()
            {
                try
                {

                    _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    bool aa = _socket.Connected;
                    _receiveThread = new Thread(Receive);
                    _receiveThread.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                }
            }
            #endregion

            #region 向服务端发送数据

            public static bool sendUdp(string ipstr, int port, byte[] hex)
            {
                try
                {
                    // int len;
                    // bool flag = IsSocketConnected(_socket);
                    IPEndPoint ip = new IPEndPoint(IPAddress.Parse(ipstr), port);
                    _socket.SendTimeout = 1000;
                    _socket.ReceiveTimeout = 1000;
                    _socket.SendTo(hex, ip);
                    return true;

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return false;

                }
            }
            #endregion

            #region 关闭接收线程和socket连接
            public static void Close()
            {
                if (_receiveThread != null)
                    _receiveThread.Abort();
                if (_socket != null)
                    _socket.Close();
            }
            #endregion

            #region 接收线程委托方法
            private static void Receive()
            {
                try
                {

                    if (_socket.Connected)
                    {

                        _socket.Bind(_local);
                    }



                    while (true)
                    {
                        byte[] buffer = new byte[255];
                        EndPoint remoteEP = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
                        int len = _socket.ReceiveFrom(buffer, ref remoteEP);
                        IPEndPoint ipEndPoint = remoteEP as IPEndPoint;
                        //GBT_20999_Utils gb20999 = new GBT_20999_Utils(buffer, len);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString() + _socket.Connected.ToString() + "UpdSocket类 Receive方法出错");
                }
            }
            #endregion


            public static bool sendUdpNoReciveData(string ipstr, int port, byte[] hex)
            {
                int recv;
                Socket server = null;
                byte[] bytes = new byte[65535];
                try
                {
                    IPEndPoint ip = new IPEndPoint(IPAddress.Parse(ipstr), port);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    // string str = "Hello Server!";
                    //bytes = System.Text.Encoding.ASCII.GetBytes(str);
                    server.SendTimeout = 4000;
                    server.ReceiveTimeout = 4000;
                    server.SendTo(hex, ip);
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint Remote = (EndPoint)(sender);
                    //server.Bind(sender);
                    recv = server.ReceiveFrom(bytes, ref Remote);
                    Console.WriteLine("Message received from {0}", Remote.ToString());
                    //str = System.Text.Encoding.ASCII.GetString(bytes, 0, recv);
                    Console.WriteLine("Message: " + bytes[0]);
                    server.Close();
                    server = null;
                    if (bytes[0] == 134)
                        return false;
                    else
                        return true;
                }
                catch (Exception exce)
                {
                    Console.WriteLine(exce.ToString());
                    server.Close();
                    server = null;
                    return false;
                }

            }


            public static byte[] recvUdp(string ipstr, int port, byte[] hex)
            {
                int recv;
                Socket server = null;
                byte[] bytes = new byte[65535];
                byte[] result = { };
                try
                {
                    IPEndPoint ip = new IPEndPoint(IPAddress.Parse(ipstr), port);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    // string str = "Hello Server!";
                    server.ReceiveTimeout = 4000;
                    server.SendTimeout = 4000;
                    //bytes = System.Text.Encoding.ASCII.GetBytes(str);
                    server.SendTo(hex, ip);
                    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                    EndPoint Remote = (EndPoint)(sender);
                    //server.Bind(sender);
                    recv = server.ReceiveFrom(bytes, ref Remote);
                    Console.WriteLine("Message received from {0}", Remote.ToString());
                    //str = System.Text.Encoding.ASCII.GetString(bytes, 0, recv);
                    Console.WriteLine("Message: " + bytes[0]);
                    result = new byte[recv];
                    Array.Copy(bytes, result, recv);
                    server.Close();
                    server = null;
                    return result;
                }
                catch (Exception exce)
                {
                    Console.WriteLine(exce.ToString());
                    server.Close();
                    server = null;
                    return result;
                }

            }
            /// <summary>
            /// 得到本机的所以IP，如果只有一个网卡也就得
            /// 到一个IP，有多个就可以得到多少IP
            /// </summary>
            /// <returns></returns>
            public static List<IPAddress> getLocalHostIpAddress()
            {
                List<IPAddress> list = new List<IPAddress>();
                //for(int i=0;i)
                IPAddress[] ipa = Dns.GetHostByName(Dns.GetHostName()).AddressList;
                for (int i = 0; i < ipa.Length; i++)
                    list.Add(ipa[i]);
                return list;
            }
            /// <summary>
            /// 通过得到IP地址可以得到IP地址网段，自动将在此网段的给列出信号机
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <returns></returns>
            //public static List<IPAddress> getTSCIPAddress(IPAddress ipAddress)
            //{
            //    UdpClient client = new UdpClient(2344); //绑定UDP到本地端口

            //    IPEndPoint ep = null;
            //    //远端的终结点，Receive时会用到，表示接受来自远端IP和端口的数据
            //    List<IPAddress> list = new List<IPAddress>();
            //    string ipaddr = ipAddress.ToString();
            //    string[] iparr = ipaddr.Split('.');
            //    for (int i = 0; i < 255; i++)
            //    {
            //        ep = new IPEndPoint(IPAddress.Parse(iparr[0] + "." + iparr[1] + "." + iparr[2] + "." + i), Define.broadcast_port);

            //        try
            //        {
            //            byte[] data = Encoding.ASCII.GetBytes("Hello Tsc!");
            //            client.Send(data, data.Length, ep);
            //            client.Client.ReceiveTimeout = 10;    //设置超时为1000ms
            //            data = client.Receive(ref ep);  //连续接受数据
            //            Console.WriteLine(ep.Address.ToString() + ":" + ep.Port.ToString() + "\t" + Encoding.Default.GetString(data));
            //            list.Add(ep.Address);
            //        }
            //        catch (Exception ex)//超时的话退出Receive
            //        {
            //            Console.Write("{0}\n", ex.Message);

            //            continue;
            //        }
            //    }
            //    client.Close();
            //    return list;
            //}
            /// <summary>
            /// 发送信息
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            //private static byte[] SendBroadcast()
            //{
            //     byte[] buffer = new byte[1024];
            //    try
            //    {
            //        UdpClient client = new UdpClient(AddressFamily.InterNetwork);
            //        IPEndPoint iep = new IPEndPoint(IPAddress.Broadcast, Define.broadcast_port);
            //        buffer = Encoding.UTF8.GetBytes("123456");
            //        client.Send(buffer, buffer.Length, iep);
            //        buffer = client.Receive(ref iep);
            //        client.Close();
            //    }catch(Exception ext)
            //    {
            //        //(ext.Message);
            //    }

            //    return buffer;
            //}


            /// <summary>
            /// 参数要注意
            /// </summary>
            /// <param name="ip">192.168.0.</param>
            /// <returns></returns>
            //public static List<IPAddress> getTSCIPAddress(string ip)
            //{
            //    List<IPAddress> list = new List<IPAddress>();
            //    UdpClient client = new UdpClient(2344); //绑定UDP到本地端口
            //    IPEndPoint ep = null;
            //    for (int i = 0; i < 255; i++)
            //    {
            //        ep = new IPEndPoint(IPAddress.Parse(ip + i), Define.broadcast_port);

            //        try
            //        {
            //            byte[] data = Encoding.ASCII.GetBytes("Hello Tsc!");
            //            client.Send(data, data.Length, ep);
            //            client.Client.ReceiveTimeout = 10;    //设置超 时为1000ms
            //            data = client.Receive(ref ep);  //连续接受数据
            //            Console.WriteLine(ep.Address.ToString() + ":" + ep.Port.ToString() + "\t" + Encoding.Default.GetString(data));
            //            list.Add(ep.Address);
            //        }
            //        catch (Exception ex)//超时的话退出Receive
            //        {
            //            Console.Write("{0}\n", ex.Message);
            //            client.Close();
            //            //break;
            //        }
            //    }
            //    client.Close();
            //    return list;
            //}
        }
    
}
