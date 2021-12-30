using System;
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
        private bool _completed;

        public DirectoryImportExportPage(IDirectoryContext directoryContext)
        {
            _directoryContext = directoryContext;

            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            if (_completed)            
                await Navigation.PopAsync();            
            base.OnAppearing();
        }

        private async void ImportAsKeyUri_Tapped(object sender, EventArgs e)
        {
            bool callback(string data)
            {
                _completed = _directoryContext.TryImport(ExportFormat.KeyUri, data, true);
                return _completed;
            }
            await Navigation.PushAsync(new TextDataPage(callback));
        }

        private async void ImportMultipleAsJson_Tapped(object sender, EventArgs e)
        {
            bool callback(string data)
            {
                _completed = _directoryContext.TryImport(ExportFormat.Json, data, true);
                return _completed;
            }
            await Navigation.PushAsync(new TextDataPage(callback));
        }

        private async void ImportSingleAsJson_Tapped(object sender, EventArgs e)
        {
            bool callback(string data)
            {
                _completed = _directoryContext.TryImport(ExportFormat.Json, data, false);
                return _completed;
            }
            await Navigation.PushAsync(new TextDataPage(callback));
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