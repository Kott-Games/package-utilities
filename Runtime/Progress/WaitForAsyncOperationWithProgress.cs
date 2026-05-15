using System;
using System.Collections;
using UnityEngine;

namespace Kott.Utilities.Progress
{
    public class WaitForAsyncOperationWithProgress : IEnumerator
    {
        private readonly AsyncOperation _op;
        private readonly IProgress<ProgressReport> _progress;

        public WaitForAsyncOperationWithProgress(AsyncOperation op, IProgress<ProgressReport> progress)
        {
            _op = op;
            _progress = progress;
        }

        public object Current => null;

        public bool MoveNext()
        {
            _progress?.Report(new ProgressReport(_op.progress, null));
            return !_op.isDone;
        }

        public void Reset()
        { }
    }
}
