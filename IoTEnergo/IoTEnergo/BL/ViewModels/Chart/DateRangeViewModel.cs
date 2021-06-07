using IoTEnergo.BL.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IoTEnergo.BL.ViewModels.Chart
{
    public class DateRangeViewModel : ViewModelBase
    {
        private DateTime _startDate = DateTime.Now;
        private DateTime _endDate = DateTime.Now;

        public DateRangeViewModel()
        {
            string startDate = Preferences.Get("StartDate", string.Empty);
            string endDate = Preferences.Get("EndDate", string.Empty);
            if (!string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate))
            {
                StartDate = DateTime.ParseExact(startDate, "yyyyMMdd", null);
                EndDate = DateTime.ParseExact(endDate, "yyyyMMdd", null);
            }
        }

        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if (value != _startDate)
                {
                    _startDate = value;
                    RaisePropertyChanged(() => StartDate);
                }
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (value != _endDate)
                {
                    _endDate = value;
                    RaisePropertyChanged(() => EndDate);
                }
            }
        }

        public ICommand SaveRangeCommand => new Command(async () =>
        {
            Preferences.Set("StartDate", StartDate.ToString("yyyyMMdd"));
            Preferences.Set("EndDate", EndDate.ToString("yyyyMMdd"));
            await Shell.Current.GoToAsync("..");
        });
    }
}
