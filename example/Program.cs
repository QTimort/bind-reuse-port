using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ReusePortTest
{
    
    public class SocketBind
    {
        [DllImport("libbindreuseport.so")]
        private static extern int bindReusePort(int fd, int family, UInt16 port, bool verbose);
        
        private static readonly FieldInfo EndPointField = typeof(Socket).GetField("_rightEndPoint", BindingFlags.NonPublic | BindingFlags.Instance);
        
        public static void Bind(Socket socket, IPEndPoint endPoint)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                socket.Bind(endPoint); // On windows use default bind
            } else { // Linux / Mac
                if (!Equals(endPoint.Address, IPAddress.Any))
                {
                    // You can implement if you want you just have to uncomment code in the source of the C library provided
                    throw new ArgumentException("Binding to a specific address on Linux is not currently implemented!");
                }
                // Set the Linux native address & port reuse option and bind function
                int errorCode = bindReusePort(socket.Handle.ToInt32(), (int) socket.AddressFamily, (ushort) endPoint.Port, false);
                if (errorCode != 0) // error
                {
                    throw new SocketException {HelpLink = "Failed to bind reusing port " + endPoint.Port};
                }
                // If successful manually set with reflection the endpoint with which the socket is bind with
                SocketBind.EndPointField.SetValue(socket, endPoint);
            }
        }
    }
    
    public class Program
    {

        private static readonly byte[] ReceiveBuffer = new byte[4096];

        public static void OnReceive(IAsyncResult ar)
        {
            Console.WriteLine("Received: ");
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(ReceiveBuffer));
        }

        static void Main(string[] args)
        {
            ushort port = 5353;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SocketBind.Bind(socket, new IPEndPoint(IPAddress.Any, port));
            socket.BeginReceive(ReceiveBuffer, 0, ReceiveBuffer.Length, SocketFlags.None, OnReceive, null);
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }
}