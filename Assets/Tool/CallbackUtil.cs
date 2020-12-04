using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CallbackUtil : MonoBehaviour
{
	private static CallbackUtil instance;

	private static GameObject obj;

	//the initiation of callback utility must be in a main tread and before any possible call on it.
	public static void Init()
	{
		if (instance == null)
		{
			obj = new GameObject();
			obj.name = "CallbackFunctionalityObject";
			obj.SetActive(true);
			DontDestroyOnLoad(obj);
			instance = obj.AddComponent<CallbackUtil>();
		}
	}
	public static CallbackUtil Instance
	{
		get
		{
			return instance;
		}
	}
	
	private abstract class ActionBase
	{
		//internal bool called = false;
		public bool callOnce = true;
		public abstract void exec();
		public abstract void Dispose();

	}
	private class ActionInfo<T> : ActionBase
	{
		public T callback;
		public object[] param;

		public override void exec()
		{
			try
			{
                if (callback != null)
                {
				    (callback as System.Delegate).DynamicInvoke(param);
                }
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public override void Dispose()
		{
			callback = default(T);
			param = null;
		}
	}

	List<ActionBase> list = new List<ActionBase>();
	List<ActionBase> oncelist = new List<ActionBase>();

	void Update()
	{
		lock (oncelist) {
			var arr = oncelist.ToArray ();
			oncelist.Clear ();
			foreach (var item in arr) {
				item.exec ();
				item.Dispose ();
			}
		}
		lock (list)
		{
//			List<ActionBase> done = null;
			var arr = list.ToArray();
//			for (int i = list.Count - 1; i >= 0; i--)
			for (int i = 0; i < arr.Length; i++)
			{
				ActionBase item = arr[i];
				try
				{
					item.exec();
				}
				catch (Exception e)
				{
					Debug.LogException(e);
				}
//				if (item.callOnce)
//				{
//					if (done == null)
//					{
//						done = new List<ActionBase>();
//					}
//					done.Add(item);
//				}
			}
//			foreach (ActionBase item in list)
//			{
//				try
//				{
//					item.exec();
//				}
//				catch (Exception e)
//				{
//					DLog.LogException(e);
//				}
//				if (item.callOnce)
//				{
//					if (done == null)
//					{
//						done = new List<ActionBase>();
//					}
//					done.Add(item);
//				}
//			}
//			if (done != null)
//			{
//				foreach (ActionBase item in done)
//				{
//					item.Dispose();
//					list.Remove(item);
//				}
//				done.Clear();
//				done = null;
//			}
		}
	}

    void FixedUpdate()
    {
        lock (fixedCallbacks)
        {
            for (int i = 0; i < fixedCallbacks.Count; i++)
            {
                try
                {
                    fixedCallbacks[i]();

                } catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }

	public void UnityCall<T>(T callback, bool callOnce = true, params object[] param)
	{
		ActionInfo<T> actionInfo = new ActionInfo<T>();
		actionInfo.callback = callback;
		actionInfo.callOnce = callOnce;
		actionInfo.param = param;
		if (callOnce) {
			lock (oncelist) {
				oncelist.Add (actionInfo);	
			}
		} else {
			lock (list)
			{

				list.Add(actionInfo);
			}
		}
	}

    public void RemoveUnityCall<T>(T callback)
    {
        lock (list)
        {
//            for (int i = 0; i < list.Count; i++)
//            {
//                ActionInfo<T> info;
//                if (list[i] is ActionInfo<T>)
//                {
//                    info = (ActionInfo<T>)list[i];
//                }
//                else
//                {
//                    continue;
//                }
//
//                if (info.callback.Equals(callback))
//                {
//                    list.Remove(info);
//                }
//            }
			list.RemoveAll((ActionBase item) => { 
				if (item is ActionInfo<T>) {
					var info = (ActionInfo<T>)item;
					return info.callback.Equals(callback); 
				}
				return false;
			});
        }
    }


    private List<Action> fixedCallbacks = new List<Action>();
    
    public void AddFixedUpdate(Action callback)
    {
        lock (fixedCallbacks)
        {
            if (fixedCallbacks.Contains(callback))
            {
                return;
            }
            fixedCallbacks.Add(callback);
        }
    }
    
    public void RemoveFixedUpdate(Action callback)
    {
        lock (fixedCallbacks)
        {
            fixedCallbacks.Remove(callback);
        }
    }


}