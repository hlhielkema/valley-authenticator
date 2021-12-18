using System;
using System.Collections.Generic;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExportEntryPage : ContentPage
    {
        private IOtpEntryContext _entryContext;

        public ExportEntryPage(IOtpEntryContext entryContext)
        {
            InitializeComponent();

            _entryContext = entryContext;

            OtpData otpData = entryContext.GetOtpData();

            string dataUri = TotpUtilities.GenerateAppUri(otpData);            
            qrView.BarcodeValue = dataUri;
            qrView.IsVisible = true;

            foreach (KeyValuePair<string, string> item in TotpUtilities.ListInformation(otpData))
            {
                entryInformation.Add(new TextCell()
                {
                    Text = item.Key,
                    Detail = item.Value
                });
            }
        }

        private async void ExportToJson_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new JsonDataPage(_entryContext.ExportToJson()));
        }
    }
}