using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MillimanProjectManConsole.ComplexUpload
{
    public class TaskManager
    {
        private static object LockingVar = new object();

        Dictionary<string, TaskBase> _Tasks = new Dictionary<string, TaskBase>();

        /// <summary>
        /// Add the task to the holding list so we can access it's status when needed
        /// </summary>
        /// <param name="TB"></param>
        /// <returns></returns>
        public bool ScheduleTask(TaskBase TB)
        {
            lock (LockingVar)
            {
                try
                {
                    _Tasks.Add(TB.TaskID, TB);
                    return true;

                }
                catch (Exception ex)
                {
                    MillimanCommon.Report.Log(MillimanCommon.Report.ReportType.Error, "Failed to schedule task", ex);
                }
                return false;
            }
        }

        public TaskBase FindTask(string ID)
        {
            TaskBase TB = null;
            lock (LockingVar)
            {
                if (_Tasks.ContainsKey(ID))
                    return _Tasks[ID];
            }
            return TB ;
        }

        public bool DeleteTask(string ID)
        {
            lock (LockingVar)
            {
                if (_Tasks.ContainsKey(ID))
                {
                    TaskBase TB = _Tasks[ID];
                    TB.AbortTask = true;
                    _Tasks.Remove(ID);
                    TB = null;
                }
            }
            return true;
        }
    }
}