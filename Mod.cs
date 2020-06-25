using System.Collections.Generic;
using SimAirport.Modding.Base;
using SimAirport.Modding.Settings;
using Modding;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    public class Mod : BaseMod
    {
        public override string Name => "Airport Stats";

        public override string InternalName => "TBFlash.AirportStats";

        public override string Description => "Web-based statistics about your SimAirport!";

        public override string Author => "TBFlash";

        public override SettingManager SettingManager { get; set; }

        private TBFlash_Server server;

        public override void OnTick()
        {
        }

        public override void OnLoad(SimAirport.Modding.Data.GameState state)
        {
            if (Game.isLoaded)
            {
                server = new TBFlash_Server();
                server.Start();
            }
        }

        public override void OnDisabled()
        {
            server?.Stop();
            server = null;
        }

       public override void OnAirportLoaded(Dictionary<string, object> saveData)
        {
            ScriptHandler sh = (ScriptHandler)ModLoader.ModHandlers.Find(x => x.GetType() == typeof(ScriptHandler));
            sh?.SetModStatus(this, false);
        }

        public override void OnSettingsLoaded()
        {
            LabelSetting description = new LabelSetting
            {
                Name = "A webbrowser will open to localhost:2198 when you start this mod.",
            };
            SettingManager.AddDefault("Description", description);
        }
    }
}
