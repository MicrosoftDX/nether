// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Common.Async
{
    // based on https://blogs.msdn.microsoft.com/pfxteam/2012/02/12/building-async-coordination-primitives-part-5-asyncsemaphore/
    public class AsyncSemaphore
    {
        private readonly Queue<TaskCompletionSource<bool>> _waiters = new Queue<TaskCompletionSource<bool>>();
        private int _currentCount;

        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0)
                throw new ArgumentOutOfRangeException("initialCount");
            _currentCount = initialCount;
        }
        public Task WaitAsync()
        {
            lock (_waiters)
            {
                if (_currentCount > 0)
                {
                    --_currentCount;
                    return Task.CompletedTask;
                }
                else
                {
                    var waiter = new TaskCompletionSource<bool>();
                    _waiters.Enqueue(waiter);
                    return waiter.Task;
                }
            }
        }
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;
            lock (_waiters)
            {
                if (_waiters.Count > 0)
                    toRelease = _waiters.Dequeue();
                else
                    ++_currentCount;
            }
            if (toRelease != null)
                toRelease.SetResult(true);
        }
    }
}
