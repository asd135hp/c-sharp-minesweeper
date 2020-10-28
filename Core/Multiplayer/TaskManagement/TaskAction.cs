using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultiplayerMinesweeper.Core.Multiplayer.TaskManagement
{
    public abstract class TaskAction<T>
    {
        protected static TaskAction<T> _instance = null;
        private readonly Task _mainTask;
        private readonly List<Task<T>> _taskList;
        private bool _isClosing = false;

        // main task management cycle where it will manage the list of tasks for multiplayer experience
        protected TaskAction(
            CancellationToken token,
            Action<T> action,
            Predicate<T> taskVerification,
            int mergeDataRate
        )
        {
            _taskList = new List<Task<T>>();
            _mainTask = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    // read the whole list of tasks first
                    // tasks that are finished, leave it as it be
                    // the first unfinished task will break the reading loop
                    // and merging data start from here
                    int count = 0;
                    foreach(var task in _taskList)
                    {
                        if (task.IsCompleted && (taskVerification?.Invoke(task.Result) ?? true)) count++;
                        else break;
                    }

                    // merge data if downloading
                    if (count > 0) action?.Invoke(_taskList[count - 1].Result);
                    RemoveTask(count);

                    // wait for 1 sec (default)
                    Thread.Sleep(mergeDataRate);
                }

                // clean up after the tasks has finished
                _taskList.Clear();
            }, token);
        }

        /// <summary>
        /// Push new task into the cycle
        /// </summary>
        /// <param name="newTask"></param>
        public void AddTask(Task<T> newTask)
        {
            if (!_isClosing) _taskList.Add(newTask);
        }

        /// <summary>
        /// Don't know how "safe" this is but the tasks that vanished will eventually be finished
        /// and then disposed automatically by GC so there's no more concern, right?
        /// </summary>
        /// <param name="count"></param>
        private void RemoveTask(int count)
        {
            if (!_isClosing) _taskList.RemoveRange(0, count);
        }

        /// <summary>
        /// Close the current task management action and also its instance.
        /// It is suggested to cancel the tokenSource first and until then could you call this method.
        /// Alternatively, you could provide the tokenSource parameter.
        /// Check the paramter's description first, before using it!
        /// </summary>
        /// <param name="tokenSource">
        /// Must be the same as CancellationToken's source provided for instance creation.
        /// If not, the task will only be closed at the end of the program's cycle and no more instances
        /// could be made anymore.
        /// </param>
        public void Close(CancellationTokenSource tokenSource = null)
        {
            _isClosing = true;

            if(_instance != null)
            {
                // if user provides a tokenSource, use it instead
                tokenSource?.Cancel();

                Logger.Log("Disposing main task...");
                // wait and dispose the main task
                _mainTask?.Wait();
                _mainTask?.Dispose();
                Logger.Log("Main task disposed!");

                // set instance to null for gc and next create instance cycle
                _instance = null;
            }
        }
    }
}
