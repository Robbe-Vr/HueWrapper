using HueWrapper.Core.Default;
using Q42.HueApi.Models.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HueWrapper.Core.Entertainment
{
    internal static class EntertainmentManager
    {
        internal static IDictionary<string, bool> ActiveGroups { get; } = new Dictionary<string, bool>();

        internal static HueEntertainmentHandler Handler { get; private set; }

        internal static async Task Setup(Preferences preferences)
        {
            IReadOnlyCollection<Group> eGroups = await HueBridge.Client.GetEntertainmentGroups();

            foreach (Group eGroup in eGroups)
            {
                ActiveGroups.Add(eGroup.Id + "::" + eGroup.Name, eGroup.Stream.Active);
            }

            Handler = new HueEntertainmentHandler(preferences);
        }

        internal static string[] GetModes()
        {
            return new string[]
            {
                "HitSync",
            };
        }

        internal static bool GetRoom(string room)
        {
            return ActiveGroups.Keys.Any(x => x.Split("::")[1] == room) ? ActiveGroups.FirstOrDefault(x => x.Key.Split("::")[1] == room).Value : false;
        }

        internal static void SetRoomActive(string room)
        {
            ActiveGroups[ActiveGroups.Keys.FirstOrDefault(x => x.Split("::")[1] == room)] = true;
        }

        internal static void SetRoomInactive(string room)
        {
            ActiveGroups[ActiveGroups.Keys.FirstOrDefault(x => x.Split("::")[1] == room)] = false;
        }
    }
}
