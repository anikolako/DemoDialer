using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DialerDemo.Helper
{
    public static class ForEachAsync
    {
        /// <summary>
        /// Concurrently Executes async actions for each item of <see cref="IEnumerable<typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of IEnumerable</typeparam>
        /// <param name="enumerable">instance of <see cref="IEnumerable<typeparamref name="T"/>"/></param>
        /// <param name="action">an async <see cref="Action" /> to execute</param>
        /// <param name="maxDegreeOfParallelism">Optional, An integer that represents the maximum degree of parallelism,
        /// Must be grater than 0</param>
        /// <returns>A Task representing an async operation</returns>
        public static async Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> asyncAction, int maxDegreeOfParallelism = 5)
        {
            var semaphoreSlim = new SemaphoreSlim(maxDegreeOfParallelism);
            var tcs = new TaskCompletionSource<object>();
            var exceptions = new ConcurrentBag<Exception>();
            bool addingCompleted = false;

            foreach (T item in source)
            {
                await semaphoreSlim.WaitAsync();
                await asyncAction(item).ContinueWith(t =>
                 {
                     semaphoreSlim.Release();

                     if (t.Exception != null)
                     {
                         exceptions.Add(t.Exception);
                     }

                     if (Volatile.Read(ref addingCompleted) && semaphoreSlim.CurrentCount == maxDegreeOfParallelism)
                     {
                         tcs.SetResult(null);
                     }
                 });
            }

            Volatile.Write(ref addingCompleted, true);
            await tcs.Task;
            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
