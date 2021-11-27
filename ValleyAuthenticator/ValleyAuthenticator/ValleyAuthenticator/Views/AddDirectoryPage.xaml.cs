using System;
using ValleyAuthenticator.Storage;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddDirectoryPage : ContentPage
    {
        private readonly AuthenticatorStorage _storage;
        private Guid? _directory;

        public AddDirectoryPage(AuthenticatorStorage storage, Guid? directory)
        {
            InitializeComponent();

            _storage = storage;
            _directory = directory;
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            string name = nameEditor.Text;
            if (!string.IsNullOrWhiteSpace(name))
            {
                _storage.AddDirectory(_directory, name);
                await Navigation.PopAsync();
            }
        }
    }
}