using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

namespace NREAL.AR.Async
{
    public class WaitForBackgroundThread
    {
        public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
        {
            return Task.Run(() => { }).ConfigureAwait(false).GetAwaiter();
        }
    }
}
