using System;
using System.Collections.Generic;

using UnityEngine;

/*
 *
 *  Author: Xingguang Yu
 *
 **/
public class MonoDelegate
{
	const string OBJ_NAME = "__HIDDEN_GAMEOBJECT__";
	static GameObject hiddenNonDestroyGameObject;

	public static GameObject GetHiddenNonDestroyGameObject()
	{
		if (hiddenNonDestroyGameObject == null)
		{
			hiddenNonDestroyGameObject = GameObject.Find(OBJ_NAME);
			if (hiddenNonDestroyGameObject != null)
			{
				//Destroy old one in scene.
				GameObject.Destroy(hiddenNonDestroyGameObject);
			}
			hiddenNonDestroyGameObject = new GameObject(OBJ_NAME);

			//Hide and don't destroy on load.
			hiddenNonDestroyGameObject.hideFlags = HideFlags.HideAndDontSave;
			//hiddenNonDestroyGameObject.hideFlags = HideFlags.None;
			GameObject.DontDestroyOnLoad(hiddenNonDestroyGameObject);
		}

		return hiddenNonDestroyGameObject;
	}

	public static void DestroyHiddeObject()
	{
		GameObject.Destroy(hiddenNonDestroyGameObject);
	}

	public static void AddUpdateCallback(Action callback)
	{
		CallbackUtil.Instance.UnityCall(callback, false);
	}

	public static void RemoveUpdateCallback(Action callback)
	{
//		CallbackUtil.Instance.RemoveUnityCall(callback);
        CallbackUtil.Instance.RemoveUnityCall<Action>(callback);
	}

    public static void AddFixedUpdateCallback(Action callback)
    {
        CallbackUtil.Instance.AddFixedUpdate(callback);
    }

    public static void RemoveFixedUdpateCallback(Action callback)
    {
        CallbackUtil.Instance.RemoveFixedUpdate(callback);
    }

}
