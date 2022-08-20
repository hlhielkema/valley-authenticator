using OtpNet;
using System;
using System.Collections.Generic;
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
        private readonly System.Timers.Timer _timer;
        private readonly Otp _otp;
        private OtpData _otpData;
        private DateTime? _timeCopied;

        public EntryDetailPage(IOtpEntryContext entryContext)
        {
            InitializeComponent();

            _entryContext = entryContext;
            _otpData = entryContext.OtpData;

            if (_otpData.Type == OtpType.Hotp)
                NextCodeFrame.IsVisible = true;

            List<TextCell> cells = new List<TextCell>()
            {
                new TextCell()
                {
                    Text = "Label (username)",
                    Detail = _otpData.Label,
                },
                new TextCell()
                {
                    Text = "Issuer (website)",
                    Detail = _otpData.Issuer
                }
            };

            foreach (TextCell cell in cells)
            {
                cell.Tapped += Modify_Tapped;
                entryInformation.Add(cell);
            }            

            try
            {
                OtpHashMode mode = Convert(_otpData.Algorithm);

                if (_otpData.Type == OtpType.Totp)
                {

                    byte[] base32Bytes = Base32Encoding.ToBytes(_otpData.Secret);

                    _otp = new Totp(base32Bytes, _otpData.Period, mode, _otpData.Digits);
                }
                else // Hotp
                {
                    byte[] base32Bytes = Base32Encoding.ToBytes(_otpData.Secret);

                    _otp = new Hotp(base32Bytes, mode, _otpData.Digits);
                }
            }
            catch
            {
                CodeLabel.Text = "Invalid";
                NextCodeLabel.Text = "";
            }

            if (_otp != null)
            {
                UpdateTotpCode();

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

        private void UpdateTotpCode()
        {
            if (_otp is Totp totp)
            {
                string code = totp.ComputeTotp();
                int remainingSeconds = totp.RemainingSeconds();

                if (_timeCopied > DateTime.UtcNow.AddSeconds(-2))
                    CodeLabel.Text = "Copied!";
                else
                    CodeLabel.Text = string.Format("{0} ({1})", code, remainingSeconds);

                NextCodeLabel.IsVisible = remainingSeconds <= 15;
                if (NextCodeLabel.IsVisible)
                {
                    string nextCode = totp.ComputeTotp(DateTime.Now.AddSeconds(20));
                    NextCodeLabel.Text = string.Format("Next: {0}", nextCode);
                }
            }
            else if (_otp is Hotp hotp)
            {
                CodeLabel.Text = hotp.ComputeHOTP(_otpData.Counter);                
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => {
                UpdateTotpCode();
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

        private async void OnClickedCopyToClipboard(object sender, EventArgs e)
        {
            if (_otp == null)
                return;

            if (_otp is Totp totp)
            {
                string code = totp.ComputeTotp();
                await Clipboard.SetTextAsync(code);
            }
            else
            {
                throw new NotImplementedException();
            }
            

            _timeCopied = DateTime.UtcNow;
            UpdateTotpCode();
        }
     
        private async void Export_Tapped(object sender, EventArgs e)
        {
            if (_otp == null)
                return;

            await Navigation.PushAsync(new ExportEntryPage(_entryContext));
        }

        private async void Modify_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditEntryPage(_entryContext.CreateEditFormContext()));
        }

        private async void Delete_Tapped(object sender, EventArgs e)
        {
            // Ask for confirmation
            string questions = String.Format("Are you sure you want to delete this {0}?", _entryContext.TypeDisplayName);
            if (!await DisplayAlert("Confirm delete", questions, "Yes", "No"))
                return;

            _entryContext.Delete();
            await Navigation.PopAsync();
        }

        private void NextCode_Tapped(object sender, EventArgs e)
        {
            if (_otp is Hotp hotp)
            {
                _otpData = _otpData.Next();
                _entryContext.OtpData = _otpData;
                CodeLabel.Text = hotp.ComputeHOTP(_otpData.Counter);
            }
        }
    }
}