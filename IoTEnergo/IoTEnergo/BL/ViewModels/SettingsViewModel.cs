using System;
using System.Collections.Generic;
using System.Text;

namespace IoTEnergo.BL.ViewModels
{
    public class SettingsViewModel
    {
        private SettingsViewModel() { }

        public static SettingsViewModel Instance { get; } = new SettingsViewModel();

        public string Id { get; set; }
    }
}
