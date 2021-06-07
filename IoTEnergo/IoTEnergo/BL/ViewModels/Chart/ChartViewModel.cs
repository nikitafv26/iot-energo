using IoTEnergo.BL.ViewModels.Base;
using IoTEnergo.DAL.Models;
using IoTEnergo.DAL.Services;
using IoTEnergo.DAL.Services.Abstract;
using IoTEnergo.UI.Pages.Account;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IoTEnergo.BL.ViewModels.Chart
{
    public class ChartViewModel : ViewModelBase
    {
        private Microcharts.Chart _chart;
        private List<Microcharts.ChartEntry> entries;

        private string _title;
        private List<ExportData> exportData;

        private IAccountService _accountService;
        private readonly IDeviceService _deviceService;
        private CancellationTokenSource cts = new CancellationTokenSource();

        //private string startDate;

        public ChartViewModel(IDeviceService deviceService, IAccountService accountService)
        {
            _deviceService = deviceService;
            _accountService = accountService;

            Subscribe();

            //Chart = new Microcharts.LineChart();
            //RaisePropertyChanged(() => Chart);

            Task.Factory.StartNew(async () =>
            {
                await AutoLogin();
                await GetData();
            });
        }


        private async Task AutoLogin()
        {
            string name = Preferences.Get("Login", String.Empty);
            string password = Preferences.Get("Password", String.Empty);

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password))
            {
                var user = new UserModel { Name = name, Password = password };
                await _accountService.Login(user, cts);
            }
        }

        private void Unsubscribe()
        {
            MessageReceiver.RecievedMessage -= RecievedMessage;
        }

        private void Subscribe()
        {
            MessageReceiver.RecievedMessage += RecievedMessage;
        }

        private async Task GetData()
        {
            try
            {
                string startDate = Preferences.Get("StartDate", String.Empty);
                string endDate = Preferences.Get("EndDate", String.Empty);
                if (string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    startDate = DateTime.Now.ToString("yyyyMMdd");
                    endDate = DateTime.Now.ToString("yyyyMMdd");
                }

                string id = SettingsViewModel.Instance.Id;
                await _deviceService.Get(startDate, endDate, id, cts);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    RaisePropertyChanged(() => Title);
                }
            }
        }

        private void RecievedMessage(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                var dataWSResponse = JsonConvert.DeserializeObject<DataResp>(message);

                if (dataWSResponse.err_string == "unknown_auth")
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        Shell.Current.Navigation.PushModalAsync(new LoginPage());
                    });
                    Unsubscribe();
                }

                if (dataWSResponse != null && dataWSResponse.cmd == "get_data_resp")
                {
                    Debug.WriteLine(String.Format("get_devices_req status is {0}", dataWSResponse.status));
                    if (dataWSResponse.status == true)
                    {
                        Unsubscribe();

                        entries = new List<Microcharts.ChartEntry>();

                        var data = dataWSResponse.data_list.Select(resp => new DataModel
                        {
                            Value = resp.data
                        });

                        data = data.Reverse();

                        double visibleValuesCount = 10;
                        double count = data.Count();
                        double range = count / visibleValuesCount;
                        int roundedRange = (int)Math.Round(range);
                        
                        if (count < 10)
                            roundedRange = 1;

                        float batteryChargeValue = DecodeBatteryChargeData(data.FirstOrDefault().Value);
                        Title = $"DevId: { SettingsViewModel.Instance.Id} Battery charge: {batteryChargeValue}%";

                        exportData = new List<ExportData>();

                        int i = 0;
                        int k = 0;

                        var avgTemp = new List<float>();

                        foreach (var d in data)
                        {

                            Debug.WriteLine(d.Value);

                            var time = DecodeTimeData(d.Value);
                            Debug.WriteLine(time?.ToString("dd-MM-yyyy H:mm:ss"));

                            float tempValue = DecodeTempData(d.Value);
                            Debug.WriteLine(tempValue.ToString("0.00"));

                            string dd = time?.ToString("dd.MM");
                            string hh = time?.ToString("HH:mm");

                            avgTemp.Add(tempValue);

                            if (k == 0)
                            {
                                entries.Add(new Microcharts.ChartEntry(tempValue)
                                {
                                    Color = SKColor.Parse("#6666ff"),
                                    ValueLabel = tempValue.ToString("0.00"),
                                    Label = $"{hh}  {dd}"
                                });
                            }

                            if (i == roundedRange || k == count - 1)
                            {
                                entries.Add(new Microcharts.ChartEntry(tempValue)
                                {
                                    Color = SKColor.Parse("#6666ff"),
                                    ValueLabel = avgTemp.Average().ToString("0.00"),
                                    Label = $"{hh}  {dd}"
                                });
                                avgTemp = new List<float>();
                                i = 0;
                            }

                            exportData.Add(new ExportData
                            {
                                DeviceId = SettingsViewModel.Instance.Id,
                                Date = time?.ToString("dd-MM-yyyy HH:mm:ss"),
                                Value = tempValue.ToString("0.00")
                            });
                            i++;
                            k++;
                        }

                        Chart = new Microcharts.LineChart
                        {
                            Entries = entries,
                            LabelTextSize = 15,
                            LabelOrientation = Microcharts.Orientation.Vertical,
                            ValueLabelOrientation = Microcharts.Orientation.Horizontal
                        };
                    }
                    //else
                }
            }
        }

        public async Task SendEmail(string subject, string body, List<string> recipients, string attachmentPath)
        {
            try
            {
                var message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients,
                    //Cc = ccRecipients,
                    //Bcc = bccRecipients
                };
                message.Attachments.Add(new EmailAttachment(attachmentPath));
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
        }

        private float DecodeBatteryChargeData(string data)
        {
            try
            {
                string hexData = data.Substring(2, 2);
                int value = int.Parse(hexData, System.Globalization.NumberStyles.AllowHexSpecifier);
                return value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Can't decode data value. {ex.Message}");
                return 0;
            }
        }

        private float DecodeTempData(string data)
        {
            try
            {
                string tempHex = data.Substring(14, 4);
                string littleEndianTempHex = tempHex.Substring(2, 2) + tempHex.Substring(0, 2);
                Int32 intRep = Int32.Parse(littleEndianTempHex, System.Globalization.NumberStyles.AllowHexSpecifier);
                return (float)intRep / 10;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Can't decode data value. {ex.Message}");
                return 0;
            }
        }

        private DateTime? DecodeTimeData(string data)
        {
            try
            {
                string tempHex = data.Substring(6, 8);
                string littleEndianTempHex0 = tempHex.Substring(6, 2) + tempHex.Substring(4, 2);
                string littleEndianTempHex1 = tempHex.Substring(2, 2) + tempHex.Substring(0, 2);
                string littleEndianTempHexRes = littleEndianTempHex0 + littleEndianTempHex1;
                Int32 intRep = Int32.Parse(littleEndianTempHexRes, System.Globalization.NumberStyles.AllowHexSpecifier);
                return DateTimeOffset.FromUnixTimeSeconds(intRep).AddHours(3).DateTime;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Can't decode data value. {ex.Message}");
                return null;
            }
        }

        private byte[] HexToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private float BytesToFloat(byte[] bytes)
        {
            bytes = bytes.Reverse().ToArray();
            return BitConverter.ToSingle(bytes, 0);
        }

        public Microcharts.Chart Chart
        {
            get { return _chart; }
            set
            {
                if (value != _chart)
                {
                    _chart = value;
                    RaisePropertyChanged(() => Chart);
                }
            }
        }

        public ICommand ExportCommand => new Command(async () =>
        {
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), $"export-{Guid.NewGuid()}.csv");
            using (var stream = new System.IO.StreamWriter(path))
            {
                foreach (var ed in exportData)
                {
                    string csvRow = $"{ed.DeviceId},{ed.Date},{ed.Value.Replace(',', '.')}";
                    stream.WriteLine(csvRow);
                    //stream.Flush();
                }
            }

            var recipients = new List<string>();
            //recipients.Add("FedorenkoNIV@energomera.ru");
            await SendEmail("Export", string.Empty, recipients, path);

        });

        public ICommand RangeCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync($"//device/chart/daterange", true);
        });
    }
}
