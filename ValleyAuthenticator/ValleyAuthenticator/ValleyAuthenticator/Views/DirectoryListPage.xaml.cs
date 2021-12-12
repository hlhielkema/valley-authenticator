﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private readonly IDirectoryContext _directoryContext;
        private ObservableCollection<NodeInfo> _viewedItems;
        private ISearchContext _searchContext;
        private bool _searching;
        private bool _childActive;
        private bool _disposed;
        private bool _isRoot;

        public DirectoryListPage()
            : this(AuthenticatorStorage.GetRootDirectoryContext(), isRoot: true)
        { }

        public DirectoryListPage(IDirectoryContext directoryContext)
            : this(directoryContext, isRoot: false)
        { }

        private DirectoryListPage(IDirectoryContext directoryContext, bool isRoot)
        {
            InitializeComponent();
            
            _directoryContext = directoryContext;
            _isRoot = isRoot;
            ObservableCollection<NodeInfo> items = directoryContext.ListAndSubscribe();
            
            ItemsView.ItemsSource = items;
            _viewedItems = items;

            items.CollectionChanged += (sender, e) =>
            {
                UpdateImageState();
            };

            UpdateImageState();            
        }

        protected override void OnAppearing()
        {
            if (_disposed)
                throw new Exception("Page disposed");

            // Validate if a child page popped
            if (_childActive)                
                _directoryContext.Validate();

            _childActive = false;            
            if (_searchContext != null)
                _searchContext.Validate();

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            if (!_childActive && !_isRoot)
            {
                // Page popped

                _disposed = true;
                ItemsView.ItemsSource = null;
                _viewedItems = null;

                if (_searchContext != null)                
                    _searchContext.Dispose();                
            }
        }   

        public void MainSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = MainSearchBar.Text;

            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                // Reset
                if (_searching)
                {
                    _searching = false;
                    _viewedItems = _directoryContext.ListAndSubscribe();
                    ItemsView.ItemsSource = _viewedItems;

                }
            }
            else
            {
                if (_searching)
                {
                    _searchContext.Update(searchQuery);
                }
                else
                {

                    if (_searchContext == null)
                    {
                        _searchContext = _directoryContext.CreateSearchContext();                        
                    }

                    _searching = true;
                    _viewedItems = _searchContext.ListAndSubscribe();
                    ItemsView.ItemsSource = _viewedItems;
                }                
            }

            UpdateImageState();
        }

        private void UpdateImageState()
        {
            bool anyItems = _viewedItems != null && _viewedItems.Count > 0;
            ItemsView.IsVisible = anyItems;
            NoItemsView.IsVisible = !anyItems;
        }        

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            NodeInfo item = (NodeInfo)e.Item;

            if (item.Context is IDirectoryContext directoryContext)
            {
                _childActive = true;
                await Navigation.PushAsync(new DirectoryListPage(directoryContext));
            }
            else if (item.Context is IOtpEntryContext otpEntryContext)
            {
                _childActive = true;
                await Navigation.PushAsync(new EntryDetailPage(otpEntryContext));
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
                    _childActive = true;
                    await Navigation.PushAsync(new AddEntryFromQrPage(_directoryContext));
                    return;

                case "Enter secret":
                    _childActive = true;
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