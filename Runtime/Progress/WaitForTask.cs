using System.Collections;
using System.Threading.Tasks;

namespace Kott.Utilities.Progress
{
    public class WaitForTask : IEnumerator
    {
        private Task _task;

        public object Current => null;

        public WaitForTask(Task task)
        {
            _task = task;
        }

        public bool MoveNext()
        {
            return !_task.IsCompleted;
        }

        public void Reset()
        { }
    }
}
