using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Noos.Web.WebServer.Core
{
    public class Request
    {
        private HttpListenerRequest httpRequest;
        private string body;
        public Request(HttpListenerRequest httpRequest) => this.httpRequest = httpRequest;
        public string Endpoint => httpRequest.RawUrl;
        public string Method => httpRequest.HttpMethod;

        public async Task<string> GetBodyAsync()
        {
            if (Method == Methods.Get || !httpRequest.HasEntityBody)
                return null;

            if (body == null)
            {
                byte[] buffer = new byte[httpRequest.ContentLength64];
                using (Stream inputStream = httpRequest.InputStream)
                    await inputStream.ReadAsync(buffer, 0, buffer.Length);

                body = Encoding.UTF8.GetString(buffer);
            }
            return body;
        }
    }
}