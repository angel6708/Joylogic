using Core.Infrastructure;
using Core.Infrastructure.Controls;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LoginModule
{
    /// <summary>
    /// Interaction logic for LoginHeader.xaml
    /// </summary>
    public partial class LoginView : BaseView
    {
        
        public LoginView(LoginViewModel vm) :
            base()
        {
            this.ViewModel = vm;
            InitializeComponent();
        }
           
        private void pwd_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            pwd.Tag = pwd.Password;
        }

        private void Button_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            { 
              
            }
        }
         
    }
}
