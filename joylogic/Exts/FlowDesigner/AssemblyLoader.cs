using Core.Infrastructure.ModuleLoader;
using Core.Infrastructure.Services;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace FlowDesigner
{
    public class AssemblyLoader
    {
        private IUnityContainer container;
        public AssemblyLoader(IUnityContainer mainContainer)
        {
            container = mainContainer;
        }
        public void LoadFromFile(string file)
        {
            try
            {
                if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp"))) 
                {
                    Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp"));
                }
                File.Copy(file, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules", Path.GetFileName(file)), true);
                File.Copy(file, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Temp", Path.GetFileName(file)), true);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "不能复制文件"); }

            try
            {
                DirectoryModuleCatalog catalog = new DirectoryModuleCatalog() { ModulePath = ".\\Temp" };
                catalog.Initialize();
               var log= ContainerService.Current.GetInstance<IModuleCatalog>() as AggregateModuleCatalog;
                log.AddCatalog(catalog);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "不能加载库"); }
             
        }


    }
}
