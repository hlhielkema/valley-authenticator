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

            ToolbarItems.Remove(CopyButton);            
        }

        public TextDataPage(string json)
        {
            InitializeComponent();

            dataEditor.Text = json;
            dataEditor.IsReadOnly = true;

            ToolbarItems.Remove(PasteButton);
            ToolbarItems.Remove(AcceptButton);
        }

        private async void OnClickedCopy(object sender, EventArgs e)
        {            
            await Clipboard.SetTextAsync(dataEditor.Text);
        }

        private async void OnClickedPaste(object sender, EventArgs e)
        {            
            dataEditor.Text = await Clipboard.GetTextAsync();
        }

        private async void OnClickedAccept(object sender, EventArgs e)
        {
            if (_callback == null)
                throw new InvalidOperationException();

            if (_callback(dataEditor.Text))
                await Navigation.PopAsync();
        }
    }
}