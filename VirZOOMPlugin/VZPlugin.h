//***********************************************************************
// Raw bike SDK
// Copyright 2017 VirZOOM
//***********************************************************************

#ifndef DLL_DECLARE
# define DLL_DECLARE extern "C" __declspec(dllimport)
#endif

// "Bike state" is raw data that comes out of the bike
struct VZBikeState
{
   float Timestamp;         // last time VZUpdateBike called
   float TimeAtLastPulseMs; // last time heartbeat received
   float HeartRate;         // in beats per minute
   float BatteryVolts;      // starts acting wonky < 2.2v
   float Speed;             // pedaling "meters per second", can be negative
   int Pulses;              // raw pedaling counter
   int FilteredResistance;  // pedaling resistance setting (needs calibration by application)
   int Type;                // type of bike dongle (-1:no bike, 0:wired, 1:alpha, 2:beta)
   int BetaVersion;         // if beta bike connected
   bool LeftTrigger;        // left grip trigger and buttons down
   bool DpadUp;             
   bool DpadDown;
   bool DpadLeft;
   bool DpadRight;
   bool RightTrigger;       // right grip trigger and buttons down
   bool RightUp;
   bool RightDown;
   bool RightLeft;
   bool RightRight;
   bool Connected;          // whether bike is connected to dongle
   unsigned char Sender0;   // numeric mac address of beta bike
   unsigned char Sender1;
   unsigned char Sender2;
   unsigned char Sender3;
   unsigned char Sender4;
   unsigned char Sender5;
};

// Call VZInit once to init 
DLL_DECLARE void VZInit();

// Call VZConnectBike until state.Type > 0 (when dongle is plugged in)
DLL_DECLARE void VZConnectBike(VZBikeState* state);

// Call VZUpdateBike once per game frame after state.Type > 0
DLL_DECLARE bool VZUpdateBike(VZBikeState* state, float time);

// Call VZCloseBike to disconnect from bike
DLL_DECLARE void VZCloseBike();

// Set the name of your application (for telemetry purposes)
DLL_DECLARE void VZSetGameName(const char* name);
