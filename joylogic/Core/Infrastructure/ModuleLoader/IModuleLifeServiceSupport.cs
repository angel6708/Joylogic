using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.ModuleLoader
{
    public interface IModuleLifeServiceSupport
    {
        void OnActive();
        void OnDisactive();
    }
}
