using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValleyAuthenticator.Storage;
using ValleyAuthenticator.Storage.Models;
using ValleyAuthenticator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEntryFromQrPage : ContentPage
    {
        private readonly AuthenticatorStorage _storage;
        private Guid? _directory;
        private bool _completed;

        public AddEntryFromQrPage(AuthenticatorStorage storage, Guid? directory)
        {
            // https://github.com/Redth/ZXing.Net.Mobile
            // https://www.youtube.com/watch?v=kwVtlT3E7fw

            InitializeComponent();

            _storage = storage;
            _directory = directory;
            _completed = false;
        }

        private void ZXingScannerView_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Prevent adding multiple entries.
                // This also prevents a second navigation pop.
                if (_completed)
                    return;

                if (result.BarcodeFormat == ZXing.BarcodeFormat.QR_CODE)
                {
                    if (TotpUtilities.TryParseAppUri(result.Text, out OtpData data))
                    {                        
                        _completed = true;
                        _storage.AddEntry(_directory, data);                        
                        await Navigation.PopAsync();
                    }
                }
            });
        }        
    }
}