

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool.CodeGen
{
    partial class MainWindow
    {

        string GenerateMaps(Table t)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in t.Columns)
            {
                sb.AppendFormat(@"
                    ColumnPropMaps.Add(""{0}"", ""{1}"");", c.Name, c.AName);

            }
            return sb.ToString();
        }

         

        private void GenerateBLLClassFile(Table t)
        {
            var pkCol = t.Columns.Where(a => a.IsPK).FirstOrDefault();
            string _fileName;
            //mns,dns, cn
            //mns,dns, cn
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Data.Access.Interface;
using Data.Models;
 
namespace Data.Access.Interface.BLL
{{
    [ServiceContract(Namespace =""Data.Access.BLL"")]
    public partial interface I{2}BLL{1}
    {{
        
    }}
}}
", NameSpace + ".Models",
 t.IsSnapshot ? ":IRegable ,IBaseBLL<" + t.AName + "> ": ":IRegable,IBaseBLL<" + t.AName + "> ",
 t.AName,
 t.AName.ToLower());

            _fileName = "I" + t.AName + "BLL.cs";
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IBLL")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IBLL"));
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IBLL") + "\\" + _fileName))
            {
                sw.Write(sb.ToString());
            }

            sb = new StringBuilder();

            sb.AppendFormat(
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Data.Access;
using Data.Models;
using Data.Access.Interface.BLL; 

namespace Data.Access.BLL
{{

   [ServiceBehavior( Name=""{2}ServiceBehavior"", InstanceContextMode =InstanceContextMode.Single)]
	public partial class {2}BLL  : BaseBLL<{2}> , I{2}BLL
	{{
 
    }}

}}",
   NameSpace + ".Models",
   NameSpace + ".BLL",
   t.AName,
   t.AName.ToLower(),
   t.Name,
   t.Columns.Where(a => a.IsPK).FirstOrDefault().Name,
   this.GenerateMaps(t),
   this.Generateparams(t),
   t.IsSnapshot? "protected override bool IsSnapshot { get { return true; } }":""
   );
            _fileName = t.AName + "BLL.cs";
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BLL")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BLL"));
            }

            using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BLL") + "\\" + _fileName))
            {
                sw.Write(sb.ToString());
            }

        }
    }
}


