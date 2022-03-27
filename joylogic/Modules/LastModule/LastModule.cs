
using Core.Infrastructure;
using Core.Infrastructure.Consts;
using Core.Infrastructure.Controls;
using Core.Infrastructure.Events;
using Core.Infrastructure.Logging;
using Core.Infrastructure.ModuleLoader;
using Core.Infrastructure.Services;
using Core.Infrastructure.Utils;
using Core.Infrastructure.Workflow;
using Data.Access.Interface;
using FlowViewModule;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace LastModule
{
    public class LastModule : BaseModule
    {
        ISysNavigationService _navigationService;
        public LastModule(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer mainContainer, ISysNavigationService navigationService)
            : base(regionManager, eventAggregator, mainContainer)
        {
            this._navigationService = navigationService;
             
        }

        protected override void AfterInitialize()
        {
            base.AfterInitialize();

            var a = ActivityLoader.Load("Test");
            WorkflowView view = new WorkflowView();
            view.Show();
            view.Load(a as ComposedActivity);
            a.Start();

            return;
            //TaskQueueHandler handler = new TaskQueueHandler();

            //handler.Enqueue(new TaskInfo { Handler = InitDbConn, Name = "连接数据库", Retry=true }); 

            //handler.Enqueue(new TaskInfo { Handler = ShowLogin, Name = "登录界面" });

            //handler.Execute(PerCallback,InitFinishedHandler);


        }

        private void InitFinishedHandler()
        {
            _eventAggregator.GetEvent<ShowLoadingEvent>().Publish(new ShowLoadingEventArg()
            {
                Status = Visibility.Collapsed,

            });
        }



        private bool ShowLogin(object obj)
        {
            ContainerService.Current.GetInstance<ISysNavigationService>().ChangeMainRegion(new Core.Infrastructure.Workflow.Intent( "LoginView"));
            return true;
        }

        private bool InitDbConn(object obj)
        {
            using (var conn = ContainerService.Current.CreateInstance<IConnectionSessionScope>())
            {
                return conn.HasOpen;
            }
        }

        private void PerCallback(TaskInfo taskInfo)
        {
            if (taskInfo != null)
            {

                var logger = ContainerService.Current.GetInstance<ILoggerFacade>();

                var msg = taskInfo.Name;
                if (taskInfo.IsFinished)
                {
                    msg = string.Format("{0},尝试次数{1},{2}", msg, taskInfo.TryTimes, taskInfo.Handled ? "已完成" : "失败");
                }
                else
                {
                    msg = string.Format("正在{0}", msg);
                }

                logger.Log(msg, Category.Info, Priority.Medium);
                //Debug.WriteLine(taskInfo.Name + ": Handled");

                IEventAggregator _eventAggregator = ContainerService.Current.GetInstance<IEventAggregator>();
                _eventAggregator.GetEvent<ShowLoadingEvent>().Publish(new ShowLoadingEventArg()
                {
                    Status = Visibility.Visible,
                    Caption = msg,
                });
            }
        }

        protected override System.Reflection.Assembly ExecAssembly
        {
            get { return System.Reflection.Assembly.GetExecutingAssembly(); }
        }
    }
}
