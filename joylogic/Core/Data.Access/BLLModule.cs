using Core.Infrastructure.Services;
using Data.Access.BLL;
using Data.Access.Interface;
using Data.Models;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Configuration;

namespace Data.Access
{
    public class BLLModule : IModule
    {
        public BLLModule()
        {
        }
        public void Initialize()
        {
            
             ConnectionFactory.Current.ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            ContainerService.Current.RegisterInstance<IConnectionFactory>(ConnectionFactory.Current);


            var assembly = System.Reflection.Assembly.GetExecutingAssembly();

            var regableType = typeof(IRegable);
            foreach (var t in assembly.DefinedTypes)
            {
                if (t.IsClass && regableType.IsAssignableFrom(t))
                {
                    var interfaces = t.GetInterfaces();
                    Type larger = regableType;
                    foreach (var i in interfaces)
                    {
                        if (larger.IsAssignableFrom(i))
                        {
                            larger = i;
                        }
                    }
                    ContainerService.Current.RegisterTypeIfMissing(larger, t.AsType(), false);
                }
            }
            AfterInit();

        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            this.Initialize();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }

        private void AfterInit() 
        {
            //PatientOrderDetailBLL p = new PatientOrderDetailBLL();
            //p.GetPatientOrderDetailList();
            //PatientOrderBLL pob=new PatientOrderBLL();
            //pob.GetPatientOrder(Guid.Empty);
            //LogicalPocketBLL bll = new LogicalPocketBLL();
            //bll.ListLogicalPocketByItemKey();
        }

    }
}
