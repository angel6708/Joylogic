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

        string genBLLServiceHostSnapshot(Table t)
        {
            return string.Format(@"
        public {0} GetCurrent({0} {1})
        {{
            using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                return this.{0}BLL.GetCurrent({1});
            }}
        }}

        public void SaveCurrent({0} {1})
        {{
             using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                 this.{0}BLL.SaveCurrent({1});
            }}
        }}

        public void UpdateCurrent({0} {1})
        {{
            using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                 this.{0}BLL.UpdateCurrent({1});
            }}
        }}

        public void DeleteCurrent({0} {1})
        {{
             using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                 this.{0}BLL.DeleteCurrent({1});
            }}
        }}   

        public bool ExsistBase({0} key)
        {{ 
             using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                return this.{0}BLL.ExsistBase(key);
            }}
        }} 

        public void InsertBase({0} key)
        {{
             using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                this.{0}BLL.InsertBase(key);
            }}
        }} 
 
", t.AName, t.AName.ToLower());
        }


        void GenerateBLLServiceHost(Table t)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RDH.Data.BLL; 
using RDH.Data.Models;
 
using RDH.Data;

namespace RDH.ServiceHost
{{
    public partial class {0}BLLService : I{0}BLL
    {{
        private I{0}BLL _{1}BLL = null;
        I{0}BLL {0}BLL
        {{
            get 
            {{
                if (_{1}BLL == null) 
                {{
                    _{1}BLL = new {0}BLL();
                }}
                return _{1}BLL;
            }}
        }}

        
        public int Save({0} {1})
        {{ 
            using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                return this.{0}BLL.Save({1});
            }}

        }}

        public {0} Get({0} {1})
        {{
           using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                return this.{0}BLL.Get({1});
            }}
        }}

        public void Update({0} {1})
        {{
            using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                this.{0}BLL.Update({1});
            }}
        }}

        public void Delete({0} {1})
        {{
             using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                this.{0}BLL.Delete({1});
            }}
        }}

        public List<{0}> ListModels(int pageSize, int index )
        {{
            using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                return this.{0}BLL.ListModels( pageSize, index );
            }}
        }}

        public List<{0}> GetHistoryData(DateTime from, DateTime to, Guid? deviceKey)
        {{
            using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                return this.{0}BLL.GetHistoryData( from, to, deviceKey );
            }}
        }}


        public int GetModelCount( )
        {{
            using (ConnectionSessionScope scope = new ConnectionSessionScope())
            {{
                return this.{0}BLL.GetModelCount( );
            }}
        }}
       {2}
    }}
}}
", t.AName, t.AName.ToLower(),
 t.IsSnapshot ? genBLLServiceHostSnapshot(t) : ""
 );




            if (!Directory.Exists("D:\\BLLHOST"))
            {
                Directory.CreateDirectory("D:\\BLLHOST");
            }
            using (StreamWriter sw = new StreamWriter("D:\\BLLHOST" + "\\" + "BLLServices." + t.AName + "BLL.cs"))
            {
                sw.Write(sb.ToString());
            }

            if (!Directory.Exists("D:\\BLLHOST_IIS"))
            {
                Directory.CreateDirectory("D:\\BLLHOST_IIS");
            }

            ///for IIS WCF svc
            sb = new StringBuilder();
            sb.AppendFormat(@"<%@ServiceHost language=C# Service=""RDH.ServiceHost.{0}"" %> ", t.AName + "BLLService");
            using (StreamWriter sw = new StreamWriter("D:\\BLLHOST_IIS" + "\\" + t.AName + "BLLService.svc"))
            {
                sw.Write(sb.ToString());
            }

        }


    }
}
