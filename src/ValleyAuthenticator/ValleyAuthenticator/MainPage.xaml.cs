using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using ValleyAuthenticator.Pages;
using ValleyAuthenticator.Storage;

namespace ValleyAuthenticator
{
	public partial class MainPage : ContentPage
	{
		//int count = 0;

		public MainPage()
		{
			InitializeComponent();
		}

		private void OnCounterClicked(object sender, EventArgs e)
		{
			//count++;
			//CounterLabel.Text = $"Current count: {count}";

			//SemanticScreenReader.Announce(CounterLabel.Text);

			AuthenticatorStorage storage = new AuthenticatorStorage();
			storage.AddTestData();

			Navigation.PushAsync(new DirectoryListPage(storage, null));
		}
	}
}
