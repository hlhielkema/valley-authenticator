using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage;
using ValleyAuthenticator.Storage.Models;
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

        public DirectoryListPage(AuthenticatorStorage storage, Guid? directoryId)
        {
            InitializeComponent();

            _storage = storage;
            _directoryId = directoryId;

            Items = new ObservableCollection<AuthNodeInfo>();

            MyListView.ItemsSource = Items;
        }

        protected override void OnAppearing()
        {
            ReloadContent();
            base.OnAppearing();
        }

        private void ReloadContent()
        {
            List<AuthNodeInfo> items = _storage.GetForDirectory(_directoryId);
            Items.Clear();
            foreach (AuthNodeInfo item in items)
                Items.Add(item);
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            AuthNodeInfo item = (AuthNodeInfo)e.Item;

            switch (item.Type)
            {
                case AuthNodeType.Directory:
                    await Navigation.PushAsync(new DirectoryListPage(_storage, item.Id));
                    break;
                case AuthNodeType.Entry:
                    await Navigation.PushAsync(new EntryDetailPage(_storage, item.Id));
                    break;

            }
        }

        private void OnClickedAddDirectory(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddDirectoryPage(_storage, _directoryId));
        }

        private void OnClickedScanQr(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddEntryFromQrPage(_storage, _directoryId));
        }

        private void OnClickedEnterSecret(object sender, EventArgs e)
        {
            Navigation.PushAsync(new AddEntryFromSecretPage(_storage, _directoryId));
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
                case AuthNodeType.Directory:
                    _storage.DeleteDirectory(item.Id);
                    break;
                case AuthNodeType.Entry:
                    _storage.DeleteEntry(item.Id);
                    break;

            }

            ReloadContent();
        }
    }
}