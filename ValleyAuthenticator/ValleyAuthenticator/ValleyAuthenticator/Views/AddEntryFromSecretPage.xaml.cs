using System;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEntryFromSecretPage : ContentPage
    {
        private IDirectoryContext _createContext;

        public AddEntryFromSecretPage(IDirectoryContext createContext)
        {
            InitializeComponent();

            _createContext = createContext;
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            string label = labelEditor.Text;
            string secret = secretEditor.Text;
            string issuer = issuerEditor.Text;

            if (!string.IsNullOrWhiteSpace(label) && !string.IsNullOrWhiteSpace(secret) && !string.IsNullOrWhiteSpace(issuer))
            {
                _createContext.AddEntry(new OtpData(OtpType.Totp, label, secret, issuer));
                await Navigation.PopAsync();
            }
        }
    }
}