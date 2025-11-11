using System;
using System.Collections;
using System.Threading.Tasks;

namespace Kott.Utilities.Progress
{
    public class WaitForTaskWithProgress : IEnumerator
    {
        private Task _task;
        private IProgress<ProgressReport> _progress;

        public object Current => null;

        public WaitForTaskWithProgress(Task task, IProgress<ProgressReport> progress)
        {
            _task = task;
            _progress = progress;
        }

        public bool MoveNext()
        {
            if (_progress != null)
            {
                TaskStatus status = _task.Status;
                float progress =
                    status <= TaskStatus.WaitingToRun ? 0f :
                    status <= TaskStatus.Running ? 0.1f :
                    status <= TaskStatus.WaitingForChildrenToComplete ? 0.9f : 1f;

                _progress.Report(new(progress, status.ToString()));
            }

            return !_task.IsCompleted;
        }

        public void Reset()
        { }
    }
}
