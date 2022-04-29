using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Bridge;
using Q42.HueApi.Models.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SYWCentralLogging;

namespace HueWrapper.Core.Default
{
    internal static class HueBridge
    {
        private static Preferences _preferences;

        internal static ILocalHueClient Client { get; private set; }

        private static Dictionary<string[], LightCommand> _queue = new Dictionary<string[], LightCommand>();

        private static Timer _queueTimer = new Timer(120);

        internal static async Task<bool> Setup(Preferences preferences)
        {
            _preferences = preferences;

            LocatedBridge bridge;

            IEnumerable<LocatedBridge> bridges = (await new HttpBridgeLocator().LocateBridgesAsync(TimeSpan.FromSeconds(5)));

            if (String.IsNullOrWhiteSpace(preferences.BridgeIp) && bridges.Any(x => x.BridgeId.ToUpper() == preferences.PreferedBridge))
            {
                int bridgeCount = bridges.Count();
                Logger.Log($"Found {bridgeCount} bridges!");

                bridge = bridges.FirstOrDefault();

                if (bridge == null) return false;
            }
            else
            {
                bridge = new();
                bridge.BridgeId = preferences.PreferedBridge;
                bridge.IpAddress = preferences.BridgeIp;
            }

            await ClientConnect(bridge);

            _queueTimer.Elapsed += PursueNextCommand;
            _queueTimer.AutoReset = false;
            
            return true;
        }

        private static void PursueNextCommand(object sender, ElapsedEventArgs e)
        {
            KeyValuePair<string[], LightCommand> command = _queue.FirstOrDefault();

            if (command.Key != null && command.Key.Length > 0)
            {
                SendCommand(command.Key, command.Value);

                _queue.Remove(command.Key);
            }
        }

        internal static void QueueCommand(string[] targets, LightCommand command)
        {
            if (_queueTimer.Enabled || _queue.Count > 0)
            {
                if (_queue.ContainsKey(targets))
                {
                    _queue[targets] = command;
                }
                else
                {
                    _queue.Add(targets, command);
                }
            }
            else
            {
                SendCommand(targets, command);
            }
        }

        private static void SendCommand(string[] targets, LightCommand command)
        {
            Task<HueResults> task = HueBridge.Client.SendCommandAsync(command, targets);
            task.Wait();
            HueResults results = task.Result;

            foreach (DefaultHueResult result in results)
            {
                if (!String.IsNullOrWhiteSpace(result.Error?.Description))
                {
                    Logger.Log($"Failed to execute light command!\nAddress: {result.Error.Address}\n{result.Error.Description}");
                }
            }
        }

        private static async Task<bool> ClientConnect(LocatedBridge bridge)
        {
            try
            {
                Client = new LocalHueClient(bridge.IpAddress);

                if (bridge.BridgeId.ToUpper() == _preferences.PreferedBridge)
                {
                    if (String.IsNullOrWhiteSpace(_preferences.AppKey))
                    {
                        _preferences.AppKey = await Client.RegisterAsync("MidiDomotica", Environment.MachineName);
                        _preferences.Store();
                    }
                    else
                    {
                        Client.Initialize(_preferences.AppKey);
                    }
                }
                else
                {
                    await Client.RegisterAsync("MidiDomotica", Environment.MachineName);
                }

                if (Client != null)
                {
                    Task<bool> connected = Client.CheckConnection();
                    connected.Wait();

                    return connected.Result;
                }
                else return false;
            }
            catch (Exception e)
            {
                Logger.Log($"Unable to connect to bridge {bridge.BridgeId} at {bridge.IpAddress}! Error: " + e.Message);
                return false;
            }
        }

        internal static string ConnectToBridge(string bridgeInfo)
        {
            Task<IEnumerable<LocatedBridge>> task = new HttpBridgeLocator().LocateBridgesAsync(TimeSpan.FromSeconds(5));
            task.Wait();
            IEnumerable<LocatedBridge> bridges = task.Result;

            LocatedBridge bridge = bridges.FirstOrDefault(x => x.IpAddress == bridgeInfo || x.BridgeId == bridgeInfo);

            if (bridge != null)
            {
                Task<bool> connectTask = ClientConnect(bridge);
                connectTask.Wait();

                if (connectTask.Result)
                    return "CONNECTED";
            }

            return "FAIL";
        }
    }
}
