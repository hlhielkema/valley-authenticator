using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using ValleyAuthenticator.Pages;
using ValleyAuthenticator.Storage;
using Application = Microsoft.Maui.Controls.Application;

namespace ValleyAuthenticator
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();
			
			AuthenticatorStorage storage = new AuthenticatorStorage();
			storage.AddTestData();
			MainPage = new NavigationPage(new DirectoryListPage(storage, null));

			//MainPage = new NavigationPage(new MainPage());
		}
	}
}
