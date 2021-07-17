using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Noos.Web.WebServer.Core
{
    public class Response
    {
        private HttpListenerResponse httpListenerResponse;
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }

        public Response(HttpListenerResponse httpListenerResponse)
        {
            this.httpListenerResponse = httpListenerResponse;
            StatusCode = 200;
            StatusDescription = "OK";
        }

        public async Task SendAsync()
        {
            httpListenerResponse.ContentType = ContentType;
            httpListenerResponse.StatusCode = StatusCode;
            httpListenerResponse.StatusDescription = StatusDescription;
            httpListenerResponse.AppendHeader("Access-Control-Allow-Origin", "*");
            if (httpListenerResponse.ContentLength64 == 0)
                httpListenerResponse.ContentLength64 = Content.Length;
            using (Stream output = httpListenerResponse.OutputStream)
                await output.WriteAsync(Content, 0, Content.Length);
            // Console.WriteLine("{0}: Responded with {1} bytes of data.", DateTime.Now, responseBuffer.Length);
            httpListenerResponse.Close();
        }
    }
}