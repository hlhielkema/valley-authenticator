using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValleyAuthenticator.Storage.Abstract;
using ValleyAuthenticator.Storage.Abstract.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DirectoryImportExportPage : ContentPage
    {
        private readonly IDirectoryContext _directoryContext;

        public DirectoryImportExportPage(IDirectoryContext directoryContext)
        {
            _directoryContext = directoryContext;

            InitializeComponent();
        }

        private async void ImportToKeyUri_Tapped(object sender, EventArgs e)
        {
            await DisplayAlert("Alert", "Import URI", "Cancel");
        }

        private async void ImportToJson_Tapped(object sender, EventArgs e)
        {
            await DisplayAlert("Alert", "Import JSON", "Cancel");
        }

        private async void ExportToKeyUri_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TextDataPage(_directoryContext.Export(ExportFormat.KeyUri)));
        }

        private async void ExportToJson_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TextDataPage(_directoryContext.Export(ExportFormat.Json)));
        }
    }
}