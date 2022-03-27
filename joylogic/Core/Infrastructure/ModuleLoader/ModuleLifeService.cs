using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.ModuleLoader
{
    public class ModuleLifeService
    {
        public IModuleLifeServiceSupport CurrentModule { get; set; }
    }
}
