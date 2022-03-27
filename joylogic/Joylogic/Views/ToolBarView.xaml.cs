using Joylogic.ViewModels;
using Core.Infrastructure;
using Core.Infrastructure.Controls;
using Core.Infrastructure.ViewModels;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Joylogic.Views
{
    /// <summary>
    /// Interaction logic for LoginHeader.xaml
    /// </summary>

    public partial class ToolBarView : IBaseView
    {
        public ToolBarView(ToolBarViewModel vm)
            : base()
        {
            this.DataContext = vm;
            InitializeComponent();
             
        }
 

        
         
        
    }
}
