

using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Services
{
    public class CurrentContextSessionService
    {
        /// <summary>
        /// 上次退出登录的人，用于锁屏恢复
        /// </summary>
        public UserInfo LastLoginOutUser { get; private set; }
        /// <summary>
        /// 锁屏时，当前视图的名称
        /// </summary>
        public string LastViewName { get; private set; }
        public UserInfo CurrentLoginedUser { get; private set; }

        public UserInfo CurrentWitnessUser { get; private set; }
  

        public event EventHandler LoginedEvent;
        public event EventHandler LoginOutEvent;

        public void WitnessLogin(UserInfo info)
        {
            CurrentWitnessUser = info;
        }

        public void Login(UserInfo info)
        {
            CurrentLoginedUser = info;
            if (LoginedEvent != null)
            {
                LoginedEvent(this, EventArgs.Empty);
            }
        }

        public void LoginOut()
        {
            CurrentLoginedUser = null;
            if (LoginOutEvent != null)
            {
                LoginOutEvent(this, EventArgs.Empty);
            }
        }

        public void WitnessLoginOut()
        {
            CurrentWitnessUser = null;
        }
  
    }
}
