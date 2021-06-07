using IoTEnergo.BL.ViewModels.Base;
using IoTEnergo.DAL.Models;
using IoTEnergo.DAL.Services;
using IoTEnergo.UI.Pages.Map;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using static IoTEnergo.DAL.Services.RequestProvider;

namespace IoTEnergo.BL.ViewModels.Account
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AccountService _accountService;
        private readonly string token_key = "TOKEN";


        private CancellationTokenSource cts = new CancellationTokenSource();

        private string _name;
        private string _password;

        public LoginViewModel(AccountService accountService)
        {
            _accountService = accountService;
            MessageReceiver.RecievedMessage += RecievedMessage;
            //_accountService.AuthRecievedMessage += RecievedMessage;
        }

        private void Unsubscribe()
        {
            MessageReceiver.RecievedMessage -= RecievedMessage;
            //_accountService.AuthRecievedMessage -= RecievedMessage;
            _accountService.Unsubscribe();
        }

        private void RecievedMessage(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                var authWSResponse = JsonConvert.DeserializeObject<WSResponse>(message);
                if (authWSResponse != null && (authWSResponse.cmd == "auth_resp" || authWSResponse.cmd == "token_auth_resp"))
                {
                    Debug.WriteLine(String.Format("Auth status is {0}", authWSResponse.status));
                    if (authWSResponse.status == true)
                    {
                        Unsubscribe();

                        string token = authWSResponse.token;
                        Preferences.Set("Token", token);

                        SaveUserInfo();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Application.Current.MainPage = new AppShell());
                    }
                    else
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.DisplayAlert("Authorization failed", "Incorrect Name or Password", "Cancel"));
                        Preferences.Set("Login", String.Empty);
                        Preferences.Set("Password", String.Empty);
                    }
                }
            }
        }

        private void SaveUserInfo()
        {
            Preferences.Set("Login", Name);
            Preferences.Set("Password", Password);
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (value != _password)
                {
                    _password = value;
                    RaisePropertyChanged(() => Password);
                }
            }
        }

        public ICommand LoginCommand => new Command(async () =>
        {
            var user = new UserModel
            {
                Name = Name,
                Password = Password
            };

            await _accountService.Login(user, cts);
        });
    }
}
