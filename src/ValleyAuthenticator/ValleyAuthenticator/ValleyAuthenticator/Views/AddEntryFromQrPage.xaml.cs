using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Utils;
using ValleyAuthenticator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddEntryFromQrPage : ContentPage
    {
        private IDirectoryContext _createContext;
        private bool _completed;

        public AddEntryFromQrPage(IDirectoryContext createContext)
        {
            // https://github.com/Redth/ZXing.Net.Mobile
            // https://www.youtube.com/watch?v=kwVtlT3E7fw

            InitializeComponent();

            _createContext = createContext;
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
                    if (KeyUriFormat.TryParseAppUri(result.Text, out OtpData data))
                    {                        
                        _completed = true;
                        _createContext.AddEntry(data);
                        await Navigation.PopAsync();
                    }
                }
            });
        }        
    }
}