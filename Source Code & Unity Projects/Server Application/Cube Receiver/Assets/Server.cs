using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets
{
    public class Server : MonoBehaviour
    {
        public int Port = 9999;
        public int MaxConnections = 10;

        // The id we use to identify our messages and register the handler
        short _messageId = 1000;
        short _cubeMessageId = 1001;

        public Event CubeMessageReceived;

        [Tooltip("Text that displays client status")]
        public Text ServerText;
        [Tooltip("Text that displays incoming messages from clients.")]
        public Text MessagesText;

        [Tooltip("The maximum number of messages to be displayed in the Message Box.")]
        public int MaxMessages = 7;

        private int _messageCount;

        // Use this for initialization
        void Start()
        {
            // Usually the server doesn't need to draw anything on the screen
            Application.runInBackground = true;
            CreateServer();
        }

        void CreateServer()
        {
            // Register handlers for the types of messages we can receive
            RegisterHandlers();

            var config = new ConnectionConfig();
            // There are different types of channels you can use, check the official documentation
            config.AddChannel(QosType.ReliableFragmented);
            config.AddChannel(QosType.UnreliableFragmented);

            var ht = new HostTopology(config, MaxConnections);

            if (!NetworkServer.Configure(ht))
            {
                Debug.Log("No server created, error on the configuration definition");
                return;
            }
            else
            {
                // Start listening on the defined port
                if (NetworkServer.Listen(Port))
                    Debug.Log("Server created, listening on port: " + Port);
                else
                    Debug.Log("No server created, could not listen to the port: " + Port);
            }
        }

        void OnApplicationQuit()
        {
            NetworkServer.Shutdown();
        }

        private void RegisterHandlers()
        {
            // Unity have different Messages types defined in MsgType
            NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnected);

            // Our message use his own message type.
            NetworkServer.RegisterHandler(_messageId, OnMessageReceived);
            NetworkServer.RegisterHandler(_cubeMessageId, OnCubeMessageReceived);
        }

        private void RegisterHandler(short t, NetworkMessageDelegate handler)
        {
            NetworkServer.RegisterHandler(t, handler);
        }

        void OnClientConnected(NetworkMessage netMessage)
        {
            // Do stuff when a client connects to this server
            SetServerConnectionText("Client has connected");
            AddMessageToUi($"Client has connected: {netMessage.channelId}");
            // Send a thank you message to the client that just connected
            MyNetworkMessage messageContainer = new MyNetworkMessage();
            messageContainer.Message = "Thanks for joining!";

            // This sends a message to a specific client, using the connectionId
            NetworkServer.SendToClient(netMessage.conn.connectionId, _messageId, messageContainer);

            // Send a message to all the clients connected
            messageContainer = new MyNetworkMessage();
            messageContainer.Message = "A new player has conencted to the server";

            // Broadcast a message a to everyone connected
            NetworkServer.SendToAll(_messageId, messageContainer);
        }

        void OnClientDisconnected(NetworkMessage netMessage)
        {
            // Do stuff when a client dissconnects
            SetServerConnectionText("Client has disconnected");
            AddMessageToUi($"Client has disconnected: {netMessage.channelId}");
            GridController.Instance.ResetCubes();
        }

        void OnMessageReceived(NetworkMessage netMessage)
        {
            // You can send any object that inherence from MessageBase
            // The client and server can be on different projects, as long as the MyNetworkMessage or the class you are using have the same implementation on both projects
            // The first thing we do is deserialize the message to our custom type
            var objectMessage = netMessage.ReadMessage<MyNetworkMessage>();
            Debug.Log("Message received: " + objectMessage.Message);

        }

        private void OnCubeMessageReceived(NetworkMessage netMessage)
        {
            //var receiveTime = (long) (DateTime.Now - _epoch).TotalMilliseconds;
            var objectMessage = netMessage.ReadMessage<SelectedCubeMessage>();
            var timeDifference = objectMessage.SentTime;
            if (objectMessage.Cube1 == -1)
            {
                GridController.Instance.ResetCubes();
                AddMessageToUi($"Reset Cubes Message Received: Time Taken {timeDifference}ms");
            }
            else
            {
                GridController.Instance.SetGrid(objectMessage.Cube1, objectMessage.Cube2);
                AddMessageToUi($"New Cube Message from {netMessage.channelId}: [{objectMessage.Cube1},{objectMessage.Cube2}]." +
                               $"Time Taken {timeDifference}ms");
            }
            
            Debug.Log($"Message received: Time Taken {timeDifference}ms." + objectMessage.Cube1 + "," + objectMessage.Cube2);
        }

        private void AddMessageToUi(string message)
        {
            var text = MessagesText.text;
            if (_messageCount > MaxMessages)
            {
                text = "Messages:\n";
                _messageCount = 0;
            }

            text = text + $"\n {message}";
            _messageCount++;
            MessagesText.text = text;
        }

        private void SetServerConnectionText(string connectionText)
        {
            ServerText.text = connectionText;
        }
    }
}