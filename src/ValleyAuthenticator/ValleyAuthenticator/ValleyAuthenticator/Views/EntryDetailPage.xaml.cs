using OtpNet;
using System;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
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
        private readonly Totp _otp;
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
                if (_otpData.Type != OtpType.Totp)
                    throw new NotImplementedException();

                byte[] base32Bytes = Base32Encoding.ToBytes(_otpData.Secret);

                _otp = new Totp(base32Bytes, _otpData.Period, Convert(_otpData.Algorithm), _otpData.Digits);                
            }
            catch
            {
                CodeLabel.Text = "Invalid";
                NextCodeLabel.Text = "";
            }

            if (_otp != null)
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

        private OtpHashMode Convert(string algorithm)
        {
            switch (algorithm)
            {
                case "SHA256": return OtpHashMode.Sha256;
                case "SHA512": return OtpHashMode.Sha512;
                default: return OtpHashMode.Sha1;
            }
        }

        private void UpdateCode()
        {
            string code = _otp.ComputeTotp();
            int remainingSeconds = _otp.RemainingSeconds();

            if (_timeCopied > DateTime.UtcNow.AddSeconds(-2))
                CodeLabel.Text = "Copied!";
            else
                CodeLabel.Text = string.Format("{0} ({1})", code, remainingSeconds);

            NextCodeLabel.IsVisible = remainingSeconds <= 15;
            if (NextCodeLabel.IsVisible)
            {
                string nextCode = _otp.ComputeTotp(DateTime.Now.AddSeconds(20));
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

        private async void OnClickedEdit(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditEntryPage(_entryContext.CreateEditFormContext()));
        }

        private async void OnClickedDelete(object sender, EventArgs e)
        {
            // Ask for confirmation
            string questions = String.Format("Are you sure you want to delete this {0}?", _entryContext.TypeDisplayName);
            if (!await DisplayAlert("Confirm delete", questions, "Yes", "No"))
                return;

            _entryContext.Delete();
            await Navigation.PopAsync();
        }

        private async void OnClickedCopyToClipboard(object sender, EventArgs e)
        {
            if (_otp == null)
                return;
            
            string code = _otp.ComputeTotp();
            await Clipboard.SetTextAsync(code);

            _timeCopied = DateTime.UtcNow;
            UpdateCode();
        }

        private async void OnClickedExport(object sender, EventArgs e)
        {
            if (_otp == null)
                return;

            await Navigation.PushAsync(new ExportEntryPage(_entryContext));
        }
    }
}