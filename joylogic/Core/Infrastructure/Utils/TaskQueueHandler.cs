using Core.Infrastructure.Services;
using Data.Access.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Infrastructure.Utils
{
    public class TaskQueueHandler
    {


        private LinkedList<TaskInfo> _taskQueue;

        public TaskQueueHandler() { _taskQueue = new LinkedList<TaskInfo>(); }
        public void ExecuteTake()
        {

        }

        public void Enqueue(TaskInfo task)
        {
            _taskQueue.AddLast(task);
        }

        public void Execute(Action<TaskInfo> perCall = null, Action callback = null)
        {

            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            ThreadPool.QueueUserWorkItem((obj) =>
            {

                var start = _taskQueue.First;
                var temp = start; 
                 
                while (start != null)
                {
                    start.Value.Handled = false;
                    start.Value.IsFinished = false;

                    if (perCall != null)
                    {
                        perCall(start.Value);
                    }
                    start.Value.TryTimes++;
                    if (!start.Value.Handler(start.Value.Param))
                    {
                       
                        if (!start.Value.Retry)
                        {
                            start.Value.IsFinished = true;
                            start.Value.Handled = false;
                            start = start.Next;
                        }
                        else
                        { 
                            start.Value.Handled = false;
                        }

                    }
                    else
                    {
                        start.Value.Handled = true;
                        start.Value.IsFinished = true;
                        start = start.Next;

                    }



                    if (temp.Value.IsFinished && perCall != null)
                    {
                        perCall(temp.Value);
                        temp = start;
                    }
                }

                if (callback != null)
                {
                    callback();
                }
            });
        }
    }
}
