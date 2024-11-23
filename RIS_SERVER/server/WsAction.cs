using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.server
{
    public delegate object Action(ClientRequest req);

    public class WsAction
    {
        public Action Action { get; set; }
        public string Path { get; set; }

        public List<Middleware> Middlewares { get; set; } = new List<Middleware>();

        public WsAction(string path, Action action, List<Middleware> middlewares)
        {
            Path = path;
            Action = action;
            Middlewares = middlewares;
        }

        public WsAction(string path, Action action)
        {
            Path = path;
            Action = action;
            Middlewares = [];
        }

        public object Run(ClientRequest clientRequest)
        {
            foreach (var middleware in Middlewares)
            {
                middleware.Run(clientRequest);
            }

            return Action(clientRequest);
        }
    }
}
