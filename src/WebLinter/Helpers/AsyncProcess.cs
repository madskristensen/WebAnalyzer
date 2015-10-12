using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WebLinter
{
    public static class AsyncProcess
    {
        // Based on answers from here: http://stackoverflow.com/questions/470256/process-waitforexit-asynchronously
        public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);

            if (cancellationToken != default(CancellationToken))
            {
                cancellationToken.Register(() =>
                {
                    tcs.TrySetCanceled();
                });
            }

            return tcs.Task;
        }
    }
}
