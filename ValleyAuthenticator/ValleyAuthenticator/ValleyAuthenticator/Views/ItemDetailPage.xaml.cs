using System.ComponentModel;
using ValleyAuthenticator.ViewModels;
using Xamarin.Forms;

namespace ValleyAuthenticator.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}