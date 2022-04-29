using HueWrapper.Core.Entertainment;
using Q42.HueApi;
using Q42.HueApi.Models.Bridge;
using Q42.HueApi.Models.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueWrapper.Core.Default
{
    internal partial class HueControl
    {
        public static string GetLights()
        {
            Task<IEnumerable<Light>> task = HueBridge.Client.GetLightsAsync();
            task.Wait();
            return String.Join(',', task.Result.Select(x => x.Id + ":" + x.Name));
        }

        public static string GetRooms()
        {
            Task<IReadOnlyCollection<Group>> task = HueBridge.Client.GetGroupsAsync();
            task.Wait();
            return String.Join(',', task.Result.Where(x => x.Type != GroupType.Entertainment).Select(x => x.Name));
        }

        public static string GetEntertainment(IEnumerable<string> parts)
        {
            switch (parts.FirstOrDefault())
            {
                case "Rooms":
                    return GetEntertainmentRooms();

                case "Modes":
                    return GetEntertainmentModes();
            }

            return "UNKNOWN GET";
        }

        private static string GetEntertainmentRooms()
        {
            Task<IReadOnlyList<Group>> task = HueBridge.Client.GetEntertainmentGroups();
            task.Wait();
            return String.Join(',', task.Result.Select(x => x.Name));
        }

        private static string GetEntertainmentModes()
        {
            return String.Join(',', EntertainmentManager.GetModes());
        }

        public static string GetBridges()
        {
            Task<IEnumerable<LocatedBridge>> task = new HttpBridgeLocator().LocateBridgesAsync(TimeSpan.FromSeconds(5));
            task.Wait(TimeSpan.FromSeconds(6));
            return String.Join(',', task.Result.Select(x => x.BridgeId + ":" + x.IpAddress));
        }
    }
}
