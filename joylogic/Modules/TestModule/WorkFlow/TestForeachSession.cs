using Core.Infrastructure.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModule.WorkFlow
{
    public interface ITestForeachSession : IForeachSession { }
    public class TestForeachSession : ITestForeachSession
    {
        public ForeachEnumerator ForeachEnumerator { get; set; } = new ForeachEnumerator();
    }
}
