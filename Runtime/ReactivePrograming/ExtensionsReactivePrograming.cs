using System;
using System.Collections.Generic;

namespace ReactivePrograming
{
    public static class ExtensionsReactivePrograming
    {
        public static void DisposeAll(this List<IDisposable> disposables)
        {
            foreach (var disposable in disposables)
                disposable.Dispose();
        }
    }
}