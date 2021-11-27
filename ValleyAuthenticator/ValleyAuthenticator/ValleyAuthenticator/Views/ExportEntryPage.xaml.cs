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

            string dataUri = TotpUtilities.GenerateAppUri(entryInfo.Name, entryInfo.Secret);
            ExportUriLabel.Text = dataUri;
        }
    }
}