using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;

namespace ModSettings
{
    public class GraffitiScriptSettings : Settings
    {
        [SerializableProperty("Settings", "A decal will always be loaded when you are this close to it (in meters), whether you are looking at it or not. Increasing this too much may lag your game.")]
        public float MinimumLoadingRange { get; set; }

        [SerializableProperty("Settings", "When you are looking towards a decal and within this range (meters), it will be loaded. Increasing this too much may lag your game.")]
        public float MaximumLoadingRange { get; set; }

        [SerializableProperty("Settings", "Only applies if you do not have the Script Communicator mod menu installed.")]
        public Keys MenuKey { get; set; }

        public GraffitiScriptSettings(string path) : base(path)
        {
        }

        public override void SetDefault()
        {
            MinimumLoadingRange = 20f;
            MaximumLoadingRange = 50f;
            MenuKey = Keys.F10;
        }
    }
}
