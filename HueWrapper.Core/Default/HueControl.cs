using Q42.HueApi;
using Q42.HueApi.Models.Groups;
using Q42.HueApi.ColorConverters.HSB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SYWCentralLogging;
using Q42.HueApi.ColorConverters;

namespace HueWrapper.Core.Default
{
    internal partial class HueControl
    {
        private string groupId;
        private string[] lights;

        internal HueControl(string room) : base()
        {
            Task<IReadOnlyCollection<Group>> task = HueBridge.Client.GetGroupsAsync();
            task.Wait();
            
            Group group = task.Result.FirstOrDefault(x => x.Name == room);
            this.groupId = group?.Id;
            this.lights = group?.Lights.ToArray() ?? Array.Empty<string>();
        }

        internal HueControl(params string[] lights)
        {
            this.lights = lights;
        }

        private int GetHue(string colorPart, State current)
        {
            if (System.Text.RegularExpressions.Regex.Matches(colorPart, "{keep}").Count >= 3)
            {
                return -1;
            }

            RGBColor rgbColor;
            if (colorPart.StartsWith('#'))
            {
                rgbColor = new RGBColor(colorPart);
            }
            else if (colorPart.Contains(','))
            {
                string[] colors = colorPart.Split(',');

                int[] colorValues = new int[3];

                for (int i = 0; i < 3; i++)
                {
                    if (colors[i] == "{keep}")
                    {
                        colorValues[i] = (int)((i == 0 ? current.ToRgb().R : i == 1 ? current.ToRgb().G : current.ToRgb().B) * 255);
                    }
                    else
                    {
                        colorValues[i] = int.Parse(colors[i]);
                    }
                }

                rgbColor = new RGBColor(colorValues[0], colorValues[1], colorValues[2]);
            }
            else
            {
                return -1;
            }

            return (int)(rgbColor.GetHue());
        }

        private int GetSat(string colorPart, State current)
        {
            if (System.Text.RegularExpressions.Regex.Matches(colorPart, "{keep}").Count >= 3)
            {
                return -1;
            }

            RGBColor rgbColor;
            if (colorPart.StartsWith('#'))
            {
                rgbColor = new RGBColor(colorPart);
            }
            else if (colorPart.Contains(','))
            {
                string[] colors = colorPart.Split(',');
                
                int[] colorValues = new int[3];

                for (int i = 0; i < 3; i++)
                {
                    if (colors[i] == "{keep}")
                    {
                        colorValues[i] = (int)((i == 0 ? current.ToRgb().R : i == 1 ? current.ToRgb().G : current.ToRgb().B) * 255);
                    }
                    else
                    {
                        colorValues[i] = int.Parse(colors[i]);
                    }
                }

                rgbColor = new RGBColor(colorValues[0], colorValues[1], colorValues[2]);
            }
            else
            {
                return -1;
            }

            return (int)(rgbColor.GetSaturation());
        }

        internal string Process(IEnumerable<string> commandParts)
        {
            if (lights.Length < 1) return "NO TARGETS";

            try
            {
                int commandPartsCount = commandParts.Count();

                string first = commandParts.FirstOrDefault()?.ToUpper();
                bool alert = first.StartsWith("ALERT");

                if ((first != "ON" && first != "OFF" && !alert))
                {
                    return "INVALID COMMAND";
                }

                bool? ON = first == "OFF" ? false : first == "ON" ? true : null;
                Alert ALERT =
                    alert ?
                        first?.Split('-').ElementAtOrDefault(1) == "ONCE" ?
                            Alert.Once
                            : Alert.Multiple
                        : Alert.None;

                commandParts = commandParts.Skip(1);


                State currentState = null;
                if (commandParts.Any(x => x.Contains("{keep}")))
                {
                    Task<Light> lightTask = HueBridge.Client.GetLightAsync(lights.First());
                    lightTask.Wait();

                    currentState = lightTask.Result.State;
                }

                byte? BRIGHTNESS = null;
                if (commandParts.FirstOrDefault() != "{keep}")
                {
                    byte BRIGHTNESS_;
                    if (byte.TryParse(commandParts.FirstOrDefault(), out BRIGHTNESS_))
                    {
                        BRIGHTNESS = BRIGHTNESS_;
                    }
                }

                commandParts = commandParts.Skip(1);
                string colorPart = commandParts.FirstOrDefault();

                int? HUE = GetHue(colorPart, currentState);
                if (HUE < 0) HUE = null;
                int? SATURATION = GetSat(colorPart, currentState);
                if (SATURATION < 0) SATURATION = null;

                LightCommand command = new LightCommand()
                {
                    Alert = ALERT,
                    Brightness = BRIGHTNESS,
                    Hue = HUE,
                    Saturation = SATURATION,
                    On = ON,
                };

                HueBridge.QueueCommand(lights, command);

                return "EXECUTED";
            }
            catch (Exception e)
            {
                Logger.Log($"Failed to execute command!\n{e.Message}");
                return "FAIL";
            }
        }
    }
}
