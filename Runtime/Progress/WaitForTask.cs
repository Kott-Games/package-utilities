using System;
using System.Collections;
using System.Threading.Tasks;

namespace ToG.Scopes
{
    public class WaitForTask : IEnumerator
    {
        private Task _task;
        private IProgress<ScopeProgress> _progress;

        public object Current => null;

        public WaitForTask(Task task) : this(task, null) { }

        public WaitForTask(Task task, IProgress<ScopeProgress> progress)
        {
            _task = task;
            _progress = progress;
        }

        public bool MoveNext()
        {
            if (_progress != null)
            {
                TaskStatus status = _task.Status;
                float p =
                    status <= TaskStatus.WaitingToRun ? 0f :
                    status <= TaskStatus.Running ? 0.1f :
                    status <= TaskStatus.WaitingForChildrenToComplete ? 0.9f : 1f;

                _progress.Report(new ScopeProgress(p, status.ToString()));
            }

            return !_task.IsCompleted;
        }

        public void Reset()
        { }
    }
}