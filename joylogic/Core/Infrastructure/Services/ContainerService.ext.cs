
using Core.Infrastructure.ModuleLoader;
//using Core.Infrastructure.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Services.ext
{

    public static class MyExtensions
    {
        public static void Configure(this ContainerService service)
        {
            CallbackLogger callbackLogger = new CallbackLogger();
            service.RegisterTypeIfMissing(typeof(IModuleTracker), typeof(ModuleTracker), true);
            service.RegisterInstance<CallbackLogger>(callbackLogger);

            // create navigation Service
            SysNavigationService navigationService = service.CreateInstance<SysNavigationService>();
            service.RegisterTypeIfMissing(typeof(ISysNavigationService), typeof(SysNavigationService), true);
            service.RegisterInstance<ISysNavigationService>(navigationService);

            //CurrentContextSessionService
            CurrentContextSessionService userSessionService = service.CreateInstance<CurrentContextSessionService>();
            service.RegisterTypeIfMissing(typeof(CurrentContextSessionService), typeof(CurrentContextSessionService), true);
            service.RegisterInstance<CurrentContextSessionService>(userSessionService);
             
                //CurrentContextSessionService
            //PocketInventoryCacheSession inventorySession = service.CreateInstance<PocketInventoryCacheSession>();
            //service.RegisterTypeIfMissing(typeof(PocketInventoryCacheSession), typeof(PocketInventoryCacheSession), true);
            //service.RegisterInstance<PocketInventoryCacheSession>(inventorySession);

            ModuleLifeService moduleLifeService = service.CreateInstance<ModuleLifeService>();
            service.RegisterTypeIfMissing(typeof(ModuleLifeService), typeof(ModuleLifeService), true);
            service.RegisterInstance<ModuleLifeService>(moduleLifeService);

          
             
        }
    }
}
