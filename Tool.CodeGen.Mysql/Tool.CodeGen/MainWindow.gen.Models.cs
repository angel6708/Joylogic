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

        private string NullableLanType(Column c)
        {
            if (c.LanType == "enum")
            {
                return c.IsNullable ? c.AName + "Flag" : c.AName + "Flag" + "?";
            }
            if (c.IsNullable)
            {
                if (c.LanType == "string" || c.LanType == "byte[]")
                {
                    return c.LanType;
                }

                return c.LanType + "?";
            }
            return c.LanType;
        }
            

        private class RefComparer : IEqualityComparer<Ref>
        {

            public bool Equals(Ref x, Ref y)
            {
                return x.AName == y.AName;
            }

            public int GetHashCode(Ref obj)
            {
                return obj.AName.GetHashCode();
            }
        }

        private string BuildMembers(Table t)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in t.Columns)
            {
                sb.AppendFormat(@"        private {0} _{1};
", NullableLanType(c), c.AName.ToLower());
            }

            foreach (var r in t.Refs.Distinct(new RefComparer()))
            {
                sb.AppendFormat(@"        private {0} _{1};
", r.AName, r.AName.ToLower());
            }
            return sb.ToString();
        }

        private string BuildProps(Table t)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in t.Columns)
            {

                sb.AppendFormat(@"
        /// <summary>
        /// column:[{3}],desc:[{4}]
        /// </summary>
        [DataMember]
        [Column(Column=""{3}"" )]
        public virtual {0} {1}
        {{
            get {{ return _{2}; }}
            set 
            {{
                _{2} = value;
                OnPropertyChanged(""{1}"");
            }}
        }}", NullableLanType(c), c.AName, c.AName.ToLower(),c.Name,c.Desc);
            }

            foreach (var r in t.Refs.Distinct(new RefComparer()))
            {
                sb.AppendFormat(@" 
        [DataMember]
        public {0} {1}
        {{
            get {{ return _{2}; }}
            set 
            {{
                _{2} = value;
                OnPropertyChanged(""{1}"");
            }}
        }}", r.AName, r.AName, r.AName.ToLower());
            }
            return sb.ToString();
        }

        private void GenerateModelClassFile(Table t)
        {
            //mns,cn,ms,ps
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(
           @"using System;
using System.Collections;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace Data.Models
{{


	[Serializable]
	[DataContract]
     [Table(KeyColumn=""{5}"", TableName=""{4}"" ,IsSnapshot= {7}  )]
	public partial class {1} : IModel , INotifyPropertyChanged
    {{

        
{2}
		
 
		
		#region Public Properties
{3}
 
		#endregion 



 #region ORM

         public object GetParam()
        {{
           return new {{
            {6}   //Params
            }};
        }}
  
 
        #endregion


         protected virtual void OnPropertyChanged(string p) 
        {{
            if (PropertyChanged != null)
            {{
                PropertyChanged(this , new PropertyChangedEventArgs(p));
            }}
        }}

        public virtual event PropertyChangedEventHandler PropertyChanged;

        
		
	}}
	
}}", NameSpace + ".Models", t.AName, BuildMembers(t), BuildProps(t),t.Name,t.Columns.Where(a=>a.IsPK).FirstOrDefault().Name,Generateparams(t),t.IsSnapshot.ToString().ToLower());
             
            string _fileName = t.AName + ".cs";
            if (!Directory.Exists(Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "Models")))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models"));
            }


            using (StreamWriter sw = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models") + "\\" + _fileName))
            {
                sw.Write(sb.ToString());
            }

        }

        string Generateparams(Table t)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in t.Columns)
            {
                sb.AppendFormat(@"
                    {0}_ = this.{0},", c.AName, t.AName.ToLower());

            }
            return sb.ToString();

        }
    }

}
