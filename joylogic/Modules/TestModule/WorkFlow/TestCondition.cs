using Core.Infrastructure.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModule.WorkFlow
{

    public interface ITestCondition: IConditionSession { }
    public class TestCondition : ITestCondition
    {
        private TestContext context;
        public TestCondition(TestContext context) 
        {
            this.context = context;
        } 
        public string ShowName => "TestCondition";

        public bool Condition()
        {
            return context.TestBoolValue;
        }
    }
}
