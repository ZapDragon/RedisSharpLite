//using System;
//using System.CodeDom;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics.Eventing.Reader;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading;

//namespace RedisSharp
//{
//    class GHGHG
//    {
//        private RedisClient client = new RedisClient();

//        private static void a()
//        {

//        }
//    }

//    public class RedisClient : IDisposable
//    {
//        private string server_address = "127.0.0.1";
//        private int server_port = 6379;
//        private IPEndPoint server_endPoint;

//        private bool IsConnected = false;
//        private bool IsDisposed = false;

//        private TcpClient client;
//        private NetworkStream ns;
//        private BinaryReader sr = new BinaryReader(new MemoryStream(), Encoding.UTF8);
//        private BinaryWriter sw;
//        private ClientMode rMode = ClientMode.Request;

//        private ConcurrentQueue<byte[]> SendBufferQueue = new ConcurrentQueue<byte[]>();
//        private ConcurrentQueue<byte[]> ReceiveBufferQueue = new ConcurrentQueue<byte[]>();

//        private Thread socketThread;
//        private bool _disposing = false;

//        private int dbslot = 0;

//        #region Constructor Overloads

//        /// <summary>
//        /// Creates a new RedisClient instance.
//        /// </summary>
//        public RedisClient()
//        {
//            Initialize();
//        }

//        /// <summary>
//        /// Creates a new RedisClient instance.
//        /// </summary>
//        /// <param name="address">Specifies the Redis Server IP address to connect to.</param>
//        public RedisClient(string address = "127.0.0.1")
//        {
//            Initialize(address);
//        }

//        /// <summary>
//        /// Creates a new RedisClient instance.
//        /// </summary>
//        /// <param name="address">Specifies the Redis Server IP address to connect to.</param>
//        /// <param name="port">Specifies the Redis Server port to connect to.</param>
//        public RedisClient(string address = "127.0.0.1", int port = 6379)
//        {
//            Initialize(address, port);
//        }

//        /// <summary>
//        /// Creates a new RedisClient instance.
//        /// </summary>
//        /// <param name="address">Specifies the Redis Server IP address to connect to.</param>
//        /// <param name="port">Specifies the Redis Server port to connect to.</param>
//        /// <param name="database">Selects the Redis DB number to be used for this connection</param>
//        public RedisClient(string address = "127.0.0.1", int port = 6379, int database = 0)
//        {
//            Initialize(address, port, database);
//        }

//        /// <summary>
//        /// Creates a new RedisClient instance.
//        /// </summary>
//        /// <param name="address">Specifies the Redis Server IPAddress to connect to.</param>
//        public RedisClient(IPAddress address)
//        {
//            Initialize(address);
//        }

//        /// <summary>
//        /// Creates a new RedisClient instance.
//        /// </summary>
//        /// <param name="address">Specifies the Redis Server IPAddress to connect to.</param>
//        /// <param name="port">Specifies the Redis Server port to connect to.</param>
//        public RedisClient(IPAddress address, int port = 6379)
//        {
//            Initialize(address, port);
//        }

//        /// <summary>
//        /// Creates a new RedisClient instance.
//        /// </summary>
//        /// <param name="address">Specifies the Redis Server IPAddress to connect to.</param>
//        /// <param name="port">Specifies the Redis Server port to connect to.</param>
//        /// <param name="database">Selects the Redis DB number to be used for this connection</param>
//        public RedisClient(IPAddress address, int port = 6379, int database = 0)
//        {
//            Initialize(address, port, database);
//        }

//        /// <summary>
//        /// Creates a new RedisClient instance.
//        /// </summary>
//        /// <param name="endpoint">Specifies the Redis Server IPEndPoint to connect to.</param>
//        public RedisClient(IPEndPoint endpoint)
//        {
//            Initialize(endpoint);
//        }

//        /// <summary>
//        /// Creates a new RedisClient instance.
//        /// </summary>
//        /// <param name="endpoint">Specifies the Redis Server IPEndPoint to connect to.</param>
//        public RedisClient(IPEndPoint endpoint, int database = 0)
//        {
//            Initialize(endpoint, database);
//        }

