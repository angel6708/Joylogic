using Core.Infrastructure;
using Core.Infrastructure.Controls;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TestModule
{
    /// <summary>
    /// Interaction logic for TestHeader.xaml
    /// </summary>
    public partial class SelectCaseView : BaseView
    {

        public SelectCaseView(SelectCaseViewModel vm) :
            base()
        {
            this.ViewModel = vm;
            InitializeComponent();
        } 
         
    }
}
