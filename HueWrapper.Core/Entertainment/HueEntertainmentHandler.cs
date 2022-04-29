using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.Streaming;
using Q42.HueApi.Streaming.Effects;
using Q42.HueApi.Streaming.Extensions;
using Q42.HueApi.Streaming.Models;
using HueWrapper.Core;
using static HueWrapper.Core.HueManager;
using SYWCentralLogging;

namespace HueWrapper.Core.Entertainment
{
    internal partial class HueEntertainmentHandler
    {
        private Preferences _preferences;

        public HueEntertainmentHandler(Preferences preferences)
        {
            _preferences = preferences;
        }

        public bool StartEntertainmentMode(string mode, string room)
        {
            switch (mode)
            {
                case "HitSync":
                    StartHitSync(room);
                    return true;
            }

            return false;
        }

        public void Cancel(string room)
        {
            StopHitSync();
        }
    }
}
