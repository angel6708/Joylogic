using Core.Infrastructure.Events;
using Core.Infrastructure.Services;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Models
{
    public abstract class AutoDisposeBase:IDisposable
    {

        public AutoDisposeBase() 
        {
            var eventAggregator = ContainerService.Current.GetInstance<IEventAggregator>();
           var evt= eventAggregator.GetEvent<DisposeEvent>();
           evt.Subscribe(new Action<bool>(InvokeDisope));
        }

        private void InvokeDisope(bool obj)
        {
            this.Dispose();
        }

       
       public  abstract void Dispose();
    }
}
