using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ValleyAuthenticator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JsonDataPage : ContentPage
    {
        public JsonDataPage(string json)
        {
            InitializeComponent();

            dataEditor.Text = json;
        }
    }
}