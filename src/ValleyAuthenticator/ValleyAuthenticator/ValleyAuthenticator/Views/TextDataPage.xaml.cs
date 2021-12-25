using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TextDataPage : ContentPage
    {
        private Func<string, bool> _callback;

        public TextDataPage(Func<string, bool> callback)
        {
            InitializeComponent();

            _callback = callback;
        }

        public TextDataPage(string json)
        {
            InitializeComponent();

            dataEditor.Text = json;
            dataEditor.IsReadOnly = true;
        }

        private async void OnClickedCopy(object sender, EventArgs e)
        {
            if (_callback == null)
                await Clipboard.SetTextAsync(dataEditor.Text);
            else
            {
                if (string.IsNullOrWhiteSpace(dataEditor.Text))
                {
                    dataEditor.Text = await Clipboard.GetTextAsync();
                }
                else
                {
                    if (_callback(dataEditor.Text))
                        await Navigation.PopAsync();
                }
            }
        }
    }
}