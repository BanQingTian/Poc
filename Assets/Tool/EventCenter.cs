using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*
 *
 *  Author: Xingguang Yu
 *
 **/
[ExecuteInEditMode]
public class EventCenter : MonoBehaviour
{
    static EventCenter instance;

    public static EventCenter Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject hostObj = MonoDelegate.GetHiddenNonDestroyGameObject();
                instance = hostObj.GetComponent<EventCenter>();
                if (instance == null)
                {
                    instance = hostObj.AddComponent<EventCenter>();
                }

            }
            return instance;
        }
    }

    public class Param<T> : Dictionary<string, T>
    {

    }

    public class Args : EventArgs
    {
        public string eventName;
        public object param;
        public Args(string eventName, object param = null)
        {
            this.eventName = eventName;
            this.param = param;
        }
    }

    public delegate void Handler(object sender, Args args);

    Dictionary<string, Handler> eventHandlers = new Dictionary<string, Handler>();

    private Dictionary<string, Handler> EventHandlers
    {
        get { return eventHandlers; }
    }
    /// <summary>
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="eventHandler"></param>
    /// <param name="onlyHaveOneThisEventName">只是希望同样名字的事件只有一个【zzy+ 7/4/2016】</param>
	public void AddEventListener(string eventName, Handler eventHandler, bool onlyHaveOneThisEventName = false)
    {
        if (eventHandlers.ContainsKey(eventName) && (!onlyHaveOneThisEventName))
        {
            eventHandlers[eventName] += eventHandler;
        }
        else
        {
            eventHandlers[eventName] = eventHandler;
        }
    }

    public void RemoveEventListener(string eventName, Handler eventHandler)
    {
        if (!eventHandlers.ContainsKey(eventName))
        {
            return;
        }
        eventHandlers[eventName] -= eventHandler;
        if (eventHandlers[eventName] == null)
        {
            eventHandlers.Remove(eventName);
        }
    }

    public void RemoveAllEventListeners(string eventName)
    {
        if (!eventHandlers.ContainsKey(eventName))
        {
            return;
        }
        Handler handlers = eventHandlers[eventName];
        while (handlers != null && handlers.GetInvocationList().GetLength(0) > 0)
        {
            Handler eventHandler = (Handler)handlers.GetInvocationList()[0];
            //handlers -= eventHandler;
            RemoveEventListener(eventName, eventHandler);
        }
    }

    public void DispatchEvent(string eventName, object sender = null, object param = null)
    {
        if (!eventHandlers.ContainsKey(eventName))
        {
            Debug.LogWarning("[EventCenter] hasn't the key:" + eventName);
            return;
        }

        Handler handlers = eventHandlers[eventName];
        if (handlers == null)
        {
            eventHandlers.Remove(eventName);
            return;
        }
        for (int i = 0; i < handlers.GetInvocationList().GetLength(0); i++)
        {
            Handler eventHandler = (Handler)handlers.GetInvocationList()[i];
            try
            {
                eventHandler(sender, new Args(eventName, param));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

    }

    internal void DispatchEvent(object event_Tutoria_Open)
    {
        throw new NotImplementedException();
    }

    void OnDestroy()
    {
        //clean();
    }

    void clean()
    {
        foreach (string eventName in eventHandlers.Keys)
        {
            RemoveAllEventListeners(eventName);
        }
        eventHandlers.Clear();
    }

}