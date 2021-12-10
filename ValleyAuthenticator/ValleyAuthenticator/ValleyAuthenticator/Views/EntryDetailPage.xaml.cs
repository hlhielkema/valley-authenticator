using OtpNet;
using System;
using ValleyAuthenticator.Storage;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Info;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EntryDetailPage : ContentPage
    {
        private readonly IOtpEntryContext _entryContext;
        private readonly OtpData _otpData;
        private readonly System.Timers.Timer _timer;
        private readonly Totp _totp;
        private DateTime? _timeCopied;

        public EntryDetailPage(IOtpEntryContext entryContext)
        {
            InitializeComponent();

            _entryContext = entryContext;
            _otpData = entryContext.GetOtpData();

            NameLabel.Text = _otpData.Label;
            IssuerLabel.Text = _otpData.Issuer;

            try
            {
                byte[] base32Bytes = Base32Encoding.ToBytes(_otpData.Secret);
                _totp = new Totp(base32Bytes);
            }
            catch
            {
                CodeLabel.Text = "Invalid secret";
                NextCodeLabel.Text = "";
            }

            if (_totp != null)
            {
                UpdateCode();

                _timer = new System.Timers.Timer(1000)
                {
                    AutoReset = true
                };
                _timer.Elapsed += Timer_Elapsed;
                _timer.Enabled = true;
            }
        }

        private void UpdateCode()
        {
            string code = _totp.ComputeTotp();
                        
            int secondsLeft = 30 - (DateTime.Now.Second % 30);

            if (_timeCopied > DateTime.UtcNow.AddSeconds(-2))
                CodeLabel.Text = "Copied!";
            else
                CodeLabel.Text = string.Format("{0} ({1})", code, secondsLeft);

            NextCodeLabel.IsVisible = secondsLeft <= 15;
            if (NextCodeLabel.IsVisible)
            {
                string nextCode = _totp.ComputeTotp(DateTime.Now.AddSeconds(20));
                NextCodeLabel.Text = nextCode;
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
            _entryContext.Delete();
            await Navigation.PopAsync();
        }

        private async void OnClickedCopyToClipboard(object sender, EventArgs e)
        {
            if (_totp == null)
                return;
            
            string code = _totp.ComputeTotp();
            await Clipboard.SetTextAsync(code);

            _timeCopied = DateTime.UtcNow;
            UpdateCode();
        }

        private async void OnClickedExport(object sender, EventArgs e)
        {
            if (_totp == null)
                return;

            await Navigation.PushAsync(new ExportEntryPage(_otpData));
        }
    }
}