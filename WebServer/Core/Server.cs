using System.Threading.Tasks;

namespace Noos.Web.WebServer.Core
{
    internal class Server
    {
        private Listener listener;
        public RouteRepository RouteRepository { get; }

        public Server(Listener listener, RouteRepository routeRepository)
        {
            this.listener = listener;
            RouteRepository = routeRepository;
        }

        public async Task StartAsync(string port)
        {
            // Console.WriteLine("Initialising server on port {0}...", port);
            await listener.StartAsync(port, RouteRepository);
        }
    }
}