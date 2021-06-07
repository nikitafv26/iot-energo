using IoTEnergo.UI.Pages.Chart;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace IoTEnergo.UI.Pages.Map
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();




        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

            if (permissionStatus != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await DisplayAlert("Need location", "Gunna need that location", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                permissionStatus = results[Permission.Location];
            }



            var map = new Xamarin.Forms.Maps.Map(
           MapSpan.FromCenterAndRadius(
                   new Position(45.04903, 42.00356), Distance.FromMiles(0.3)))
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var pin = new Pin()
            {
                Position = new Position(45.04903, 42.00356),
                Label = "PnP_GW_00003403DE7B5C29"
            };

            pin.Clicked += (sender, e) =>
            {
                Shell.Current.Navigation.PushAsync(new ChartPage(), true);
            };

            map.Pins.Add(pin);

            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(map);
            Content = stack;
        }
    }
}