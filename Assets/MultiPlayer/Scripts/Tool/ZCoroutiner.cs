using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZCoroutiner : System.Object
{
    // Coroutiner Monobehaviour.
    private static MonoBehaviour coroutiner;

    /// <summary>
    /// 设置协程 MonoBehaviour.
    /// </summary>
    /// <param name="coroutiner">Coroutiner.</param>
    public static void SetCoroutiner(MonoBehaviour c)
    {
        if (coroutiner == null)
            coroutiner = c;
    }

    /// <summary>
    /// 启动一个协程.
    /// </summary>
    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        if (coroutiner == null)
        {
            GameObject obj = new GameObject("MainCoroutine");
            GameObject.DontDestroyOnLoad(obj);
        }

        return coroutiner == null ? null : coroutiner.StartCoroutine(routine);
    }

    /// <summary>
    /// 启动一个协程.
    /// </summary>
    public static Coroutine StartCoroutine(System.Action action)
    {
        return StartCoroutine(CommCoroutine(action));
    }

    private static IEnumerator CommCoroutine(System.Action action)
    {
        yield return null;
        action();
        //yield return null;
    }

    /// <summary>
    /// 停止一个协程.
    /// </summary>
    public static void StopCoroutine(Coroutine routine)
    {
        if (coroutiner == null) return;
        coroutiner.StopCoroutine(routine);
    }

    /// <summary>
    /// 停止所有协程.
    /// </summary>
    public static void StopAllCoroutines()
    {
        if (coroutiner == null) return;
        coroutiner.StopAllCoroutines();
    }

    ///////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 异步驱动器列表.
    /// </summary>
    private static List<ZCoroutineDriver> driverList = new List<ZCoroutineDriver>();
    private static List<ZCoroutineDriver> driverRemoveList = new List<ZCoroutineDriver>();

    private static bool isStartedWhile;

    /// <summary>
    /// 协程异步驱动器.
    /// </summary>
    public class ZCoroutineDriver
    {
        static long uid = 0;

        public short ExceType = 0;
        //public short EventType = 0;

        public string Name;

        public System.Action ProcEvent;

        //public MGTimerHandler ProcHandler = null;
        //执行次数.
        public int ExceFrames;
        //延迟执行帧数（秒）.
        public int DelayFrames;
        //执行时间（秒）.
        public float ExceTime;
        //延迟执行时间（秒）.
        public float DelayTime;

        public ZCoroutineDriver()
        {
            ++uid;
            Name = uid.ToString();
        }

        public ZCoroutineDriver(string name)
        {
            ++uid;
            Name = name + uid;
        }


        public void Exec()
        {
            if (ExceType == 0)
            {
                if (DelayFrames > 0)
                {
                    --DelayFrames;
                    return;
                }
                if (ExceFrames > 0)
                {
                    if (--ExceFrames <= 0) driverRemoveList.Add(this);
                }
            }
            else
            {
                if (DelayTime > 0)
                {
                    DelayTime -= Time.deltaTime;
                    return;
                }
                if (ExceTime > 0)
                {
                    ExceTime -= Time.deltaTime;
                    if (ExceTime <= 0) driverRemoveList.Add(this);
                }
            }

            if (ProcEvent != null)
                ProcEvent();
            else
                driverRemoveList.Add(this);

        }

    }

    /// <summary>
    /// 添加一个驱动器到列表中.
    /// </summary>
    /// <param name="timer"></param>
    public static string AddDriver(ZCoroutineDriver driver)
    {
        driverList.Add(driver);
        if (!isStartedWhile) StartCoroutine(WhileCoroutines());

        return driver.Name;
    }

    /// <summary>
    /// 按帧数定时 用协程驱动.
    /// </summary>
    public static string AddCoroutineFrames(System.Action action, int execCount = 1, int delayCount = 0, string name = "")
    {
        ZCoroutineDriver driver = new ZCoroutineDriver
        {
            Name = name,
            ProcEvent = action,
            ExceType = 0,
            ExceFrames = execCount,
            DelayFrames = delayCount,
        };

        driverList.Add(driver);
        if (!isStartedWhile) StartCoroutine(WhileCoroutines());

        return driver.Name;
    }

    /// <summary>
    /// 按时间定时 用协程驱动.
    /// </summary>
    public static string AddCoroutineTime(System.Action action, float exceTime = 0.0f, float delayTime = 0.0f, string name = "")
    {
        ZCoroutineDriver driver = new ZCoroutineDriver
        {
            Name = name,
            ProcEvent = action,
            ExceType = 1,
            ExceTime = exceTime,
            DelayTime = delayTime,
        };

        driverList.Add(driver);
        if (!isStartedWhile) StartCoroutine(WhileCoroutines());

        return driver.Name;
    }

    /// <summary>
    /// 从列表中删除一个计时器.
    /// </summary>
    /// <param name="timer"></param>
    public static void RemoveTimer(ZCoroutineDriver driver)
    {
        driverList.Remove(driver);
    }

    public static void RemoveTimer(string driverName)
    {
        for (int i = 0; i < driverList.Count; i++)
        {
            if (driverList[i].Name == driverName)
            {
                driverList.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// 清空所有
    /// </summary>.
    public static void ClearAll()
    {
        driverList.Clear();
        driverRemoveList.Clear();
    }

    //public static void OnUpdate()
    //{

    //}

    //static int cc = 0;
    public static IEnumerator WhileCoroutines()
    {
        isStartedWhile = true;
        while (isStartedWhile)
        {
            //++cc;Debug.Log(cc +"--"+ driverList.Count+"--" + driverRemoveList.Count);
            if (driverRemoveList.Count > 0)
            {
                for (int i = 0; i < driverRemoveList.Count; i++)
                    driverList.Remove(driverRemoveList[i]);
                driverRemoveList.Clear();
            }

            if (driverList.Count > 0)
                for (int i = 0; i < driverList.Count; i++)
                    driverList[i].Exec();
            else
                isStartedWhile = false;

            yield return null;
        }
    }

}
