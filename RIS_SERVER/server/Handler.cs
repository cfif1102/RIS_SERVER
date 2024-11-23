using RIS_SERVER.src.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.server
{
    public class Handler
    {
        private readonly List<WsAction> _actions = new List<WsAction>();

        public void Add(string path, Action action, List<Middleware> middlewares)
        {
            foreach (var act in _actions)
            {
                if (act.Path == path)
                {
                    throw new Exception("Action is already defined...");
                }
            }

            _actions.Add(new WsAction(path, action, middlewares));
        }

        public object Run(ClientRequest request)
        {
            foreach (var action in _actions)
            {
                if (action.Path == request.Action)
                {
                    return action.Run(request);
                }
            }

            throw new WsException(404, "No action found...");
        }
    }
}
