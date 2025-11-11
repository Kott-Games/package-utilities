using System;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ToG.Scopes
{
    public class WaitForHandle : IEnumerator
    {
        private AsyncOperationHandle _handle;
        private IProgress<ScopeProgress> _progress;

        public object Current => null;

        public WaitForHandle(AsyncOperationHandle handle) : this(handle, null) { }

        public WaitForHandle(AsyncOperationHandle handle, IProgress<ScopeProgress> progress)
        {
            _handle = handle;
            _progress = progress;
        }

        public bool MoveNext()
        {
            if (_progress != null)
            {
                float p = _handle.PercentComplete;
                _progress.Report(new ScopeProgress(p, _handle.DebugName));
            }

            return (_handle as IEnumerator).MoveNext();
        }

        public void Reset()
        { }
    }
}
