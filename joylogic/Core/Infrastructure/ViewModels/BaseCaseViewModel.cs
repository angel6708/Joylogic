using Core.Infrastructure.Services;
using Core.Infrastructure.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.ViewModels
{
    public abstract class BaseCaseViewModel<TypeOfCaseEnum, TypeOfCaseSessionI> : BaseViewModel where TypeOfCaseSessionI : ICaseSession<TypeOfCaseEnum>
    {

        /// <summary>
        /// 配合流程使用，要求提前注册该类型到容器
        /// </summary>
        public TypeOfCaseSessionI CaseSession
        {
            get
            {
                return ContainerService.Current.GetInstance<TypeOfCaseSessionI>();
            }
        }

        public void GotoCase(TypeOfCaseEnum c)
        {
            CaseSession.SetCaseValue(c);
            this.Intent.Finsh();
        }
    }
}
