using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValleyAuthenticator.Storage;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEntryFromSecretPage : ContentPage
    {
        private AuthenticatorStorage _storage;
        private Guid? _directory;

        public AddEntryFromSecretPage(AuthenticatorStorage storage, Guid? directory)
        {
            InitializeComponent();

            _storage = storage;
            _directory = directory;
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            string name = nameEditor.Text;
            string secret = secretEditor.Text;
            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(secret))
            {
                _storage.AddEntry(_directory, name, secret);
                await Navigation.PopAsync();
            }
        }
    }
}