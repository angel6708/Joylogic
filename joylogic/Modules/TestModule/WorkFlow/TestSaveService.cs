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
    public interface ITestSaveServie : ISaveService { }
    public class TestSaveService : ITestSaveServie
    {
        public bool IsInUiThread { get; set; } = false;

        public string SavingDesc => "Test Saving ...";

        private WorkFlow.TestContext context;

        public TestSaveService(WorkFlow.TestContext context) 
        {
            this.context = context;
        }

        public bool ExecSave()
        {
            Debug.WriteLine($"obj: {context.CurrentObj},TestSaveService long time saving..");
            Thread.Sleep(2000);
            Debug.WriteLine($"obj: {context.CurrentObj}, TestSaveService save finished");
            return true;
        }
    }
}
