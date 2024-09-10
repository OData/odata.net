using System.Threading.Tasks;

namespace Microsoft.OData.Edm.Helpers
{
    internal class TaskUtils
    {
        /// <summary>
        /// Already completed task.
        /// </summary>
        private static Task _completedTask;

        // <summary>
        /// Returns already completed task instance.
        /// </summary>
        public static Task CompletedTask
        {
            get
            {
#if NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
                _completedTask = Task.CompletedTask;
#else
                // Note that in case of two threads competing here we would create two completed tasks, but only one
                // will be stored in the static variable. In any case, they are identical for all other purposes,
                // so it doesn't matter which one wins
                if (_completedTask == null)
                {
                    // Create a TaskCompletionSource - since there's no non-generic version use a dummy one
                    // and then cast to the non-generic version.
                    TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
                    taskCompletionSource.SetResult(null);
                    _completedTask = taskCompletionSource.Task;
                }
#endif
                return _completedTask;
            }
        }
    }
}
