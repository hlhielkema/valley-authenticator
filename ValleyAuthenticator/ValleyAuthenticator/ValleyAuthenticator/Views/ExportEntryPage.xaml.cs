using ValleyAuthenticator.Storage.Models;
using ValleyAuthenticator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExportEntryPage : ContentPage
    {
        public ExportEntryPage(AuthEntryInfo entryInfo)
        {
            InitializeComponent();

            string dataUri = TotpUtilities.GenerateAppUri(entryInfo.Data);
            //ExportUriLabel.Text = dataUri;

            qrView.BarcodeValue = dataUri;
            qrView.IsVisible = true;            
        }
    }
}