using Core.Infrastructure.Workflow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestModule
{
    public interface ITestSaveServieFalse : ISaveService { }
    public class TestSaveServiceFalse : ITestSaveServieFalse
    {
        public bool IsInUiThread { get; set; } = false;

        public string SavingDesc => "Test Saving Return False ...";

        private WorkFlow.TestContext context;

        public TestSaveServiceFalse(WorkFlow.TestContext context) 
        {
            this.context = context;
        }

        public bool ExecSave()
        {
            Debug.WriteLine("TestSaveServiceFalse saving..");
            Thread.Sleep(2000);
            return false;
        }
    }
}
