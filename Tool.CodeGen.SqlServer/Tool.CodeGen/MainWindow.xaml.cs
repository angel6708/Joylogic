using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Tool.CodeGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<Schema> _schemas;
        public List<Schema> Schemas
        {
            get { return _schemas; }
            set
            {
                _schemas = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Schemas"));
                }
            }
        }

        private List<Ref> _refs;
        public List<Ref> Refs
        {
            get { return _refs; }
            set
            {
                _refs = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Refs"));
                }
            }
        }

        private string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                _SearchText = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SearchText"));
                }

                if (this.SelectedSchema != null && this.SelectedSchema.Tables != null)
                {
                    var items = this.SelectedSchema.Tables.Where(a => a.Name.ToLower().StartsWith(_SearchText.ToLower()));
                    object toview = null;
                    if (items.Count() > 10)
                    {
                        toview = items.ElementAt(10);
                    }
                    else
                    {
                        toview = items.LastOrDefault();
                    }
                    if (toview != null)
                    {
                        this.vwTab.ScrollIntoView(toview);
                    }
                }
            }
        }

        private Schema _selectedSchema;
        public Schema SelectedSchema
        {
            get
            {
                return _selectedSchema;

            }

            set
            {
                _selectedSchema = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedSchema"));
                }
            }
        }

        private bool _isSelectAll = true;
        public bool IsSelectAll
        {
            get { return _isSelectAll; }
            set
            {
                _isSelectAll = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsSelectAll"));
                }
                if (this.SelectedSchema != null && this.SelectedSchema.Tables != null)
                {
                    foreach (var t in this.SelectedSchema.Tables)
                    {
                        t.Selected = value;
                    }
                }
            }
        }


        private Table _selectedTab;
        public Table SelectedTab
        {
            get
            {
                return _selectedTab;

            }

            set
            {
                _selectedTab = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedTab"));
                }
            }
        }


        private string _codeBaseDir;
        public string CodeBaseDir
        {
            get
            {
                return _codeBaseDir;

            }

            set
            {
                _codeBaseDir = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CodeBaseDir"));
                }
            }
        }

        private string _Assembly = "RDH.Data";
        public string Assembly
        {
            get
            {
                return _Assembly;

            }

            set
            {
                _Assembly = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Assembly"));
                }
            }
        }

        private string _NameSpace = "RDH.Data";
        public string NameSpace
        {
            get
            {
                return _NameSpace;

            }

            set
            {
                _NameSpace = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("NameSpace"));
                }
            }
        }

        private int _MaxDataLen = 1000;
        public int MaxDataLen
        {
            get { return _MaxDataLen; }
            set
            {
                _MaxDataLen = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("MaxDataLen"));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Schemas = new DB().GetAllSchemas();
            this.DataContext = this;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }



        public event PropertyChangedEventHandler PropertyChanged;


        private void cbSchema_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var schema = cbSchema.SelectedItem as Schema;
            if (schema != null)
            {
                LoadSchema(schema);
            }
        }

        private void LoadSchema(Schema schema)
        {
            schema.Tables = new DB().GetAllTabs(schema);
            schema.Tables.Sort((x, y) => x.AName.CompareTo(y.AName));
            schema.Refs = new List<Ref>();
            schema.Views = new List<Table>();
           // schema.Views = new DB().GetAllViews(schema);
           // schema.Refs = new DB().GetAllRefs(schema);

            foreach (var t in schema.Tables)
            {
                t.Columns = new DB().GetAllCols(schema, t);
                if (t.Columns.Where(a => a.Name.ToLower() == "snapshot_key").Count() == 1) 
                {
                    t.IsSnapshot = true;
                }
                t.Refs = new List<Ref>();
              //  t.Refs = new List<Ref>(SelectedSchema.Refs.Where(a => a.ManyTable == t.Name));
            }
        }

        private void vwTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnGen_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedSchema != null)
            {
                BuildAll(SelectedSchema);

            }
        }

        void BuildAll(Schema schema)
        {
            
            CheckSchema(schema);
           // BuildServiceHostConfigFile(schema);
           // BuildServiceClientConfigFile(schema);
            foreach (var t in schema.Tables)
            {                
                if (t.Selected)
                    if (!t.isSnapshotBase)
                    {
                        GenerateBLLClassFile(t);
                        GenerateModelClassFile(t);
                       // this.GenerateBLLServiceHost(t);
                    }
            }
        }
        ///gen c111

        private bool CheckSchema(Schema s)
        {
            var todelTables = new List<Table>();
            foreach (var t in s.Tables)
            {
                if (!CheckTable(t))
                {
                    todelTables.Add(t);
                }
            }
            s.Tables.RemoveAll(a => todelTables.Contains(a));

            return true;
        }

        private bool CheckTable(Table t)
        {
            if (t.Columns.Where(a => a.IsPK&&a.Selected).Count() == 0)
            {
                MessageBox.Show(t.AName + " No Pkbu, 不会生成代码");
                return false;
            }
            return true;
        }

    }
}
