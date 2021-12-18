﻿using System.Collections.Generic;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExportEntryPage : ContentPage
    {
        public ExportEntryPage(OtpData otpData)
        {
            InitializeComponent();

            string dataUri = TotpUtilities.GenerateAppUri(otpData);
            //ExportUriLabel.Text = dataUri;

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
    }
}