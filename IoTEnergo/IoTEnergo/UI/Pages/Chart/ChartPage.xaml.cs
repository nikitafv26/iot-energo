using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IoTEnergo.UI.Pages.Chart
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartPage : ContentPage
    {
        //List<Microcharts.Entry> entries = new List<Microcharts.Entry>
        //{
        //    new Microcharts.Entry(30)
        //    {
        //         Color = SKColor.Parse("#FF1493"),
        //         ValueLabel = "30"
        //    },
        //    new Microcharts.Entry(31)
        //    {
        //         Color = SKColor.Parse("#FF1493"),
        //         ValueLabel = "31"
        //    },
        //    new Microcharts.Entry(30)
        //    {
        //         Color = SKColor.Parse("#FF1493"),
        //         ValueLabel = "30"
        //    },
        //    new Microcharts.Entry(29)
        //    {
        //         Color = SKColor.Parse("#FF1493"),
        //         ValueLabel = "29"
        //    },
        //    new Microcharts.Entry(28)
        //    {
        //         Color = SKColor.Parse("#FF1493"),
        //         ValueLabel = "28"
        //    },
        //    new Microcharts.Entry(32)
        //    {
        //         Color = SKColor.Parse("#FF1493"),
        //         ValueLabel = "32"
        //    }
        //};

        public ChartPage()
        {
            InitializeComponent();


            //Chart1.Chart = new LineChart { Entries = entries };
        }
    }
}