using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage;
using ValleyAuthenticator.Storage.Info;
using ValleyAuthenticator.Storage.Models;
using ValleyAuthenticator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DirectoryListPage : ContentPage
    {
        public ObservableCollection<AuthNodeInfo> Items { get; set; }

        private readonly AuthenticatorStorage _storage;
        private Guid? _directoryId;

        public DirectoryListPage()
            : this(AuthenticatorStorage.Instance, null)
        { }

        public DirectoryListPage(AuthenticatorStorage storage, Guid? directoryId)
        {
            InitializeComponent();

            _storage = storage;
            _directoryId = directoryId;



            Items = new ObservableCollection<AuthNodeInfo>();

            ItemsView.ItemsSource = Items;
        }

        protected override void OnAppearing()
        {
            ReloadContent();
            base.OnAppearing();
        }

        private void ReloadContent()
        {
            List<AuthNodeInfo> items = _storage.GetForDirectory(_directoryId);            
            MergeObservableCollection.Replace(Items, items);

            bool anyItems = Items.Count > 0;

            ItemsView.IsVisible = anyItems;
            NoItemsView.IsVisible = !anyItems;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            AuthNodeInfo item = (AuthNodeInfo)e.Item;

            switch (item.Type)
            {
                case NodeType.Directory:
                    await Navigation.PushAsync(new DirectoryListPage(_storage, item.Id));
                    break;
                case NodeType.OtpEntry:
                    await Navigation.PushAsync(new EntryDetailPage(_storage, item.Id));
                    break;

            }

            // Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
       
        private async void OnClickedAdd(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Add", "Cancel", null, "Scan QR", "Enter secret", "New directory");

            switch (action)
            {
                case "Scan QR":
                    await Navigation.PushAsync(new AddEntryFromQrPage(_storage, _directoryId));
                    return;

                case "Enter secret":
                    await Navigation.PushAsync(new AddEntryFromSecretPage(_storage, _directoryId));
                    return;

                case "New directory":
                    string name = await DisplayPromptAsync("Create new directory", "Enter name");
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        _storage.AddDirectory(_directoryId, name);
                        ReloadContent();
                    }
                    return;
            }
        }

        public async void OnDelete(object sender, EventArgs e)
        {
            AuthNodeInfo item = (AuthNodeInfo)((MenuItem)sender).CommandParameter;

            // Ask for confirmation
            string questions = String.Format("Are you sure you want to delete this {0}?", item.Type.ToString().ToLower());
            if (!await DisplayAlert("Confirm delete", questions, "Yes", "No"))
                return;

            switch (item.Type)
            {
                case NodeType.Directory:
                    _storage.DeleteDirectory(item.Id);
                    break;
                case NodeType.OtpEntry:
                    _storage.DeleteEntry(item.Id);
                    break;

            }

            ReloadContent();
        }
    }
}