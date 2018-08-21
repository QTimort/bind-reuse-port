using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ReusePortTest
{
    
    public class Program
    {

        [DllImport("libbindreuseport.so")]
        private static extern int bindReusePort(int fd, int family, UInt16 port, bool verbose);

        private static readonly FieldInfo endPointField = typeof(Socket).GetField("_rightEndPoint", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly byte[] receiveBuffer = new byte[4096];

        public static void OnReceive(IAsyncResult ar)
        {
            Console.WriteLine("Received: ");
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(receiveBuffer));
        }

        static void Main(string[] args)
        {
            ushort port = 5353;
            AddressFamily addressFamily = AddressFamily.InterNetwork;
            SocketType socketType = SocketType.Dgram;
            ProtocolType protocolType = ProtocolType.Udp;
            Socket socket = new Socket(addressFamily, socketType, protocolType);

            int errorCode = bindReusePort(socket.Handle.ToInt32(), (int)addressFamily, port, true);
            if (errorCode != 0)
            {
                Console.Error.WriteLine("Failed to bind reusing port " + port);
            } 
            
            endPointField.SetValue(socket, new IPEndPoint(IPAddress.Any, port));
            socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, OnReceive, null);
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }
}