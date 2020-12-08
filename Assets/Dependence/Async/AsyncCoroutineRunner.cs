using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NREAL.AR.Utility
{
    public class AsyncCoroutineRunner : MonoBehaviour
    {
        static AsyncCoroutineRunner _instance;

        public static AsyncCoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("AsyncCoroutineRunner")
                        .AddComponent<AsyncCoroutineRunner>();
                }

                return _instance;
            }
        }

        void Awake()
        {
            // Don't show in scene hierarchy
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            DontDestroyOnLoad(gameObject);
        }

        enum TCoroutineState
        {
            NONE,
            PROGRESSING,
            DONE
        }

        static Dictionary<string, TCoroutineState> s_CoroutineStates = new Dictionary<string, TCoroutineState>();

        static object s_Sync = new object();

        public static IEnumerator StartCoroutineOnce(IEnumerator routine, string onceKey)
        {
            TCoroutineState state = GetCoroutineState(onceKey);

            if (state == TCoroutineState.NONE)
            {
                SetCoroutineState(onceKey, TCoroutineState.PROGRESSING);
                yield return Instance.StartCoroutine(routine);
                SetCoroutineState(onceKey, TCoroutineState.DONE);
            }

            while (true)
            {
                if (GetCoroutineState(onceKey) == TCoroutineState.DONE)
                    break;

                yield return null;
            }

        }

        private static void SetCoroutineState(string onceKey, TCoroutineState state)
        {
            lock (s_Sync)
            {
                s_CoroutineStates[onceKey] = state;
            }
        }

        private static TCoroutineState GetCoroutineState(string onceKey)
        {
            lock (s_Sync)
            {
                TCoroutineState state = TCoroutineState.NONE;
                if (s_CoroutineStates.TryGetValue(onceKey, out state))
                {
                    return state;
                }
                return TCoroutineState.NONE;
            }
        }
    }
}
