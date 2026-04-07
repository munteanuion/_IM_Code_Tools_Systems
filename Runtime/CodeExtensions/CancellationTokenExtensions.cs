using System.Threading;

namespace CodeExtensions
{
    public static class CancellationTokenExtensions
    {
        public static CancellationTokenSource ReInit(this CancellationTokenSource _, ref CancellationTokenSource cts)
        {
            cts.CancelDispose(ref cts);
            cts = new CancellationTokenSource();
            return cts;
        }

        public static void CancelDispose(this CancellationTokenSource _, ref CancellationTokenSource cts)
        {
            if (cts == null) return;

            try
            {
                if (!cts.IsCancellationRequested)
                    cts.Cancel();
            }
            catch { /* ignore */ }

            cts.Dispose();
            cts = null;
        }
    }
}