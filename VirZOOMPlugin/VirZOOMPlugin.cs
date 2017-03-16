using FreePIE.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VirZOOMPlugin
{
    [GlobalType(Type = typeof(VirZOOMPluginGlobal))]
    public class VirZOOMPlugin : IPlugin
    {
        internal VZBikeState state;
        private static bool libraryInitialized = false;
        private DateTime startTime;
        public object CreateGlobal()
        {
            // The library needs to be initialized only once, but do it lazily (only when it's actually used in a script, which is when CreateGlobal is called)
            if (!libraryInitialized)
            {
                VZInit();
                libraryInitialized = true;
            }
            return new VirZOOMPluginGlobal(this);
        }

        public Action Start()
        {
            return () =>
            {
                startTime = DateTime.Now;
                VZConnectBike(ref state);
            };
        }

        public void Stop()
        {
            VZCloseBike();
        }

        public event EventHandler Started;

        public string FriendlyName
        {
            get { return "A name that will be used from GUI"; }
        }

        public bool GetProperty(int index, IPluginProperty property)
        {
            return false;
        }

        public bool SetProperties(Dictionary<string, object> properties)
        {
            return false;
        }

        public void DoBeforeNextExecute()
        {
            //This method will be executed each iteration of the script
            if (state.Type > 0)
                VZUpdateBike(ref state, (float)(DateTime.Now - startTime).TotalSeconds);
            else
                VZConnectBike(ref state);
        }

        public void SetGameName(string name)
        {
            VZSetGameName(name);
        }

        [DllImport("VZPlugin.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VZInit();
        [DllImport("VZPlugin.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VZConnectBike(ref VZBikeState State);
        [DllImport("VZPlugin.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VZUpdateBike(ref VZBikeState State, float DeltaTime);
        [DllImport("VZPlugin.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VZCloseBike();
        [DllImport("VZPlugin.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void VZSetGameName(string name);
    }

    [Global(Name = "VirZOOM")]
    public class VirZOOMPluginGlobal
    {
        private readonly VirZOOMPlugin plugin;

        public VirZOOMPluginGlobal(VirZOOMPlugin plugin)
        {
            this.plugin = plugin;
        }

        public void setGameName(string name)
        {
            plugin.SetGameName(name);
        }

        public float timeAtLastPulseMs
        {
            get { return plugin.state.TimeAtLastPulseMs; }
        }
        public float heartRate
        {
            get { return plugin.state.HeartRate; }
        }
        public float batteryVolts
        {
            get { return plugin.state.BatteryVolts; }
        }
        public float speed
        {
            get { return plugin.state.Speed; }
        }
        public int pulses
        {
            get { return plugin.state.Pulses; }
        }
        public int filteredResistance
        {
            get { return plugin.state.FilteredResistance; }
        }
        public BikeDongleType type
        {
            get { return plugin.state.Type; }
        }
        public int betaVersion
        {
            get { return plugin.state.BetaVersion; }
        }
        public bool leftTrigger
        {
            get { return plugin.state.LeftTrigger; }
        }
        public bool dpadUp
        {
            get { return plugin.state.DpadUp; }
        }
        public bool dpadDown
        {
            get { return plugin.state.DpadDown; }
        }
        public bool dpadLeft
        {
            get { return plugin.state.DpadLeft; }
        }
        public bool dpadRight
        {
            get { return plugin.state.DpadRight; }
        }
        public bool rightTrigger
        {
            get { return plugin.state.RightTrigger; }
        }
        public bool rightUp
        {
            get { return plugin.state.RightUp; }
        }
        public bool rightDown
        {
            get { return plugin.state.RightDown; }
        }
        public bool rightLeft
        {
            get { return plugin.state.RightLeft; }
        }
        public bool rightRight
        {
            get { return plugin.state.RightRight; }
        }
        public bool connected
        {
            get {
                // The "Connected" data in state is only valid once plugin.state.Type > 0, and all other state data is valid only after both are true.
                // By checking Type here, script users don't have to think about this implementation detail anymore. 
                return plugin.state.Type > 0 && plugin.state.Connected;
            }
        }

        public byte[] sender
        {
            get { return plugin.state.Sender; }
        }
    }

    [GlobalEnum]
    public enum BikeDongleType : int
    {
        lowEnergy = -2,
        noBike = -1,
        wired = 0,
        alphaWireless = 1,
        betaWireless = 2
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VZBikeState
    {
        public float Timestamp;
        public float TimeAtLastPulseMs;
        public float HeartRate;
        public float BatteryVolts;
        public float Speed;
        public int Pulses;
        public int FilteredResistance;
        public BikeDongleType Type;
        //-1 none
        // 0 original pin outs and reversed pedal direction
        // 1 new pin outs for better sleep and fixed pedal direction
        // 2 blinking leds and ota
        public int BetaVersion;
        public bool LeftTrigger,
            DpadUp,
            DpadDown,
            DpadLeft,
            DpadRight,
            RightTrigger,
            RightUp,
            RightDown,
            RightLeft,
            RightRight,
            Connected;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Sender;
    };
}
