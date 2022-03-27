using Core.Infrastructure.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Workflow
{

    public class Intent
    {
        public string IntentName { get; internal set; }
        public string ViewName { get; internal set; }
        public NavigationBehavior NavigationBehavior { get; internal set; }
        public bool IsFromCancel { get; internal set; }

        public void Finsh()
        {
          
            if (Finshed != null)
            {
                this.Finshed();
            }
          
        }
        public void Cancel()
        {
           
            if (Canceled != null)
            {
                this.Canceled();
            }
        }

        public Action Finshed { get; internal set; }
        public Action Canceled { get; internal set; }


        public Intent()
            : this("")
        {

        }
        public Intent(string viewName, string viewShownName, NavigationBehavior behavior = NavigationBehavior.Replace)
        {
        }

        public Intent(string viewName) :
            this(viewName, viewName, NavigationBehavior.Replace)
        {

        }



        public string ViewShownName { get; internal set; }
    }
}
