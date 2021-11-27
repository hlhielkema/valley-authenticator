using System;
using System.Collections.Generic;
using System.Text;
using ValleyAuthenticator.Storage;
using ValleyAuthenticator.Views;
using Xamarin.Forms;

namespace ValleyAuthenticator.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }

        private INavigation _navigation;

        public LoginViewModel(INavigation navigation)
        {
            LoginCommand = new Command(OnLoginClicked);
            _navigation = navigation;
        }

        private async void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");

            AuthenticatorStorage storage = new AuthenticatorStorage();
            storage.AddTestData();

            await _navigation.PushAsync(new DirectoryListPage(storage, null));
        }
    }
}
