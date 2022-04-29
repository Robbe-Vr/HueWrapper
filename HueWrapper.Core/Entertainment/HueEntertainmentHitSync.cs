using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SYWCentralLogging;
using SYWPipeNetworkManager;

namespace HueWrapper.Core.Entertainment
{
    internal partial class HueEntertainmentHandler
    {
        private string _pipeTargetName = "HitSync";

        public void StartHitSync(string room)
        {
            PipeMessageControl.SendToApp(_pipeTargetName, "Start=" + room);
        }

        public void StopHitSync()
        {
            PipeMessageControl.SendToApp(_pipeTargetName, "Stop");
        }

        public string SetHitSyncMode(IEnumerable<string> commandParts)
        {
            try
            {
                string mode = commandParts.FirstOrDefault()?.ToUpper();
                if (!String.IsNullOrWhiteSpace(mode))
                {
                    PipeMessageControl.SendToApp(_pipeTargetName, "Mode=" + mode);

                    return "EXECUTED";
                }
                else return "INVALID DATA";
            }
            catch (Exception e)
            {
                Logger.Log($"Unable to parse HitSync MODE command! Command: {String.Join(',', commandParts)}");
                return "INVALID COMMAND";
            }
        }

        public string SetHitSyncColors(IEnumerable<string> commandParts)
        {
            try
            {
                string colors = commandParts.FirstOrDefault()?.ToUpper();
                if (!String.IsNullOrWhiteSpace(colors) && Regex.IsMatch(colors, @"^\[([0-9]{1,3},[0-9]{1,3},[0-9]{1,3}\],\[)+[0-9]{1,3},[0-9]{1,3},[0-9]{1,3}\]$"))
                {
                    PipeMessageControl.SendToApp(_pipeTargetName, "Colors=" + colors);

                    return "EXECUTED";
                }
                else return "INVALID DATA";
            }
            catch (Exception e)
            {
                Logger.Log($"Unable to parse HitSync COLORS command! Command: {String.Join(',', commandParts)}");
                return "INVALID COMMAND";
            }
        }

        public string SetHitSyncValues(IEnumerable<string> commandParts)
        {
            try
            {
                string property = commandParts.FirstOrDefault()?.ToUpper();
                if (!String.IsNullOrWhiteSpace(property))
                {
                    if (property == "MODE")
                        return SetHitSyncMode(commandParts.Skip(1));
                    else if (property == "COLORS")
                        return SetHitSyncColors(commandParts.Skip(1));
                    else PipeMessageControl.SendToApp(_pipeTargetName, "Set=" + property + "=" + commandParts.ElementAtOrDefault(1));

                    return "EXECUTED";
                }
                else return "INVALID DATA";
            }
            catch (Exception e)
            {
                Logger.Log($"Unable to parse HitSync SET command! Command: {String.Join(',', commandParts)}");
                return "INVALID COMMAND";
            }
        }
    }
}
