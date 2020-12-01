using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NREAL.AR.Async
{
    // https://github.com/gonglei007/CsAsync
    public class Waterfall
    {
        public delegate void TaskCallback(Exception e);

        private Queue<Action<TaskCallback>> m_Tasks = new Queue<Action<TaskCallback>>();

        private TaskCallback m_ResultCallback;

        public void AddTask(Action<TaskCallback> task)
        {
            m_Tasks.Enqueue(task);
        }

        public void Start(TaskCallback resultCallback)
        {
            m_ResultCallback = resultCallback;

            if (m_Tasks.Count == 0)
            {
                m_ResultCallback.Invoke(null);
                return;
            }

            Next();
        }

        private void Next()
        {
            Action<TaskCallback> task = m_Tasks.Dequeue();
            task.Invoke(OnTaskCompleted);
        }

        private void OnTaskCompleted(Exception e)
        {
            if (m_Tasks.Count == 0 || e != null)
            {
                this.m_ResultCallback.Invoke(e);
                return;
            }

            Next();
        }
    }
}