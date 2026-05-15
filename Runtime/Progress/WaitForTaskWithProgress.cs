using System;
using System.Collections;
using System.Threading.Tasks;

namespace Kott.Utilities.Progress
{
    public class WaitForTaskWithProgress : IEnumerator
    {
        private readonly Task _task;
        private readonly IProgress<ProgressReport> _progress;

        public object Current => null;

        public WaitForTaskWithProgress(Task task, IProgress<ProgressReport> progress)
        {
            _task = task;
            _progress = progress;
        }

        public bool MoveNext()
        {
            if (_progress == null) return !_task.IsCompleted;
            
            TaskStatus status = _task.Status;
            float progress =
                status <= TaskStatus.WaitingToRun ? 0f :
                status <= TaskStatus.Running ? 0.1f :
                status <= TaskStatus.WaitingForChildrenToComplete ? 0.9f : 1f;

            _progress.Report(new ProgressReport(progress, status.ToString()));
            return !_task.IsCompleted;
        }

        public void Reset()
        { }
    }
}
