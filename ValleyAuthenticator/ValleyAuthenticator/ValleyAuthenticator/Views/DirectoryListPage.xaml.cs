using System;
using System.Collections.ObjectModel;
using ValleyAuthenticator.Storage;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DirectoryListPage : ContentPage
    {
        public ObservableCollection<NodeInfo> Items { get; set; }
        private readonly IDirectoryContext _directoryContext;
        //private bool _disposed;

        public DirectoryListPage()
            : this(AuthenticatorStorage.GetRootDirectoryContext())
        { }

        public DirectoryListPage(IDirectoryContext directoryContext)
        {
            InitializeComponent();

            _directoryContext = directoryContext;

            Items = directoryContext.ListAndSubscribe();
            ItemsView.ItemsSource = Items;

            Items.CollectionChanged += (sender, e) =>
            {
                UpdateImageState();
            };

            UpdateImageState();
        }

        private void UpdateImageState()
        {
            bool anyItems = Items.Count > 0;
            ItemsView.IsVisible = anyItems;
            NoItemsView.IsVisible = !anyItems;
        }

        protected override void OnAppearing()
        {
            //if (_disposed)
            //    throw new Exception("Directory list was disposed");

            base.OnAppearing();
        }

        //protected override void OnDisappearing()
        //{
        //    _directoryContext.Unsubscribe(Items);
        //    _disposed = true;
        //    base.OnDisappearing();
        //}        

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
                    }
                    return;
            }
        }

        public async void OnDelete(object sender, EventArgs e)
        {
            NodeInfo item = (NodeInfo)((MenuItem)sender).CommandParameter;

            // Ask for confirmation
            string questions = String.Format("Are you sure you want to delete this {0}?", item.Context.TypeDisplayName);
            if (!await DisplayAlert("Confirm delete", questions, "Yes", "No"))
                return;

            item.Context.Delete();
        }
    }
}