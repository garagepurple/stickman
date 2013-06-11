using System;
using SuperWebSocket;

namespace Demo
{
    class Program
    {
        private const int Port = 2013;
        private static WebSocketSession _currentSession;

        static void Main()
        {
            // Instantiate and setup websocket server
            var wss = new WebSocketServer();
            wss.Setup(Port);

            // Add session event bindings
            wss.NewSessionConnected += session =>
                {
                    Console.WriteLine("Connected. Type a message and ENTER to send");
                    _currentSession = session;
                };
            wss.SessionClosed += (session, value) =>
                {
                    Console.WriteLine("Session disconnected");
                    _currentSession = null;
                };

            // Add message received event bindings
            wss.NewMessageReceived += (session, value) => Console.WriteLine("RECEIVED: " + value);
            
            // Start service (non-blocking)
            wss.Start();

            Console.WriteLine("Browse to http://www.websocket.org/echo.html and connect to location 'ws://localhost:2013'");
            Console.WriteLine("Ctrl-C to exit");

            while (true)
            {
                // Take user input
                var str = Console.ReadLine();
                if (_currentSession == null) continue;
                
                // Send to websocket
                Console.WriteLine("SENDING: " + str);
                _currentSession.Send(str);
            }
        }
    }
}
