using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TextDataPage : ContentPage
    {
        public TextDataPage(string json)
        {
            InitializeComponent();

            dataEditor.Text = json;
        }

        private async void OnClickedCopy(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(dataEditor.Text);
        }
    }
}