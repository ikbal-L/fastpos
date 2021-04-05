using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace FastPosFrontend.Helpers
{
    public class NotifyAllTasksCompletion: PropertyChangedBase
    {
        private readonly Task[] _tasks;

        public NotifyAllTasksCompletion(params Task[] tasks)
        {
            _tasks = tasks;
            Task = Task.WhenAll(tasks);
            
            if (!Task.IsCompleted)
            {
                var _ = WatchTaskAsync(Task);
            }
            else
            {
                NotifyOfPropertyChange(nameof(Status));
                NotifyOfPropertyChange(nameof(IsCompleted));
                NotifyOfPropertyChange(nameof(IsNotCompleted));
                //NotifyOfPropertyChange(nameof(IsSuccessfullyCompleted));
                AllTasksCompleted?.Invoke(this, new AllTasksCompletedEventArgs(true));
            }
        }

        private async Task WatchTaskAsync(Task task)
        {
            try
            {
                await task;
            }
            catch(Exception e)
            {
                throw e;
            }
            
            
            NotifyOfPropertyChange(nameof(Status));
            NotifyOfPropertyChange(nameof(IsCompleted));
            NotifyOfPropertyChange(nameof(IsNotCompleted));
            if (task.IsCanceled)
            {
                NotifyOfPropertyChange(nameof(IsCanceled));
            }
            else if (task.IsFaulted)
            {
                
                NotifyOfPropertyChange(nameof(IsFaulted));
                NotifyOfPropertyChange(nameof(Exception));
                NotifyOfPropertyChange(nameof(ErrorMessage));
            }
            else if (task.Status == TaskStatus.RanToCompletion)
            {
                NotifyOfPropertyChange(nameof(IsSuccessfullyCompleted));
                AllTasksCompleted?.Invoke(this,new AllTasksCompletedEventArgs(true));
                
            }
        }

        public Task Task { get;  private set; }


        public TResult GetResult<TResult>()
        {
            var t = _tasks.ToList().FirstOrDefault(task => task is Task<TResult> ttR );
            return t switch
            {
                null => default,
                Task<TResult> taskOfTResult => taskOfTResult.Result,
                _ => default
            };
        }

        public TaskStatus Status => Task.Status;
        public bool IsCompleted => Task.IsCompleted;
        public bool IsNotCompleted => !Task.IsCompleted;

        public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

        public bool IsCanceled => Task.IsCanceled;
        public bool IsFaulted => Task.IsFaulted;
        public AggregateException Exception => Task.Exception;

        public Exception InnerException => Exception?.InnerException;

        public string ErrorMessage => InnerException?.Message;

        public event EventHandler<AllTasksCompletedEventArgs> AllTasksCompleted;


    }

    public class AllTasksCompletedEventArgs:EventArgs
    {
        public AllTasksCompletedEventArgs(bool isTaskCollectionCompleted)
        {
            IsTaskCollectionCompleted = isTaskCollectionCompleted;
        }

        public bool IsTaskCollectionCompleted { get; }

    }
}