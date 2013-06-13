using SuperSocket.SocketBase;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;

namespace websockettransmitter
{
    public class WebSocketHost : IDisposable
    {
        private WebSocketServer appServer;
        private static WebSocket client;
        private static bool clientOpen = false;
        private static List<WebSocketSession> sessions = new List<WebSocketSession>();


        public WebSocketHost()
        {
        }

        public bool Setup()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            appServer = new WebSocketServer();

            //Setup the appServer
            if (!appServer.Setup(2012)) //Setup with listening port
            {
                Console.WriteLine("Failed to setup!");
                Console.ReadKey();
                return false;
            }

            appServer.NewSessionConnected += new SessionHandler<WebSocketSession>(appServer_NewSessionConnected);
            appServer.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(appServer_SessionClosed);
            appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(appServer_NewMessageReceived);

            //Try to start the appServer
            if (!appServer.Start())
            {
                Console.WriteLine("Failed to start!");
                return false;
            }

            // Connect to the cloud
            client = new WebSocket("ws://garagepurple.cloudapp.net:2012");
            client.Opened += new EventHandler(webSocketClient_Opened);
            client.Closed += new EventHandler(webSocketClient_Closed);
            client.MessageReceived += new EventHandler<MessageReceivedEventArgs>(webSocketClient_MessageReceived);
            client.Open();

            return true;
        }

        public void Send(string message)
        {
            if (clientOpen == true)
                client.Send(message);
            try
            {
                foreach (var s in sessions)
                {
                    s.Send(message);
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine("Exception in Send(): {0}", ex.ToString());
            }
        }

        public bool IsCloudConnected()
        {
            return clientOpen;
        }

        private void webSocketClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // Ignore messages from the cloud
        }

        private void webSocketClient_Closed(object sender, EventArgs e)
        {
            clientOpen = false;
        }

        private void webSocketClient_Opened(object sender, EventArgs e)
        {
            clientOpen = true;
        }
        public void Dispose()
        {
            appServer.Dispose();
        }
        private static void appServer_SessionClosed(WebSocketSession session, CloseReason value)
        {
            sessions.Remove(session);
        }

        private static void appServer_NewSessionConnected(WebSocketSession session)
        {
            sessions.Add(session);
        }

        static void appServer_NewMessageReceived(WebSocketSession session, string message)
        {
            // Ignore messages from the web clients
        }
    }
}
