using IoTEnergo.BL.ViewModels.Base;
using IoTEnergo.DAL.Services;
using IoTEnergo.DAL.Services.Abstract;
using IoTEnergo.UI.Pages.Account;
using IoTEnergo.UI.Pages.Device;
using System;
using System.Diagnostics;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace IoTEnergo
{
    public partial class App : Application
    {
        private CancellationTokenSource cts = new CancellationTokenSource();
        private IRequestProvider _requestProvider;

        public App()
        {
            InitializeComponent();

            //MainPage = new NavigationPage(new LoginPage());
            MainPage = new AppShell();
            InitApp();
        }

        private void InitApp()
        {

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected async override void OnSleep()
        {
            try
            {
                _requestProvider = ViewModelLocator.Resolve<IRequestProvider>();
                await _requestProvider.CloseAsync();
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
