using Core.Infrastructure.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModule
{

    public enum Views
    {
        Foo,
        Bar 
    }

    public interface ITestCaseSession4View : ICaseSession<Views>
    {
    }
    public class TestCaseSession4View : CaseSessionBase<Views>, ITestCaseSession4View
    {
    
        
        public override string ShowName
        {
            get { return "TestCaseSession4View"; }
        }
    }
}
