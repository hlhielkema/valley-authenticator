using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Info;
using ValleyAuthenticator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DirectoryListPage : ContentPage
    {
        public ObservableCollection<NodeInfo> Items { get; set; }

        private readonly IDirectoryContext _directoryContext;

        public DirectoryListPage()
            : this(AuthenticatorStorage.Instance.GetRootDirectoryContext())
        { }

        public DirectoryListPage(IDirectoryContext directoryContext)
        {
            InitializeComponent();

            _directoryContext = directoryContext;

            Items = new ObservableCollection<NodeInfo>();

            ItemsView.ItemsSource = Items;
        }

        protected override void OnAppearing()
        {
            ReloadContent();
            base.OnAppearing();
        }

        private void ReloadContent()
        {
            List<NodeInfo> items = _directoryContext.GetChilds();            
            MergeObservableCollection.Replace(Items, items);

            bool anyItems = Items.Count > 0;

            ItemsView.IsVisible = anyItems;
            NoItemsView.IsVisible = !anyItems;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            NodeInfo item = (NodeInfo)e.Item;

            if (item.Context is IDirectoryContext directoryContext)
                await Navigation.PushAsync(new DirectoryListPage(directoryContext));
            else if (item.Context is IOtpEntryContext otpEntryContext)
                await Navigation.PushAsync(new EntryDetailPage(otpEntryContext));

            // Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
       
        private async void OnClickedAdd(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Add", "Cancel", null, "Scan QR", "Enter secret", "New directory");

            switch (action)
            {
                case "Scan QR":
                    await Navigation.PushAsync(new AddEntryFromQrPage(_directoryContext));
                    return;

                case "Enter secret":
                    await Navigation.PushAsync(new AddEntryFromSecretPage(_directoryContext));
                    return;

                case "New directory":
                    string name = await DisplayPromptAsync("Create new directory", "Enter name");
                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        _directoryContext.AddDirectory(name);
                        ReloadContent();
                    }
                    return;
            }
        }

        public async void OnDelete(object sender, EventArgs e)
        {
            NodeInfo item = (NodeInfo)((MenuItem)sender).CommandParameter;

            // Ask for confirmation
            string questions = String.Format("Are you sure you want to delete this {0}?", item.Type.ToString().ToLower());
            if (!await DisplayAlert("Confirm delete", questions, "Yes", "No"))
                return;

            item.Context.Delete();

            ReloadContent();
        }
    }
}