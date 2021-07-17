using Noos.Web.WebServer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Noos.Web.WebServer.StaticContext
{
    internal class Server
    {
        private readonly string sitepath;

        private readonly string[] _indexFiles = {
            "index.html", "index.htm"
        };

        private static IDictionary<string, string> _mimeTypeMappings =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                { ".avi", "video/x-msvideo"},
                { ".css", "text/css"},
                { ".dll", "application/octet-stream"},
                { ".exe", "application/octet-stream"},
                { ".gif", "image/gif"},
                { ".htm", "text/html"},
                { ".html", "text/html"},
                { ".ico", "image/x-icon"},
                { ".iso", "application/octet-stream"},
                { ".jpeg", "image/jpeg"},
                { ".jpg", "image/jpeg"},
                { ".js", "application/x-javascript"},
                { ".json", "application/json"},
                { ".mov", "video/quicktime"},
                { ".mp3", "audio/mpeg"},
                { ".mpeg", "video/mpeg"},
                { ".mpg", "video/mpeg"},
                { ".msi", "application/octet-stream"},
                { ".pdf", "application/pdf"},
                { ".png", "image/png"},
                { ".rss", "text/xml"},
                { ".shtml", "text/html"},
                { ".svg", "image/svg+xml"},
                { ".ttf", "font/ttf"},
                { ".txt", "text/plain"},
                { ".xml", "text/xml"},
                { ".woff", "font/woff"},
                { ".woff2", "font/woff2"},
                { ".zip", "application/zip"},
            };

        public Server(string sitepath)
        {
            this.sitepath = sitepath;
        }

        public void ProcessRequest(Request req, ref Response res)
        {
            string filename = req.Endpoint;
            filename = filename.Substring(4); // Remove "/ui/" from URI

            if (string.IsNullOrEmpty(filename))
                foreach (string indexFile in _indexFiles)
                    if (File.Exists(Path.Combine(sitepath, indexFile)))
                    {
                        filename = indexFile;
                        break;
                    }

            filename = Path.Combine(sitepath, filename);
            if (File.Exists(filename))
            {
                try
                {
                    res.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename),
                                       out string mime) ? mime : "application/octet-stream";
                    // res.AddHeader("Last-Modified", File.GetLastWriteTime(filename).ToString("r"));
                    Stream input = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[input.Length];
                    input.Read(buffer, 0, Convert.ToInt32(input.Length));
                    input.Close();
                    res.Content = buffer;
                    res.StatusCode = 200;
                    res.StatusDescription = "OK";
                }
                catch (Exception)
                {
                    res.StatusCode = 500;
                    res.StatusDescription = "Internal Server Error";
                    res.Content = Encoding.UTF8.GetBytes("Internal Server Error");
                    res.ContentType = "text/html";
                }
            }
            else
            {
                res.StatusCode = 404;
                res.StatusDescription = "Not Found";
                res.Content = Encoding.UTF8.GetBytes("Resource not found");
                res.ContentType = "text/html";
            }
        }
    }
}