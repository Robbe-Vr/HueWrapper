using HueWrapper.Core.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYWCentralLogging;

namespace HueWrapper.Core.Entertainment
{
    internal class EntertainmentControl
    {
        private string entertainmentRoom;

        internal EntertainmentControl(string room)
        {
            this.entertainmentRoom = room;
        }

        internal string Process(IEnumerable<string> commandParts)
        {
            switch (commandParts.FirstOrDefault()?.ToUpper())
            {
                case "START":
                    if (!EntertainmentManager.Handler.StartEntertainmentMode(commandParts.Skip(1)?.FirstOrDefault(), entertainmentRoom))
                    {
                        return "UNKNOWN MODE";
                    }
                    EntertainmentManager.SetRoomActive(entertainmentRoom);
                    break;

                case "STOP":
                    EntertainmentManager.Handler.Cancel(entertainmentRoom);
                    EntertainmentManager.SetRoomInactive(entertainmentRoom);
                    break;

                case "SET":
                    return SetEntertainmentValue(commandParts.Skip(1));

                default:
                    return "INVALID COMMAND";
            }

            return "EXECUTED";
        }

        public string SetEntertainmentValue(IEnumerable<string> commandParts)
        {
            try
            {
                switch (commandParts.FirstOrDefault().ToUpper())
                {
                    case "HITSYNC":
                        EntertainmentManager.Handler.SetHitSyncValues(commandParts.Skip(1));
                        break;

                    default:
                        return "INVALID PROPERTY";
                }

                return "EXECUTED";
            }
            catch (ArgumentNullException e)
            {
                Logger.Log($"Missing values in command! Command: {String.Join(',', commandParts)}");
                return "INVALID COMMAND";
            }
            catch (Exception e)
            {
                Logger.Log($"Unable to parse command! Command: {String.Join(',', commandParts)}");
                return "INVALID COMMAND";
            }
        }
    }
}
