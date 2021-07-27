using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopNow.ViewModels
{
    public class LocationDetailsCreateViewModel
    {
        public double SourceLatitude { get; set; }
        public double SourceLontitude { get; set; }
        public double DestinationLatitude { get; set; }
        public double DestinationLontitude { get; set; }
        public double Distance { get; set; }
        public double Duration { get; set; }
    }
}