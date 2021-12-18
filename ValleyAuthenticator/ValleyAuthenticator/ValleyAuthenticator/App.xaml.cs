using ValleyAuthenticator.Storage;
using Xamarin.Forms;

namespace ValleyAuthenticator
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<AuthenticatorStorage>();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            
        }       

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
