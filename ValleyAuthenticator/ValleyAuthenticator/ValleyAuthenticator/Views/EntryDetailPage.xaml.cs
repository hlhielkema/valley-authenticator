using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValleyAuthenticator.Storage;
using ValleyAuthenticator.Storage.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EntryDetailPage : ContentPage
    {
        private AuthenticatorStorage _storage;
        private Guid _entryId;
        private AuthEntryInfo _entryInfo;
        private System.Timers.Timer _timer;
        private Totp _totp;

        public EntryDetailPage(AuthenticatorStorage storage, Guid entryId)
        {
            InitializeComponent();

            _storage = storage;
            _entryId = entryId;
            _entryInfo = _storage.GetEntry(_entryId);

            NameLabel.Text = _entryInfo.Name;

            try
            {
                byte[] base32Bytes = Base32Encoding.ToBytes(_entryInfo.Secret);
                _totp = new Totp(base32Bytes);
            }
            catch
            {
                CodeLabel.Text = "Invalid secret";
            }

            if (_totp != null)
            {
                UpdateCode();

                _timer = new System.Timers.Timer(1000);
                _timer.AutoReset = true;
                _timer.Elapsed += _timer_Elapsed;
                _timer.Enabled = true;
            }
        }

        private void UpdateCode()
        {
            string code = _totp.ComputeTotp();
            CodeLabel.Text = string.Format("{0} ({1})", code, 60 - DateTime.Now.Second);
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => {
                UpdateCode();
            });
        }

        protected override void OnAppearing()
        {
            if (_timer != null)
                _timer.Enabled = true;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            if (_timer != null)
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

        private async void OnClickedExport(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExportEntryPage(_entryInfo));
        }
    }
}