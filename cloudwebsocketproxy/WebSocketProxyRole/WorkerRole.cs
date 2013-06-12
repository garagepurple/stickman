using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using SuperWebSocket;
using SuperSocket.SocketBase;

namespace WebSocketProxyRole
{
    public class WorkerRole : RoleEntryPoint
    {
        static List<WebSocketSession> sessions = new List<WebSocketSession>();

        private WebSocketServer appServer;

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("WebSocketProxyRole entry point called", "Information");

            while (true)
            {
                Thread.Sleep(10000);
                Trace.WriteLine("Working", "Information");
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            appServer = new WebSocketServer();

            //Setup the appServer
            if (!appServer.Setup(2012)) //Setup with listening port
            {
                Trace.WriteLine("Failed to setup!");
                Console.ReadKey();
                return false;
            }

            appServer.NewSessionConnected += new SessionHandler<WebSocketSession>(appServer_NewSessionConnected);
            appServer.SessionClosed += new SessionHandler<WebSocketSession, CloseReason>(appServer_SessionClosed);
            appServer.NewMessageReceived += new SessionHandler<WebSocketSession, string>(appServer_NewMessageReceived);

            //Try to start the appServer
            if (!appServer.Start())
            {
                Trace.WriteLine("Failed to start!");
                return false;
            }


            return base.OnStart();
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
            //Send the received message back
            //session.Send("Server: " + message);
            foreach (var s in sessions)
            {
                if (s != session)
                {
                    s.Send(message);
                }
            }
        }
    }
}
