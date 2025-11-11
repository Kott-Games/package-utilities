#if UNITY_ADDRESSABLES

using System;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Kott.Utilities.Progress
{
    public class WaitForAsyncOperationHandleWithProgress : IEnumerator
    {
        private AsyncOperationHandle _opHandle;
        private IProgress<ProgressReport> _progress;

        public WaitForAsyncOperationHandleWithProgress(AsyncOperationHandle op, IProgress<ProgressReport> progress)
        {
            _opHandle = op;
            _progress = progress;
        }

        public object Current => null;

        public bool MoveNext()
        {
            _progress.Report(new(_opHandle.PercentComplete, null));
            return !_opHandle.IsDone;
        }

        public void Reset()
        { }
    }
}

#endif
