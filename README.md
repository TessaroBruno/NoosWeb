<h1 align="center">NoosWeb</h1>

Simple static C# Web Server, with REST and Websockets functionality

## Usage

Run webserver instance

```csharp
using Noos.Web.WebServer;
using Noos.Web.WebServer.RestContext;
using System;
using System.Threading;

namespace Noos.SampleServerConsole
{
    class Program
    {
        private static WebHandlersManager webserver;

        static void Main(string[] args)
        {
            Thread webServerThread;
            webServerThread = new Thread(new ThreadStart(WebServerWorker));
            webServerThread.Start();

            Console.ReadLine();
            webServerThread.Abort();
        }

        private static void WebServerWorker()
        {
            try
            {
                Console.WriteLine("Running web server");

                // Static
                webserver = new WebHandlersManager(null, "4800",
                    AppDomain.CurrentDomain.BaseDirectory + "Site\\");

                // Rest
                GetHandler restGetHandler = new RestGet();
                webserver.RestGetHandler = restGetHandler;
                PostHandler restPostHandler = new RestPost();
                webserver.RestPostHandler = restPostHandler;

                // WebSockets
                WSSrv wssrv = new WSSrv();
                webserver.ServerWS = wssrv;

                webserver.Run();
            }
            catch (Exception ex) { }
        }
    }
}
```

REST GET handler

```csharp
using Noos.Web.WebServer.Core;
using Noos.Web.WebServer.RestContext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Noos.SampleServerConsole
{
    internal class RestGet : GetHandler
    {
        private Request req;
        private int StatusCode;
        private string StatusDescription;
        private byte[] Content;
        private string ContentType;

        public override void ProcessRequest(Request req, string body, ref Response res)
        {
            this.req = req;
            string[] parts = req.Endpoint.Split('?')[1].Split('=');

            if (string.Equals(parts[0], "TOKEN", StringComparison.OrdinalIgnoreCase)
                && parts[1] == "admin")
            {
                if (req.Endpoint.StartsWith("/API/USER",
                    StringComparison.OrdinalIgnoreCase)) UserArea();
                else NotFoundResource();
            }
            else
                NotAllowedResource();

            res.StatusCode = StatusCode;
            res.StatusDescription = StatusDescription;
            res.ContentType = ContentType;
            res.Content = Content;
        }

        private void UserArea()
        {
            string result = "";

            if (req.Endpoint.StartsWith("/API/USER/LIST",
                StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    result = "User list";                    
                    StatusCode = 200;
                    StatusDescription = "OK";
                }
                catch (Exception ex)
                {
                    StatusCode = 500;
                    StatusDescription = "Internal server error";
                }
                Content = Encoding.UTF8.GetBytes(result);
                ContentType = "application/json";
            }
            else NotFoundResource();
        }

        private void NotFoundResource()
        {
            StatusCode = 404;
            StatusDescription = "Not Found";
            Content = Encoding.UTF8.GetBytes("Resource not found");
            ContentType = "text/html";
        }

        private void NotAllowedResource()
        {
            StatusCode = 405;
            StatusDescription = "Not allowed";
            Content = Encoding.UTF8.GetBytes("Resource not allowed");
            ContentType = "text/html";
        }
    }
}
```

REST POST handler

```csharp
using Noos.Web.WebServer.Core;
using Noos.Web.WebServer.RestContext;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace Noos.SampleServerConsole
{
    internal class RestPost : PostHandler
    {
        private Request req;
        private string body;
        private int StatusCode;
        private string StatusDescription;
        private byte[] Content;
        private string ContentType;

        public override void ProcessRequest(Request req, string body, ref Response res)
        {
            this.req = req;
            this.body = body;
            string[] parts = req.Endpoint.Split('?')[1].Split('=');

            if (string.Equals(parts[0], "TOKEN", 
                     StringComparison.OrdinalIgnoreCase)
                && parts[1] == "admin")
            {
                if (req.Endpoint.StartsWith("/API/USER",
                    StringComparison.OrdinalIgnoreCase)) UserArea();
                else NotFoundResource();
            }
            else
                NotAllowedResource();

            res.StatusCode = StatusCode;
            res.StatusDescription = StatusDescription;
            res.ContentType = ContentType;
            res.Content = Content;
        }

        private void UserArea()
        {
            if (req.Endpoint.StartsWith("/API/USER/CREATE",
                StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    string postbody = body;
                    
                    StatusCode = 201;
                    StatusDescription = "Created";
                }
                catch (Exception ex)
                {
                    StatusCode = 500;
                    StatusDescription = "Internal server error";
                }
                item.Write(writer);
                Content = Encoding.UTF8.GetBytes(writer.ToString());
                ContentType = "application/json";
            }
            else NotFoundResource();
        }

        private void NotFoundResource()
        {
            StatusCode = 404;
            StatusDescription = "Not Found";
            Content = Encoding.UTF8.GetBytes("Resource not found");
            ContentType = "text/html";
        }

        private void NotAllowedResource()
        {
            StatusCode = 405;
            StatusDescription = "Not allowed";
            Content = Encoding.UTF8.GetBytes("Resource not allowed");
            ContentType = "text/html";
        }
    }
}
```

WebSockets handler

```csharp
using Noos.Web.WebServer.WebSocketsContext;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Noos.SampleServerConsole
{
    internal class WSSrv : WSServer
    {
        public override async Task<bool> ProcessRequest(HttpListenerContext httpListenerContext)
        {
            WebSocketContext webSocketContext = null;
            try
            {
                webSocketContext = await 
                  httpListenerContext.AcceptWebSocketAsync(subProtocol: null);
                string ipAddress =
                  httpListenerContext.Request.RemoteEndPoint.Address.ToString();
                Console.WriteLine("Connected: IPAddress {0}", ipAddress);
            }
            catch (Exception e)
            {
                httpListenerContext.Response.StatusCode = 500;
                httpListenerContext.Response.Close();
                Console.WriteLine("Exception: {0}", e);
                return false;
            }

            WebSocket webSocket = webSocketContext.WebSocket;
            try
            {
                byte[] receiveBuffer = new byte[1024];
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult receiveResult =
                        await webSocket.ReceiveAsync(new
                          ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                         "", CancellationToken.None);
                    else
                    {
                        await webSocket.SendAsync(new ArraySegment<byte>(receiveBuffer,
                                                  0, receiveResult.Count),
                            WebSocketMessageType.Binary, receiveResult.EndOfMessage,
                                  CancellationToken.None);
                        Console.WriteLine("Received message");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
                return false;
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
            }
            return true;
        }
    }
}
```