// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
public static class CancellationTokenEx
{
    public static Task WhenCanceled(this CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();
        cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
        return tcs.Task;
    }
}