using Microsoft.Win32;

namespace FreePIE.GUI.Events
{
    public class PowerModeEvent
    {
        public PowerModes Mode { get; private set; }
        public PowerModeEvent(PowerModeChangedEventArgs e)
        {
            Mode = e.Mode;
        }
    }
}