//        #endregion

//        #region IDisposable Block

//        ~RedisClient()
//        {
//            IsDisposed = true;
//            Dispose(false);
//        }

//        public void Dispose()
//        {
//            Dispose(false);
//            GC.SuppressFinalize(this);
//        }

//        protected virtual void Dispose(bool disposing)
//        {
//            if (IsDisposed) { return; }

//            IsDisposed = true;

//            if (disposing)
//            {
//                if (socketThread.IsAlive)
//                {
//                    Thread.Sleep(10000);
//                    if (socketThread.IsAlive) { socketThread.Abort(); }
//                }
//                socketThread = null;

//                sw?.Dispose();
//                sw = null;

//                sr?.Dispose();
//                sr = null;

//                client?.Dispose();
//                client = null;
//            }
//        }

//        #endregion

//        #region Initializer Block

//        /// <summary>
//        /// Called by the constructor to setup the environment, Using default information, since no connection details were given.
//        /// </summary>
//        private void Initialize()
//        {
//            // Create our final endpoint for connection.
//            server_endPoint = new IPEndPoint(IPAddress.Parse(server_address), server_port);

//            // Create our tcp client
//            client = new TcpClient(server_endPoint);

//            // Cant make this till connected
//            // sr = new BinaryReader()
//        }

//        private void Initialize(string address)
//        {
//            server_address = address;

//            IPAddress ip = null;
//            if (!IPAddress.TryParse(server_address, out ip))
//            {
//                throw new InvalidIPAddressException();
//            }

//            // Create our final endpoint for connection.
//            server_endPoint = new IPEndPoint(ip, server_port);

//            // Create our tcp client
//            client = new TcpClient(server_endPoint);
//        }

//        private void Initialize(string address, int port)
//        {
//            server_address = address;
//            server_port = port;

//            IPAddress ip = null;
//            if (!IPAddress.TryParse(server_address, out ip))
//            {
//                throw new InvalidIPAddressException();
//            }

//            // Create our final endpoint for connection.
//            server_endPoint = new IPEndPoint(ip, server_port);

//            // Create our tcp client
//            client = new TcpClient(server_endPoint);
//        }

//        private void Initialize(string address, int port, int database)
//        {
//            server_address = address;
//            server_port = port;
//            dbslot = database;

//            IPAddress ip = null;
//            if (!IPAddress.TryParse(server_address, out ip))
//            {
//                throw new InvalidIPAddressException();
//            }

//            // Create our final endpoint for connection.
//            server_endPoint = new IPEndPoint(ip, server_port);

//            // Create our tcp client
//            client = new TcpClient(server_endPoint);
//        }

//        private void Initialize(IPAddress address)
//        {
//            server_address = address.ToString();

//            // Create our final endpoint for connection.
//            server_endPoint = new IPEndPoint(address, server_port);

//            // Create our tcp client
//            client = new TcpClient(server_endPoint);
//        }

//        private void Initialize(IPAddress address, int port)
//        {
//            server_address = address.ToString();
//            server_port = port;

//            // Create our final endpoint for connection.
//            server_endPoint = new IPEndPoint(address, server_port);

//            // Create our tcp client
//            client = new TcpClient(server_endPoint);
//        }

//        private void Initialize(IPAddress address, int port, int database)
//        {
//            server_address = address.ToString();
//            server_port = port;
//            dbslot = database;

//            // Create our final endpoint for connection.
//            server_endPoint = new IPEndPoint(address, server_port);

//            // Create our tcp client
//            client = new TcpClient(server_endPoint);
//        }

//        private void Initialize(IPEndPoint endPoint)
//        {
//            server_address = endPoint.Address.ToString();
//            server_port = endPoint.Port;

//            // Create our final endpoint for connection.
//            server_endPoint = endPoint;

//            // Create our tcp client
//            client = new TcpClient(endPoint);
//        }

//        private void Initialize(IPEndPoint endPoint, int database)
//        {
//            server_address = endPoint.Address.ToString();
//            server_port = endPoint.Port;
//            dbslot = database;

//            // Create our final endpoint for connection.
//            server_endPoint = endPoint;

