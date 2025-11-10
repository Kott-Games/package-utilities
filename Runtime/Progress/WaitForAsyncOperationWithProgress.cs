using System;
using System.Collections;
using UnityEngine;

namespace Kott.Utilities.Progress
{
    public class WaitForAsyncOperationWithProgress : IEnumerator
    {
        private AsyncOperation _op;
        private IProgress<ProgressReport> _progress;

        public WaitForAsyncOperationWithProgress(AsyncOperation op, IProgress<ProgressReport> progress)
        {
            _op = op;
            _progress = progress;
        }

        public object Current => null;

        public bool MoveNext()
        {
            _progress.Report(new(_op.progress, null));
            return !_op.isDone;
        }

        public void Reset()
        { }
    }
}
