#if UNITY_ADDRESSABLES

using System;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kott.Utilities.Progress
{
    public class WaitForAsyncOperationHandleWithProgress : IEnumerator
    {
        private AsyncOperationHandle _opHandle;
        private readonly IProgress<ProgressReport> _progress;

        public WaitForAsyncOperationHandleWithProgress(AsyncOperationHandle op, IProgress<ProgressReport> progress)
        {
            _opHandle = op;
            _progress = progress;
        }

        public object Current => null;

        public bool MoveNext()
        {
            if (!_opHandle.IsValid()) return false;
            _progress?.Report(new ProgressReport(_opHandle.PercentComplete, null));
            return !_opHandle.IsDone;
        }

        public void Reset()
        { }
    }
}

#endif
