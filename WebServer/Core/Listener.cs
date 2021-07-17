using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Noos.Web.WebServer.Core
{
    internal class Listener
    {
        private HttpListener httpListener;
        private HttpListenerContext context;
        private WebSocketsContext.WSServer wssrv;

        public Listener(WebSocketsContext.WSServer serverws)
        {
            httpListener = new HttpListener();
            wssrv = serverws;
        }

        public async Task StartAsync(string port, RouteRepository routeRepository)
        {
            httpListener.Prefixes.Add(string.Format("http://*:{0}/", port));
            httpListener.Start();
            // Console.WriteLine("Listening for requests on port {0}.", port);
            Request request = await GetNextRequestAsync();
            while (request != null)
            {
                //Console.WriteLine("{0}: {1} {2}", DateTime.Now, request.Method, request.Endpoint);
                await TryRespondAsync(request, routeRepository);
                request = await GetNextRequestAsync();
            }
        }

        private async Task<bool> TryRespondAsync(Request request, RouteRepository routeRepository)
        {
            string currRoute = "notfound";
            Dictionary<string, Action<Request, Response>> routes = routeRepository.GetRoutes(request.Method);
            if (routes == null) return false;
            foreach (string route in routes.Keys)
                if (request.Endpoint.StartsWith(route, StringComparison.OrdinalIgnoreCase)) currRoute = route;
            await Task.Run(() => routes[currRoute](request, new Response(context.Response)));
            return true;
        }

        private async Task<Request> GetNextRequestAsync()
        {
            try
            {
                context = await httpListener.GetContextAsync();
                // WebSockets check
                if (context.Request.IsWebSocketRequest)
                {
                    await wssrv.ProcessRequest(context);
                    return null;
                }
                else
                {
                    HttpListenerRequest httpRequest = context.Request;
                    return new Request(httpRequest);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}