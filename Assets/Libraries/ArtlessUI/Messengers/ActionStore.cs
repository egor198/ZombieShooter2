using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtlessUI.Messaging.Core
{
    public class ActionStore<TMessage> : IActionStore where TMessage : class, IMessage
    {
        private List<Action<TMessage>> actions = new List<Action<TMessage>>();

        public void Send(TMessage message)
        {
            foreach (var action in actions.ToList())
                action.Invoke(message);
        }

        public bool Contains(Action<TMessage> action)
        {
            return actions.Contains(action);
        }
        
        public void Register(Action<TMessage> action)
        {
            if (actions.Contains(action) == false)
                actions.Add(action);
        }

        public void Unregister(Action<TMessage> action)
        {
            actions.Remove(action);
        }

        public int Count()
        {
            return actions.Count;
        }
    }
}