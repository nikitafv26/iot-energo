using IoTEnergo.BL.ViewModels.Account;
using IoTEnergo.BL.ViewModels.Device;
using IoTEnergo.BL.ViewModels.Map;
using IoTEnergo.DAL.Services;
using IoTEnergo.DAL.Services.Abstract;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoTEnergo
{
    public class IoTEnergoNinjectModule : NinjectModule
    {
        public override void Load()
        {
            //Bind<DeviceViewModel>().ToSelf();
            //Bind<LoginViewModel>().ToSelf();
            //Bind<MapViewModel>().ToSelf();
            //Bind<INavigationService>().To<NavigationService>();
            //Bind<ISettingsService>().To<SettingsService>();

            Bind<IRequestProvider>().To<RequestProvider>();

            //Bind<IEquipmentRepository>().To<EquipmentRepository>();
            //Bind<IRepairRepository>().To<RepairRepository>();
            //Bind<ISpareRepository>().To<SpareRepository>();
            //Bind<IUserRepository>().To<UserRepository>();

            //if use mock services we should update dependencies
            UpdateServiceDependencies(GlobalSettings.Instance.UseMockService);
        }

        private void UpdateServiceDependencies(bool useMockServices)
        {
            if (!useMockServices)
            {
                Bind<IAccountService>().To<AccountService>();
                Bind<IDeviceService>().To<DeviceService>();
            }
            else
            {
                //Bind<IAccountService>().To<AccountMockService>();
            }
        }
    }
}
