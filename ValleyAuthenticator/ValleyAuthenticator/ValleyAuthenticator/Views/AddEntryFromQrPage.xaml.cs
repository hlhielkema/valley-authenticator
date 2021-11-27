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

        public AddEntryFromQrPage(AuthenticatorStorage storage, Guid? directory)
        {
            InitializeComponent();

            _storage = storage;
            _directory = directory;
        }

        private void ZXingScannerView_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (result.BarcodeFormat == ZXing.BarcodeFormat.QR_CODE)
                {
                    if (TotpUtilities.TryParseAppUri(result.Text, out OtpData data))
                    {
                        _storage.AddEntry(_directory, data.Label, data.Secret);
                        await Navigation.PopAsync();
                    }
                }
            });
        }

        // https://github.com/Redth/ZXing.Net.Mobile
        // https://www.youtube.com/watch?v=kwVtlT3E7fw
    }
}