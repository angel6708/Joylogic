using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Workflow
{
    public interface ICaseSession
    {
        string GetCaseValue(); 
        string ShowName { get; }

        Type CaseType { get; }
    }

    public interface ICaseSession<ToC> : ICaseSession 
    {
        void SetCaseValue(ToC value);
    }


    public abstract class CaseSessionBase<ToC> : ICaseSession<ToC>
    {

        private ToC _value;
        public void SetCaseValue(ToC value) 
        {
            _value = value;
        }
        public string GetCaseValue()
        {
            return  _value.ToString();
        }

          

        public abstract string ShowName { get; }


        public Type CaseType
        {
            get { return (typeof(ToC)); }
        }
    }
}
