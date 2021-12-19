﻿using System;
using System.Collections.Generic;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using ValleyAuthenticator.Storage.Utils;
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

            string dataUri = KeyUriFormat.GenerateAppUri(otpData);            
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

        private async void ExportToKeyUri_Tapped(object sender, EventArgs e)
        {
            OtpData otpData = _entryContext.GetOtpData();
            string dataUri = KeyUriFormat.GenerateAppUri(otpData);

            await Navigation.PushAsync(new TextDataPage(dataUri));
        }

        private async void ExportToJson_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TextDataPage(_entryContext.ExportToJson()));
        }
    }
}