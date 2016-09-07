using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace ClientPublisher
{
    public class TaskBase
    {
        private string _QualafiedProjectFile;

        public string QualafiedProjectFile
        {
            get { return _QualafiedProjectFile; }
            set {
                if (Reports == null)
                    Reports = new MillimanCommon.QVWReportBank(value);
                  _QualafiedProjectFile = value; }
        }

        public string TaskID { get; set; }
        public DateTime TaskStart { get; set; }
        public DateTime TaskEnd { get; set; }
        public bool TaskCompleted { get; set; }
        public bool TaskCompletionWithError { get; set; }
        private string _TaskCompletionMessage;

        public string TaskCompletionMessage
        {
            get { return _TaskCompletionMessage; }
            set {
                if (Reports != null)
                    Reports.AddItemToList(new MillimanCommon.QVWReportBank.AuditClass(value));
                   _TaskCompletionMessage = value; }
        }

        public MillimanCommon.QVWReportBank Reports { get; set; }

        private ParameterizedThreadStart ThreadStart = null;
        private Thread ProcessingThread = null;

        public bool AbortTask { get; set; }
        public TaskBase()
        {
            TaskID = Guid.NewGuid().ToString("N");
            TaskCompletionWithError = false;
            TaskCompleted = false;
            AbortTask = false;
        }

        public void StartTask()
        {
            ThreadStart = new ParameterizedThreadStart(TaskProcessor);
            TaskStart = DateTime.Now;
            ProcessingThread = new Thread(ThreadStart);
            ProcessingThread.Start();
        }

        virtual public void TaskProcessor(object parms) 
        {
            TaskCompleted = true;
            TaskEnd = DateTime.Now;
        }
    }
}