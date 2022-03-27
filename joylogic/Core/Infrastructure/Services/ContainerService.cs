
using System;

using System.Threading;

using System.IO;
using System.ServiceModel;
using Unity;
using Unity.Resolution;
using Unity.Lifetime;

namespace Core.Infrastructure.Services
{
    public class ContainerService
    {

        private static ContainerService _current;
        public static ContainerService Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new ContainerService();
                }
                return _current;
            }
        }
        public void SetContainer(IUnityContainer container)
        {
            Container = container;
        }
        private IUnityContainer Container { get; set; }

        public T CreateInstance<T>()
        {

            return (T)Container.Resolve(typeof(T));
           
        }

        public T GetInstance<T>()
        {

            return (T)Container.Resolve(typeof(T));

        }


        public object GetInstance(Type t)
        {
            return Container.Resolve(t);
        }

        public void RegisterInstance<T>(T instance)
        {
            Container.RegisterInstance<T>(instance);

        }

        public void RegisterInstance(Type t, object instance)
        {
            Container.RegisterInstance(t, instance);
        }

        public void RegisterTypeIfMissing<From,To>( bool registerAsSingleton=false)
        {
            Type fromType = typeof(From); 
            Type toType = typeof(To);
            if (!Container.IsRegistered(fromType))
            {

                if (registerAsSingleton)
                {
                    Container.RegisterType(fromType, toType, new ContainerControlledLifetimeManager());
                }
                else
                {
                    Container.RegisterType(fromType, toType);
                }
            }
        }

        public void RegisterTypeIfMissing<T>(bool registerAsSingleton = false)
        {
            var type = typeof(T);
            if (!Container.IsRegistered(type))
            {

                if (registerAsSingleton)
                {
                    Container.RegisterType(type, new ContainerControlledLifetimeManager());
                }
                else
                {
                    Container.RegisterType(type);
                }
            }
        }
        public void RegisterTypeIfMissing(Type fromType, Type toType, bool registerAsSingleton)
        {
            if (fromType == null)
            {
                throw new ArgumentNullException("fromType");
            }
            if (toType == null)
            {
                throw new ArgumentNullException("toType");
            }
            if (!Container.IsRegistered(fromType))
            {

                if (registerAsSingleton)
                {
                    Container.RegisterType(fromType, toType, new ContainerControlledLifetimeManager());
                }
                else
                {
                    Container.RegisterType(fromType, toType);
                }
            }
        }

    }
}
