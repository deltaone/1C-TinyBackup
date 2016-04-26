using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core
{
    public delegate void MessageEventHandler(ref object arg);

    public class MessageManager
    {
        private struct MessageHandler 
        {
            public MessageEventHandler handler { get; set; }
            public bool persistent { get; set; }
        }

        private struct MessageEvent
        {
            public string tag  { get; set; }
            public string message { get; set; }
            public object arg { get; set; }
            public TimeSpan delay { get; set; }
            public DateTime time { get; set; }
            public long runsMax { get; set; }
            public long runs { get; set; }
        }

        private Dictionary<string, int> _statistics { get; set; }
        private Dictionary<string, List<MessageHandler>> _messages { get; set; }        
        private List<MessageEvent> _events { get; set; }

        //---------------------------------------------------------------------
        public MessageManager()
        {
            Reset(true);            
        }

        public void Reset(bool fullReset = true)
        {
            _statistics = new Dictionary<string, int>();

            if (fullReset)
            {
                _messages = new Dictionary<string, List<MessageHandler>>();
                _events = new List<MessageEvent>();
                return;
            }

            foreach (var handlers in _messages)
                for (int i = 0; i < handlers.Value.Count; i++)
                    if (handlers.Value[i].persistent == false) handlers.Value.RemoveAt(i--);
        }

        public void Subscribe(string message, MessageEventHandler handler, bool persistent = false)
        {
            if (!_messages.ContainsKey(message))
            {
                _messages.Add(message, new List<MessageHandler>());
                _statistics.Add(message, 0);
            }
            var subscribers = _messages[message];
            foreach (var s in subscribers)
            {
                if (s.handler == handler)
                {
                    GM.Warning(String.Format("Subscriber already added <{0}> !", handler.Method), true);
                    return;
                }
            }
            subscribers.Add(new MessageHandler() { handler = handler, persistent = persistent } );
        }

        public void Unsubscribe(string message)
        {
            if (_messages.ContainsKey(message)) _messages.Remove(message);
            else GM.Warning(String.Format("Try to unsubscribe from nonexistent message '{0}'!", message), true);
        }

        public void Unsubscribe(string message, MessageEventHandler handler)
        {
            if (!_messages.ContainsKey(message))
            {
                GM.Warning(String.Format("Try to unsubscribe from nonexistent message '{0}'!", message), true);
                return;                
            }

            var subscribers = _messages[message];
            for (var i = 0; i < subscribers.Count; i++)
            {
                if (subscribers[i].handler != handler) continue;
                subscribers.RemoveAt(i);
                if (subscribers.Count == 0) _messages.Remove(message);
                return;
            }
            GM.Warning(String.Format("Try to remove nonexistent handler '{0}' from '{1}'!", handler.Method, message), true);
        }

        public void Broadcast(string message, object arg = null)
        {
            if (!_messages.ContainsKey(message))
            {
                GM.Warning(String.Format("Try to broadcast nonexistent message '{0}'!", message), true);
                return;
            }

            _statistics[message] = _statistics[message] + 1;
            try
            {
                foreach (var e in _messages[message]) ((MessageEventHandler) e.handler)(ref arg);
            }
            catch(Exception ex)
            {
                GM.Warning(String.Format("Exception on message '{0}'!", message), true);
                GM.Log(String.Format("Exception: {0}\n{1}", ex.Message, ex.StackTrace));
            }            
        }

        public void EventRemoveByTag(string tag)
        {
            for (int i = 0; i < _events.Count; i++)
            {
                if (!String.Equals(_events[i].tag, tag, StringComparison.InvariantCultureIgnoreCase)) continue;
                _events.RemoveAt(i--);
            }      
        }

        public void EventRemoveByMessage(string message)
        {
            for (int i = 0; i < _events.Count; i++)
            {
                if (!String.Equals(_events[i].message, message, StringComparison.InvariantCultureIgnoreCase)) continue;
                _events.RemoveAt(i--);
            }      
        }

        public void EventAdd(string message, object arg, TimeSpan delay, string tag = "", int runs = 0)        
        {
            // tag message arg delay time runsMax runs
            _events.Add(new MessageEvent()
            {
                tag = tag,
                message = message,
                arg = arg,
                delay = delay,
                time = DateTime.Now,
                runsMax = runs,
                runs = 0,
            });
        }

        public void EventAddDelayed(string message, object arg, TimeSpan delay)
        {
            EventAdd(message, arg, delay, "", 1);
        }

        public void EventEnqueue(string message, object arg)
        {
            EventAdd(message, arg, TimeSpan.FromMilliseconds(0), "", 1);
        }

        public void Update()
        {
            for (int i = 0; i < _events.Count; i++)
            {                
                if (_events[i].time + _events[i].delay >= DateTime.Now) continue;
                Broadcast(_events[i].message, _events[i].arg);
                var e = _events[i];
                e.time = DateTime.Now; 
                _events[i] = e;
                if (_events[i].runsMax == 0) continue;
                e.runs++;
                _events[i] = e;
                if (_events[i].runs < _events[i].runsMax) continue;
                _events.RemoveAt(i--);
            }                
        }

        public void Dump_()
        {
            
        }
    }
}