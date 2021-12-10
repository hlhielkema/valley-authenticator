using System;
using ValleyAuthenticator.Storage;
using ValleyAuthenticator.Storage.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEntryFromSecretPage : ContentPage
    {
        private readonly AuthenticatorStorage _storage;
        private Guid? _directory;

        public AddEntryFromSecretPage(AuthenticatorStorage storage, Guid? directory)
        {
            InitializeComponent();

            _storage = storage;
            _directory = directory;
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            string label = labelEditor.Text;
            string secret = secretEditor.Text;
            string issuer = issuerEditor.Text;
            if (!string.IsNullOrWhiteSpace(label) && !string.IsNullOrWhiteSpace(secret) && !string.IsNullOrWhiteSpace(issuer))
            {
                _storage.AddEntry(_directory, new OtpData(EntryType.Totp, label, secret, issuer));
                await Navigation.PopAsync();
            }
        }
    }
}