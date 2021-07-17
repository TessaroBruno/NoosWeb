using Noos.Web.WebServer.Core;
using System.Text;

namespace Noos.Web.WebServer
{
    public class WebHandlersManager
    {
        public RestContext.GetHandler RestGetHandler { get; set; }

        public RestContext.PostHandler RestPostHandler { get; set; }

        public RestContext.PutHandler RestPutHandler { get; set; }

        public RestContext.DeleteHandler RestDeleteHandler { get; set; }

        public string Port { get; set; }

        public string SitePath { get; set; }

        public WebSocketsContext.WSServer ServerWS { get; set; }

        public WebHandlersManager(WebSocketsContext.WSServer serverws,
            string port = "8000", string sitepath = "Site\\")
        {
            Port = port;
            SitePath = sitepath;
            RestGetHandler = new RestContext.GetHandler();
            RestPostHandler = new RestContext.PostHandler();
            RestPutHandler = new RestContext.PutHandler();
            RestDeleteHandler = new RestContext.DeleteHandler();
            if (serverws == null) ServerWS = new WebSocketsContext.WSServer();
        }

        public void Run()
        {
            Application app = new Application(ServerWS);

            // Static handlers
            StaticContext.Server staticsrv = new StaticContext.Server(SitePath);
            app.Get("/ui/", async (req, res) =>
            {
                staticsrv.ProcessRequest(req, ref res);
                await res.SendAsync();
            });

            // ReST handlers
            RestContext.GetHandler gethdl = RestGetHandler;
            app.Get("/api/", async (req, res) =>
            {
                gethdl.ProcessRequest(req, await req.GetBodyAsync(), ref res);
                await res.SendAsync();
            });

            RestContext.PostHandler posthdl = RestPostHandler;
            app.Post("/api/", async (req, res) =>
            {
                posthdl.ProcessRequest(req, await req.GetBodyAsync(), ref res);
                await res.SendAsync();
            });

            RestContext.PutHandler puthdl = RestPutHandler;
            app.Put("/api/", async (req, res) =>
            {
                puthdl.ProcessRequest(req, await req.GetBodyAsync(), ref res);
                await res.SendAsync();
            });

            RestContext.DeleteHandler delhdl = RestDeleteHandler;
            app.Delete("/api/", async (req, res) =>
            {
                delhdl.ProcessRequest(req, await req.GetBodyAsync(), ref res);
                await res.SendAsync();
            });

            // Default handlers
            app.Get("notfound", async (req, res) =>
            {
                res.StatusCode = 404;
                res.StatusDescription = "Not Found";
                res.Content = Encoding.UTF8.GetBytes("Resource not found");
                res.ContentType = "text/html";
                await res.SendAsync();
            });

            app.Post("notfound", async (req, res) =>
            {
                res.StatusCode = 404;
                res.StatusDescription = "Not Found";
                res.Content = Encoding.UTF8.GetBytes("Resource not found");
                res.ContentType = "text/html";
                await res.SendAsync();
            });

            app.Put("notfound", async (req, res) =>
            {
                res.StatusCode = 404;
                res.StatusDescription = "Not Found";
                res.Content = Encoding.UTF8.GetBytes("Resource not found");
                res.ContentType = "text/html";
                await res.SendAsync();
            });

            app.Delete("notfound", async (req, res) =>
            {
                res.StatusCode = 404;
                res.StatusDescription = "Not Found";
                res.Content = Encoding.UTF8.GetBytes("Resource not found");
                res.ContentType = "text/html";
                await res.SendAsync();
            });

            app.Start(Port);
        }
    }
}