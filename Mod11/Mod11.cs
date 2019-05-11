using Smod2;
using Smod2.Attributes;
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
            this.AddConfig(new ConfigSetting("battleroyale_weapons", new int[] { (int)ItemType.COM15, (int)ItemType.FRAG_GRENADE, (int)ItemType.MP4, (int)ItemType.P90 }, true, "A list of all the weapons that can spawn"));
        }
    }
}
