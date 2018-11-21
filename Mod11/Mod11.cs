using Smod2;
using Smod2.Attributes;
using GamemodeManager;
using Smod2.Config;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Smod2.Lang;
using Smod2.API;

namespace VirtualBrightPlayz.SCPSL.Mod11
{
    [PluginDetails(author = "VirtualBrightPlayz",
        description = "Battle in the Laboratory is basically Battle Royale for SCP:SL",
        id = "virtualbrightplayz.scpsl.mod11",
        name = "Battle in the Laboratory",
        version = "1.0",
        SmodMajor = 3,
        SmodMinor = 0,
        SmodRevision = 0)]
    public class Mod11 : Plugin
    {

        public override void OnDisable()
        {
        }

        public override void OnEnable()
        {
            this.Info("Battle in the Laboratory plugin enabled.");
        }

        public override void Register()
        {
            this.AddEventHandlers(new Mod11EventHandler(this), Smod2.Events.Priority.Normal);
            this.AddConfig(new ConfigSetting("battleroyale_weapons", new int[] { (int)ItemType.COM15, (int)ItemType.FRAG_GRENADE, (int)ItemType.MP4, (int)ItemType.P90 }, SettingType.NUMERIC_LIST, true, "A list of all the weapons that can spawn"));
            GamemodeManager.GamemodeManager.RegisterMode(this, "44444444444444444444");

            Dictionary<string, string> translations = new Dictionary<string, string>
            {
                { "MODEBR_YOU_ARE", "You are" },
                { "MODEBR_DESCRIPTION", "Description" },
                { "MODEBR_GOAL", "Goal" },

                { "MODEBR_CLASSD", "Class-D Personnel" },
                { "MODEBR_CLASSD_DESC", "You wake up in a random room." },
                { "MODEBR_CLASSD_GOAL", "Escape, but try not to get killed by others." },

                { "MODEBR_CHAOS_INSUGENCY", "Chaos Insurgency" },
                { "MODEBR_CHAOS_INSUGENCY_DESC", "You have escaped the facility." },
                { "MODEBR_CHAOS_INSUGENCY_GOAL", "Kill all Class-D Personnel, but not your crew.\nOr you could skrew around." },

                { "CLASSD", "Dboi" }
            };

            foreach (KeyValuePair<string, string> translation in translations)
            {
                this.AddTranslation(new LangSetting(translation.Key, translation.Value, "gamemode_battleroyale"));
            }
        }
    }
}
