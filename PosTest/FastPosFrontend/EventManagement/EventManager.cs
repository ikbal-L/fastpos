using FastPosFrontend.sse;
using Netina.Stomp.Client;
using Newtonsoft.Json;
using ServiceInterface.Authorisation;
using ServiceInterface.Model;
using ServiceLib.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastPosFrontend.EventManagement
{
   

    public class EventManager
    {
        private readonly StompClient _client;
        private readonly Dictionary<string, string> _headers;
        private readonly Dictionary<string,Action<object>> _eventHandlers;



        public EventManager()
        {
            _eventHandlers = new Dictionary<string, Action<object>>();
            _client = new StompClient("ws://localhost:8080/websocket", reconnectTimeOut: TimeSpan.FromSeconds(60*60));
            _headers = new Dictionary<string, string>() {
                {"Authorization",AuthProvider.Instance.AuthorizationToken },
                {"SessionId",RestAuthentification.SessionId },
            };
            _client.OnError += _client_OnError;
            _client.OnClose += _client_OnClose;


        }

        public StompClient Client => _client;

        private void _client_OnClose(object sender, Websocket.Client.DisconnectionInfo e)
        {
     
        }

        private void _client_OnError(object sender, string e)
        {

        }

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync(_headers);
        }

        public async Task ListenAsync<E>(string channel,bool receiveFullMessage = false)  where E:IMessage
        {
            await _client.SubscribeAsync<E>(channel, new Dictionary<string,string>(_headers), ((s, message) =>
            {
                if (_eventHandlers.ContainsKey(message.Type)&& message.Source != Thread.CurrentPrincipal.Identity.Name)
                {
                    var onEvent = _eventHandlers[message.Type];
                    if (receiveFullMessage)
                    {
                        onEvent(message);
                        return;
                    }
                    onEvent(message.Content);
                }
            }));
        }

       


        public async Task PublishAsync<T>(string eventType, T data)
        {
            var _event = new Message<T>()
            {
                Source = Thread.CurrentPrincipal.Identity.Name,
                Type = eventType,
                Content = data
            };
            await _client.SendAsync(JsonConvert.SerializeObject(_event), "/app/chat", new Dictionary<string, string>(_headers));
        }

        public void Publish<T>(string eventType, T data)
        {
            Task.Run(async () => await PublishAsync(eventType, data));
        }

        public void OnEvent<T>(string name , Action<T> action)
        {
            _eventHandlers.Add(name, (o)=> {
                if (o is T t) action(t);
            });
        }

        public void OnEvent<T>(string name, Action<IMessage<T>> action)
        {
            _eventHandlers.Add(name, (o) => {
                if (o is IMessage<T> d) action(d);
            });
        }

    }


    public class EventSubscribtion
    {
        public string EventType { get; set; }

        public string Source { get; set; }

        public Action<object> OnEvent { get; set; }

        public EventSubscribtion(string eventType, string source, Action<object> onEvent)
        {
            EventType = eventType;
            Source = source;
            OnEvent = onEvent;
        }

    }
}
