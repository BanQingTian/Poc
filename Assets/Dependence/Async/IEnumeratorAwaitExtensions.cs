using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace NREAL.AR.Async
{
    using AsyncCoroutineRunner = NREAL.AR.Utility.AsyncCoroutineRunner;

    public static class IEnumeratorAwaitExtensions
    {
        public static CoroutineAwaiter GetAwaiter(this WaitForSeconds instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static CoroutineAwaiter GetAwaiter(this WaitForUpdate instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static CoroutineAwaiter GetAwaiter(this WaitForEndOfFrame instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static CoroutineAwaiter GetAwaiter(this WaitForFixedUpdate instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static CoroutineAwaiter GetAwaiter(this WaitForSecondsRealtime instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static CoroutineAwaiter GetAwaiter(this WaitUntil instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static CoroutineAwaiter GetAwaiter(this WaitWhile instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        public static CoroutineAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation instruction)
        {
            return GetAwaiterReturnSelf(instruction);
        }

        public static CoroutineAwaiter<UnityEngine.Object> GetAwaiter(this ResourceRequest instruction)
        {
            var awaiter = new CoroutineAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.ResourceRequest(awaiter, instruction)));
            return awaiter;
        }

        public static CoroutineAwaiter<WWW> GetAwaiter(this WWW instruction)
        {
            return GetAwaiterReturnSelf(instruction);
        }

        public static CoroutineAwaiter<AssetBundle> GetAwaiter(this AssetBundleCreateRequest instruction)
        {
            var awaiter = new CoroutineAwaiter<AssetBundle>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.AssetBundleCreateRequest(awaiter, instruction)));
            return awaiter;
        }

        public static CoroutineAwaiter<UnityEngine.Object> GetAwaiter(this AssetBundleRequest instruction)
        {
            var awaiter = new CoroutineAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.AssetBundleRequest(awaiter, instruction)));
            return awaiter;
        }

        public static CoroutineAwaiter<T> GetAwaiter<T>(this IEnumerator<T> coroutine)
        {
            var awaiter = new CoroutineAwaiter<T>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                new CoroutineWrapper<T>(coroutine, awaiter).Run()));
            return awaiter;
        }

        public static CoroutineAwaiter<object> GetAwaiter(this IEnumerator coroutine)
        {
            var awaiter = new CoroutineAwaiter<object>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                new CoroutineWrapper<object>(coroutine, awaiter).Run()));
            return awaiter;
        }

        internal static CoroutineAwaiter GetAwaiterReturnVoid(object instruction)
        {
            var awaiter = new CoroutineAwaiter();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.ReturnVoid(awaiter, instruction)));
            return awaiter;
        }

        public static CoroutineAwaiter<T> GetAwaiterReturnSelf<T>(T instruction)
        {
            var awaiter = new CoroutineAwaiter<T>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.ReturnSelf(awaiter, instruction)));
            return awaiter;
        }

        public static void RunOnUnityScheduler(Action action)
        {
            if (SynchronizationContext.Current == SyncContextUtil.UnitySynchronizationContext)
            {
                action();
            }
            else
            {
                SyncContextUtil.UnitySynchronizationContext.Post(_ => action(), null);
            }
        }

        internal static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new Exception("Assert hit in UnityAsyncUtil package!");
            }
        }

        internal class CoroutineWrapper<T>
        {
            readonly CoroutineAwaiter<T> m_Awaiter;

            readonly Stack<IEnumerator> m_ProcessStack;

            public CoroutineWrapper(IEnumerator coroutine, CoroutineAwaiter<T> awaiter)
            {
                m_ProcessStack = new Stack<IEnumerator>();
                m_ProcessStack.Push(coroutine);
                m_Awaiter = awaiter;
            }

            public IEnumerator Run()
            {
                while (true)
                {
                    var topWorker = m_ProcessStack.Peek();

                    bool isDone;

                    try
                    {
                        isDone = !topWorker.MoveNext();
                    }
                    catch (Exception e)
                    {
                        // The IEnumerators we have in the process stack do not tell us the
                        // actual names of the coroutine methods but it does tell us the objects
                        // that the IEnumerators are associated with, so we can at least try
                        // adding that to the exception output
                        var objectTrace = GenerateObjectTrace(m_ProcessStack);

                        if (objectTrace.Any())
                        {
                            m_Awaiter.Complete(
                                default(T), new Exception(
                                    GenerateObjectTraceMessage(objectTrace), e));
                        }
                        else
                        {
                            m_Awaiter.Complete(default(T), e);
                        }

                        yield break;
                    }

                    if (isDone)
                    {
                        m_ProcessStack.Pop();

                        if (m_ProcessStack.Count == 0)
                        {
                            m_Awaiter.Complete((T)topWorker.Current, null);
                            yield break;
                        }
                    }

                    // We could just yield return nested IEnumerator's here but we choose to do
                    // our own handling here so that we can catch exceptions in nested coroutines
                    // instead of just top level coroutine
                    if (topWorker.Current is IEnumerator)
                    {
                        m_ProcessStack.Push((IEnumerator)topWorker.Current);
                    }
                    else
                    {
                        // Return the current value to the unity engine so it can handle things like
                        // WaitForSeconds, WaitToEndOfFrame, etc.
                        yield return topWorker.Current;
                    }
                }
            }

            string GenerateObjectTraceMessage(List<Type> objTrace)
            {
                var result = new StringBuilder();

                foreach (var objType in objTrace)
                {
                    if (result.Length != 0)
                    {
                        result.Append(" -> ");
                    }

                    result.Append(objType.ToString());
                }

                result.AppendLine();
                return "Unity Coroutine Object Trace: " + result.ToString();
            }

            static List<Type> GenerateObjectTrace(IEnumerable<IEnumerator> enumerators)
            {
                var objTrace = new List<Type>();

                foreach (var enumerator in enumerators)
                {
                    // NOTE: This only works with scripting engine 4.6
                    // And could easily stop working with unity updates
                    var field = enumerator.GetType().GetField("$this", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (field == null)
                    {
                        continue;
                    }

                    var obj = field.GetValue(enumerator);
                    if (obj == null)
                    {
                        continue;
                    }

                    var objType = obj.GetType();
                    if (!objTrace.Any() || objType != objTrace.Last())
                    {
                        objTrace.Add(objType);
                    }
                }

                objTrace.Reverse();
                return objTrace;
            }
        }

        public class CoroutineAwaiter<T> : INotifyCompletion
        {
            private bool m_IsDone;

            private Exception m_Exception;

            private T m_Result;

            private Action m_Continuation;

            public bool IsCompleted => m_IsDone;

            public void Complete(T result, Exception e)
            {
                m_IsDone = true;
                m_Exception = e;
                m_Result = result;

                // Always trigger the continuation on the unity thread when awaiting on unity yield
                // instructions
                if (m_Continuation != null)
                {
                    RunOnUnityScheduler(m_Continuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                m_Continuation = continuation;
            }

            public T GetResult()
            {
                if (m_Exception != null)
                {
                    ExceptionDispatchInfo.Capture(m_Exception).Throw();
                }

                return m_Result;
            }
        }

        public class CoroutineAwaiter : INotifyCompletion
        {
            private bool m_IsDone;

            private Exception m_Exception;

            private Action m_Continuation;

            public bool IsCompleted => m_IsDone;

            public void GetResult()
            {
                if (m_Exception != null)
                {
                    ExceptionDispatchInfo.Capture(m_Exception).Throw();
                }
            }

            public void Complete(Exception e)
            {
                m_IsDone = true;
                m_Exception = e;

                // Always trigger the continuation on the unity thread when awaiting on unity yield
                // instructions
                if (m_Continuation != null)
                {
                    RunOnUnityScheduler(m_Continuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                m_Continuation = continuation;
            }
        }

        internal static class InstructionWrappers
        {
            public static IEnumerator ReturnVoid(CoroutineAwaiter awaiter, object instruction)
            {
                // For simple instructions we assume that they don't throw exceptions
                yield return instruction;
                awaiter.Complete(null);
            }

            public static IEnumerator AssetBundleCreateRequest(CoroutineAwaiter<AssetBundle> awaiter, AssetBundleCreateRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.assetBundle, null);
            }

            public static IEnumerator ReturnSelf<T>(CoroutineAwaiter<T> awaiter, T instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction, null);
            }

            public static IEnumerator AssetBundleRequest(CoroutineAwaiter<UnityEngine.Object> awaiter, AssetBundleRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.asset, null);
            }

            public static IEnumerator ResourceRequest(CoroutineAwaiter<UnityEngine.Object> awaiter, ResourceRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.asset, null);
            }
        }
    }
}
