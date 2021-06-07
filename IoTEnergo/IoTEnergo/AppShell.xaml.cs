using IoTEnergo.UI.Pages.Account;
using IoTEnergo.UI.Pages.Chart;
using IoTEnergo.UI.Pages.Device;
using IoTEnergo.UI.Pages.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IoTEnergo
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppShell : Shell
	{
		public AppShell ()
		{
			InitializeComponent ();
            BindingContext = this;
            RegisterRoutes();

        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("device", typeof(DevicePage));
            Routing.RegisterRoute("map", typeof(MapPage));
            Routing.RegisterRoute("device/chart", typeof(ChartPage));
            Routing.RegisterRoute("//device/chart/daterange", typeof(DateRangePage));
            Routing.RegisterRoute("chart/daterange", typeof(DateRangePage));
        }

        void GoToLogin()
        {
            Shell.Current.Navigation.PushModalAsync(new LoginPage(), true);
        }
	}
}