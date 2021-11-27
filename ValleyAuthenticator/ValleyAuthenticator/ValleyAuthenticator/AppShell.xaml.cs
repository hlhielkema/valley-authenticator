using System;
using ValleyAuthenticator.Storage;
using ValleyAuthenticator.Views;

namespace ValleyAuthenticator
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DirectoryListPage(AuthenticatorStorage.Instance, null));
        }
    }
}
