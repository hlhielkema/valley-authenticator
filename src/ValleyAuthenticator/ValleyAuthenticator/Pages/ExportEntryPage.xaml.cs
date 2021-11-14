using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValleyAuthenticator.Storage.Models;
using ValleyAuthenticator.Utils;

namespace ValleyAuthenticator.Pages
{
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