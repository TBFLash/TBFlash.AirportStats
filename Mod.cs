using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimAirport.Modding.Base;
using SimAirport.Modding.Settings;
using SimAirport.Modding.Data;
using Modding;
using SimAirport.Logging;

namespace TBFlash.AirportStats
{
    public class Mod : BaseMod
    {
        public override string Name => "Airport Stats";

        public override string InternalName => "TBFlash.AirportStats";

        public override string Description => "";

        public override string Author => "TBFlash";

        public override SettingManager SettingManager { get; set; }

        private TBFlash_Server server;

        private readonly bool isTBFlashDebug = true;

        public override void OnTick()
        {
        }

        public override void OnLoad(SimAirport.Modding.Data.GameState state)
        {
            if (Game.isLoaded)
            {
                server = new TBFlash_Server();
                server.Start();
                TBFlashLogger(Log.FromPool("").WithCodepoint());
            }
        }

        public override void OnDisabled()
        {
            server?.Stop();
            server = null;
            TBFlashLogger(Log.FromPool("").WithCodepoint());
        }

        public override void OnAirportLoaded(Dictionary<string, object> saveData)
        {
            ScriptHandler sh = (ScriptHandler)ModLoader.ModHandlers.Find(x => x.GetType() == typeof(ScriptHandler));
            TBFlashLogger(Log.FromPool($"{sh?.IsModEnabled(this)}").WithCodepoint());
            sh?.SetModStatus(this, false);
            TBFlashLogger(Log.FromPool($"{sh?.IsModEnabled(this)}").WithCodepoint());
        }

        public override void OnSettingsLoaded()
        {
            TBFlashLogger(Log.FromPool("").WithCodepoint());
            LabelSetting description = new LabelSetting
            {
                Name = "A webbrowser will open to localhost:2198 when you start this mod.",
            };
            SettingManager.AddDefault("Description", description);
        }

        private void TBFlashLogger(Log log)
        {
            if (isTBFlashDebug)
            {
                Game.Logger.Write(log);
            }
        }
    }
}
