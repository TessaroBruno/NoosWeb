using System;
using System.Threading;

namespace Noos.Web.WebServer.Core
{
    internal class Application
    {
        private Server server;

        public Application(WebSocketsContext.WSServer serverws)
        {
            server = new Server(new Listener(serverws), new RouteRepository());
        }

        public void Start(string port)
        {
            AutoResetEvent keepAlive = new AutoResetEvent(false);
            server.StartAsync(port);
            keepAlive.WaitOne();
        }

        public void Get(string endpoint, Action<Request, Response> handler)
        {
            server.RouteRepository.Get.Add(endpoint, handler);
        }

        public void Post(string endpoint, Action<Request, Response> handler)
        {
            server.RouteRepository.Post.Add(endpoint, handler);
        }

        public void Put(string endpoint, Action<Request, Response> handler)
        {
            server.RouteRepository.Put.Add(endpoint, handler);
        }

        public void Delete(string endpoint, Action<Request, Response> handler)
        {
            server.RouteRepository.Delete.Add(endpoint, handler);
        }
    }
}