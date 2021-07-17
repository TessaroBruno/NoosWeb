using System.Net;

namespace Noos.Web.WebServer.Core
{
    internal static class Methods
    {
        public const string Get = WebRequestMethods.Http.Get;
        public const string Post = WebRequestMethods.Http.Post;
        public const string Put = WebRequestMethods.Http.Put;
        public const string Delete = "DELETE";
    }
}