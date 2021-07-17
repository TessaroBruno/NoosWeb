using Noos.Web.WebServer.Core;
using System.Text;

namespace Noos.Web.WebServer.RestContext
{
    public class DeleteHandler
    {
        public virtual void ProcessRequest(Request req, string body, ref Response res)
        {
            res.StatusCode = 404;
            res.StatusDescription = "Not Found";
            res.ContentType = "text/html";
            res.Content = Encoding.UTF8.GetBytes("Resource not found");
        }
    }
}