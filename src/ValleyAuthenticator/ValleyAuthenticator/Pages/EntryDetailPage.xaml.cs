using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ValleyAuthenticator.Storage;
using ValleyAuthenticator.Storage.Models;

namespace ValleyAuthenticator.Pages
{
    public partial class EntryDetailPage : ContentPage
    {
        private AuthenticatorStorage _storage;
        private Guid _entryId;
        private AuthEntryInfo _entryInfo;
        private System.Timers.Timer _timer;
        
        public EntryDetailPage(AuthenticatorStorage storage, Guid entryId)
        {
            InitializeComponent();

            _storage = storage;
            _entryId = entryId;
            _entryInfo = _storage.GetEntry(_entryId);

            NameLabel.Text = _entryInfo.Name;
            //CodeLabel.Text = _entryInfo.Secret;
            UpdateCode();

            _timer = new System.Timers.Timer(1000);
            _timer.AutoReset = true;
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;
        }

        private void UpdateCode()
        {
            CodeLabel.Text = string.Format("{0} ({1})", _entryInfo.Secret, 60 - DateTime.Now.Second);
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => {
                UpdateCode();
            });            
        }

        protected override void OnAppearing()
        {
            _timer.Enabled = true;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            _timer.Enabled = false;
            base.OnDisappearing();
        }

        private async void OnClickedDelete(object sender, EventArgs e)
        {            
            _storage.DeleteEntry(_entryId);
            await Navigation.PopAsync();            
        }

        private void OnClickedCopyToClipboard(object sender, EventArgs e)
        {
            // TODO
        }
    }
}