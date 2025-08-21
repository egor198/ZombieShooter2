using ArtlessUI.Messaging.Core;
using System.Collections.Generic;
using System;

namespace ArtlessUI.Messaging
{
    public class Messenger
    {
        private readonly Dictionary<Type, IActionStore> actionStores = new Dictionary<Type, IActionStore>();

        public void Register<TMessage>(Action<TMessage> action) where TMessage : class, IMessage
        {
            var typeMessage = typeof(TMessage);
            ActionStore<TMessage> store = null;
            if (actionStores.ContainsKey(typeMessage) == false)
            {
                store = new ActionStore<TMessage>();
                actionStores.Add(typeMessage, store);
            }
            else
                store = actionStores[typeMessage] as ActionStore<TMessage>;

            store.Register(action);
        }

        public bool HasRegistered<TMessage>(Action<TMessage> action) where TMessage : class, IMessage
        {
            var typeMessage = typeof(TMessage);
            if (actionStores.ContainsKey(typeMessage) == false)
                return false;

            var store = actionStores[typeMessage] as ActionStore<TMessage>;
            return store.Contains(action);
        }

        public void Send<TMessage>(TMessage message) where TMessage : class, IMessage
        {
            var typeMessage = typeof(TMessage);
            if (actionStores.ContainsKey(typeMessage) == false)
            {
                UnityEngine.Debug.LogWarning("No one registered with this type of message");
                return;
            }

            var store = actionStores[typeMessage] as ActionStore<TMessage>;
            store.Send(message);
        }

        public void Unregister<TMessage>(Action<TMessage> action) where TMessage : class, IMessage
        {
            var typeMessage = typeof(TMessage);
            if (actionStores.ContainsKey(typeMessage) == false)
                return;

            var store = actionStores[typeMessage] as ActionStore<TMessage>;
            if (store.Contains(action) == false)
                return;
                
            store.Unregister(action);
            if (store.Count() == 0)
                actionStores.Remove(typeMessage);
        }
    }
}