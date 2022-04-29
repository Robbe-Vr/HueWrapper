using HueWrapper.Core.Default;
using HueWrapper.Core.Entertainment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SYWPipeNetworkManager;

namespace HueWrapper.Core
{
    public class HueManager
    {
        private IEnumerable<string> validatedSources = new List<string>()
        {
            "MidiDomotica"
        };
        
        public bool Setup()
        {
            Preferences preferences = new Preferences();
            preferences.Load();

            Task<bool> bridgeSetup = HueBridge.Setup(preferences);
            if (!bridgeSetup.Wait(TimeSpan.FromSeconds(10)) || !bridgeSetup.Result)
            {
                return false;
            }

            if (!EntertainmentManager.Setup(preferences).Wait(TimeSpan.FromSeconds(20)))
            {
                return false;
            }

            PipeMessageControl.Init("Hue");
            PipeMessageControl.StartClient(
                (sourceName, message) =>
                {
                    if (ValidateSource(sourceName))
                    {
                        return $"{message} -> " + ProcessMessage(message);
                    }
                    else return $"{message} -> NO";
                }
            );

            return true;
        }

        private bool ValidateSource(string source)
        {
            return validatedSources.Contains(source);
        }

        public string ProcessMessage(string message)
        {
            IEnumerable<string> parts = new Regex(@"(::\[|\]::|::)|]$").Split(message).Where(x => !String.IsNullOrWhiteSpace(x) && !x.Contains("::")).Select(x => x.Trim());

            switch (parts.FirstOrDefault())
            {
                case "Get":
                    return ProcessGetCommand(parts.Skip(1));

                case "Connect":
                    return ProcessConnectCommand(parts.Skip(1));

                case "Lights":
                    return ProcessLightsCommand(parts.Skip(1));

                case "Room":
                    return ProcessRoomCommand(parts.Skip(1));

                case "Entertainment":
                    return ProcessEntertainmentCommand(parts.Skip(1));
            }

            return "INVALID DATA";
        }

        private string ProcessGetCommand(IEnumerable<string> parts)
        {
            switch (parts.FirstOrDefault())
            {
                case "Available":
                    return ProcessAvailableCommand(parts.Skip(1));
            }

            return "UNKNOWN GET";
        }

        private string ProcessConnectCommand(IEnumerable<string> parts)
        {
            switch (parts.FirstOrDefault())
            {
                case "Bridge":
                    return HueBridge.ConnectToBridge(parts.ElementAtOrDefault(1));
            }

            return "UNKNOWN GET";
        }

        private string ProcessAvailableCommand(IEnumerable<string> parts)
        {
            switch (parts.FirstOrDefault())
            {
                case "Lights":
                    return HueControl.GetLights();

                case "Rooms":
                    return HueControl.GetRooms();

                case "Entertainment":
                    return HueControl.GetEntertainment(parts.Skip(1));

                case "Bridge":
                    return HueControl.GetBridges();
            }

            return string.Empty;
        }

        private string ProcessEntertainmentCommand(IEnumerable<string> parts)
        {
            EntertainmentControl entertainment = new EntertainmentControl(parts.First());
            parts = parts.Skip(1);
            return entertainment.Process(parts);
        }

        private string ProcessRoomCommand(IEnumerable<string> parts)
        {
            HueControl room = new HueControl(parts.First());
            parts = parts.Skip(1);
            return room.Process(parts);
        }

        private string ProcessLightsCommand(IEnumerable<string> parts)
        {
            HueControl lights = new HueControl(parts.First().Split(','));
            parts = parts.Skip(1);
            return lights.Process(parts);
        }
    }
}
