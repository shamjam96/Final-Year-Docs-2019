using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Networking
{
    public class Client : Singleton<Client>
    {
        public  int Port = 9999;
        [Tooltip("Server's IP Address")]
        public string ServerIp = "localhost"; // Unity

        // The id we use to identify our messages and register the handler
        short _messageId = 1000;
        short _cubeMessageId = 1001;

        // The network client
        public NetworkClient client;

        public Client()
        {
        }

        void Start()
        {
            CreateClient();
        }

        public long GetRTT()
        {
            return client.GetRTT();
        }
        void CreateClient()
        {

            var config = new ConnectionConfig();

            // Config the Channels we will use
            config.AddChannel(QosType.ReliableFragmented);
            config.AddChannel(QosType.UnreliableFragmented);

            // Create the client ant attach the configuration
            client = new NetworkClient();
            client.Configure(config, 1);

            // Register the handlers for the different network messages
            RegisterHandlers();

            // Connect to the server
            client.Connect(ServerIp, Port);
        }

        // Register the handlers for the different message types
        void RegisterHandlers()
        {

            // Unity have different Messages types defined in MsgType
            client.RegisterHandler(_messageId, OnMessageReceived);
            client.RegisterHandler(MsgType.Connect, OnConnected);
            client.RegisterHandler(MsgType.Disconnect, OnDisconnected);

        }

        void OnConnected(NetworkMessage message)
        {
            // Do stuff when connected to the server

            MyNetworkMessage messageContainer = new MyNetworkMessage();
            messageContainer.message = "Hello server!";

            // Say hi to the server when connected
            client.Send(_messageId, messageContainer);
        }

        void OnDisconnected(NetworkMessage message)
        {
            // Do stuff when disconnected to the server
        }

        // Message received from the server
        void OnMessageReceived(NetworkMessage netMessage)
        {
            // You can send any object that inherence from MessageBase
            // The client and server can be on different projects, as long as the MyNetworkMessage or the class you are using have the same implementation on both projects
            // The first thing we do is deserialize the message to our custom type
            var objectMessage = netMessage.ReadMessage<MyNetworkMessage>();

            Debug.Log("Message received: " + objectMessage.message);
        }
    }
}