//            // Create our tcp client
//            client = new TcpClient(endPoint);
//        }

//        #endregion

//        #region Connection/Client Methods

//        /// <summary>
//        /// Establishes a connection with the defined (or default loopback) Redis server.
//        /// Can also be used to reconnect if the connection is briefly interrupted.
//        /// </summary>
//        public void Connect()
//        {
//            socketThread = new Thread(tcpThreadRunner);
//            socketThread.Name = "RedisSharpSocketThread";
//            socketThread.IsBackground = true;
//            socketThread.Start();
//        }

//        /// <summary>
//        /// Disconnects the established connection with the Redis server. 
//        /// This connection can be re-established, without re-creating the instance.
//        /// </summary>
//        public void Disconnect()
//        {

//        }

//        /// <summary>
//        /// 
//        /// 
//        /// </summary>
//        /// <param name="command">Any Redis, or Redis Extension supported commands in plain text</param>
//        /// <returns>A formatted string containing your query results.</returns>


//        /// <summary>
//        /// This is single command allows you to perform calls to the connected Redis server as a plain string.
//        /// Any supported Redis command can be executed here and the result will be returned as a new instance of T
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="command"></param>
//        /// <returns></returns>
//        public T Execute<T>(string command)
//        {
//            string t = GetStaticType(typeof(T));

//            switch (t)
//            {
//                case "string": { break; }
//                case "int": { break; }
//                case "long": { break; }
//                case "double": { break; }
//                case "float": { break; }
//                case "string": { break; }
//                case "string": { break; }
//                case "string": { break; }
//                case "string": { break; }
//                case "string": { break; }
//                case "string": { break; }

//            }
//        }

//        private string GetStaticType<T>(T x) => typeof(T).Name;



//        #region Configuration Methods

//        /// <summary>
//        /// Allows you to change how RedisSharp passes database information to you.
//        /// Can only be set when disconnected.
//        /// </summary>
//        /// <param name="mode">
//        /// Request: Default mode: Use Execute(), and get a response.
//        /// Monitor: RedisSharp enters monitor mode, and passes all events into the ReceiveBufferQueue for you to pull from as needed. Adjust the filter with SetMonitorFilter()
//        /// Queue: The Execute command returns an empty string. All results of the query are stored in ReceiveBufferQueue for you to pull from as needed.
//        /// </param>
//        public void SetClientMode(ClientMode mode)
//        {

//        }

//        public void SetMonitorFilter()
//        {
//        }

//        #endregion


//        #endregion

//        #region Socket Thread

//        /// <summary>
//        /// The mastermind behind the quick replies to querries.
//        /// This thread makes sure the connection stays active, errors are handled, and disconnections are cleaned up.
//        /// </summary>
//        private void tcpThreadRunner()
//        {
//            client.Connect(server_endPoint);
//            client.Client.ReceiveBufferSize = 1024;
//            client.Client.SendBufferSize = 1024;
//            client.Client.Blocking = false;

//            IsConnected = true;

//            NetworkStream ns = client.GetStream();
//            sw = new BinaryWriter(ns);
//            sr = new BinaryReader(ns);

//            if (rMode == ClientMode.Request)
//            {

//                while (!_disposing && client.Connected)
//                {
//                    sr.BaseStream.
//                }
//            }
//            else if (rMode == ClientMode.Queue)
//            {
//                while (!_disposing && client.Connected)
//                {

//                }
//            }
//            else if (rMode == ClientMode.Monitor)
//            {
//                while (!_disposing && client.Connected)
//                {

//                }
//            }

//            IsConnected = false;

//            sr?.Close();
//            sr = null;

//            sw?.Close();
//            sw = null;

//            ns.Close();
//            ns = null;

//            client?.Close();
//            client?.Dispose();

//            // Thread dies
//            return;
//        }

//        #endregion
//    }

//    public enum ClientMode : byte
//    {
//        Request = 0,
//        Monitor = 1,
//        Queue = 2
//    }

//    #region Exception Handlers

//    public class InvalidIPAddressException : Exception { }

//    #endregion
//}
