using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
    public class TaskEventHandler
    {
        public Task CurrentTask { get; set; }

        public Action<Task> _onTaskStatusChanged;

        public TaskEventHandler(Task task = null)
        {
            CurrentTask = task;

        }

        public async Task WatchTaskAsync<T>(Task<T> task,Action<Task<T>> onTaskStatusChanged)
        {
            CurrentTask = task;
            try
            {
                await task;
            }
            catch
            {
            }

            onTaskStatusChanged?.Invoke(task);
            //TaskStatusChanged?.Invoke(this, new TaskStatusChangedEventArgs(task.Status,task));
        }


        public event EventHandler<TaskStatusChangedEventArgs> TaskStatusChanged;

    }

    public class TaskStatusChangedEventArgs:EventArgs
    {

        public TaskStatus Status { get;}

        public Task Task { get;}

        public TaskStatusChangedEventArgs(TaskStatus status,Task task)
        {
            Status = status;
            Task = task;
        }
    }
}
