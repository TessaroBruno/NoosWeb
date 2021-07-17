using System;
using System.Collections.Generic;

namespace Noos.Web.WebServer.Core
{
    internal class RouteRepository
    {
        public Dictionary<string, Action<Request, Response>> Get { get; }
        public Dictionary<string, Action<Request, Response>> Post { get; }
        public Dictionary<string, Action<Request, Response>> Put { get; }
        public Dictionary<string, Action<Request, Response>> Delete { get; }

        public RouteRepository()
        {
            Get = new Dictionary<string, Action<Request, Response>>();
            Post = new Dictionary<string, Action<Request, Response>>();
            Put = new Dictionary<string, Action<Request, Response>>();
            Delete = new Dictionary<string, Action<Request, Response>>();
        }

        public Dictionary<string, Action<Request, Response>> GetRoutes(string method)
        {
            switch (method)
            {
                case Methods.Get:
                    return Get;
                case Methods.Post:
                    return Post;
                case Methods.Put:
                    return Put;
                case Methods.Delete:
                    return Delete;
                default:
                    return null;
            }
        }
    }
}