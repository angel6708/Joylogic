using Core.Infrastructure;
using Core.Infrastructure.Consts;
using Core.Infrastructure.Events;
using Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using System.Threading;
using System.Windows.Threading;
using Core.Infrastructure.ViewModels;

using Core.Infrastructure.Workflow;
using Data.Models;
using Data.Access.BLL;
using Data.Access.Interface;
using System.Transactions;
using Prism.Regions;
using Prism.Events;
using Core.Infrastructure.Logging;
using Prism.Commands;

namespace LoginModule
{
    public class LoginViewModel : BaseViewModel
    {
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private ISysNavigationService _sysNavigationService;
        private Dispatcher _uiDispatcher;
        private ILoggerFacade _logger;
        private CurrentContextSessionService _userSessionService;


        public LoginViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ISysNavigationService sysNavigationService, CurrentContextSessionService userSessionService, ILoggerFacade logger)
        {
            _uiDispatcher = Dispatcher.CurrentDispatcher;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _sysNavigationService = sysNavigationService;
            _userSessionService = userSessionService;
            _logger = logger;
            LoginCommand = new DelegateCommand<string>(ExecCommand);
        }

        protected override IReadOnlyCollection<Core.Infrastructure.Controls.IconTextDelegateCommand<string>> GetToolBarCommands()
        {
            var cmds = new List<Core.Infrastructure.Controls.IconTextDelegateCommand<string>>();
            cmds.Add(new Core.Infrastructure.Controls.IconTextDelegateCommand<string>("Hello", s => { }));
            return cmds;
        }


        protected override void Actived()
        {
            base.Actived();
            this.UserId = string.Empty;
            this.Password = string.Empty;
            this.ErrorMessage = "用户名";

        }

        protected override void Disactived()
        {
            base.Disactived();

        }

        protected override bool IsShowHeader
        {
            get
            {
                return true;
            }
        }

        protected override bool IsSearchEnabled
        {
            get
            {
                return true;
            }
        }

        protected override string GetInitialHeaderName()
        {
            return "登录";
        }

        private void ExecCommand(string viewName)
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(Password))
            {
                this.ErrorMessage = "用户名或密码不能为空";

                return;
            }
            OnUserLogin(UserId, Password);
        }


        /// <summary>
        /// 注意:要把该方法放到 connection scope中使用，不能单独使用
        /// </summary>
        /// <param name="user"></param>
        private void OnUserLogin(string username, string password)
        {

            this.BeginLoading("正在登陆系统。。");
            ThreadPool.QueueUserWorkItem(a =>
            {

                UserInfo ui = null;
                //using(TransactionScope scope=new TransactionScope())
                using (var conScope = ContainerService.Current.GetInstance<IConnectionSessionScope>())
                {
                    UserInfoBLL uib = new UserInfoBLL();
                    ui = uib.Login(username, password);
                }
                this.EndLoading();
                ui = new UserInfo { LoginName = "admin", UserName = "admin" };
                if (ui == null || string.IsNullOrEmpty(ui.LoginName))
                {
                    this.ErrorMessage = "用户名或密码错误";
                }
                else
                {
                    _userSessionService.Login(ui);
                    var evt = _eventAggregator.GetEvent<LoginEvent>();
                    evt.Publish(new OperationEventArgs<UserInfo> { TargetModel = ui });
                    _sysNavigationService.ChangeMainRegion(new Intent(nameof(LoginView)));
                }

            });

        }


        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                this.RaisePropertyChanged(() => this.Password);
            }
        }

        private string _userId;
        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;

                if (string.IsNullOrEmpty(_userId))
                {
                    this._errorMessage = "用户名";
                    this.RaisePropertyChanged(() => this.ErrorMessage);
                }
                else
                {
                    this._errorMessage = "";
                    this.RaisePropertyChanged(() => this.ErrorMessage);
                }
                this.RaisePropertyChanged(() => this.UserId);
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                this.RaisePropertyChanged(() => this.ErrorMessage);

                if (!string.IsNullOrEmpty(_errorMessage))
                {
                    _userId = "";
                    this.RaisePropertyChanged(() => this.UserId);
                }


            }
        }

        private DelegateCommand<string> _loginCommand;
        public DelegateCommand<string> LoginCommand
        {

            get { return _loginCommand; }
            set
            {
                _loginCommand = value;
                this.RaisePropertyChanged(() => this.LoginCommand);
            }
        }



        public int UserNameMaxLength
        {
            get { return 200; }
        }

        public int PasswordMaxLength
        {
            get { return 200; }
        }

    }
}
