using Core.Infrastructure.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModule
{
    public enum TestCases { Foreach, Condition,SaveService }
    public interface ITestCaseSession : ICaseSession<TestCases> { }
    public class TestCaseSession : CaseSessionBase<TestCases>, ITestCaseSession
    {
        public override string ShowName => "TestCaseSession";

     }
}
