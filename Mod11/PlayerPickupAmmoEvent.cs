using Smod2.API;
using Smod2.Events;
using System.Threading;

namespace VirtualBrightPlayz.SCPSL.Mod11
{
    internal class PlayerPickupAmmoEvent
    {
        private Mod11 mod11;
        private PlayerPickupItemEvent ev;

        public PlayerPickupAmmoEvent(Mod11 mod11, PlayerPickupItemEvent ev)
        {
            this.mod11 = mod11;
            this.ev = ev;
            Thread.Sleep(100);
            if (ev.Item.ItemType == ItemType.DROPPED_5)
            {
                int ammo = ev.Player.GetAmmo(AmmoType.DROPPED_5);
                ev.Player.SetAmmo(AmmoType.DROPPED_5, ammo + 20);
            }
            if (ev.Item.ItemType == ItemType.DROPPED_7)
            {
                int ammo = ev.Player.GetAmmo(AmmoType.DROPPED_7);
                ev.Player.SetAmmo(AmmoType.DROPPED_7, ammo + 20);
            }
            if (ev.Item.ItemType == ItemType.DROPPED_9)
            {
                int ammo = ev.Player.GetAmmo(AmmoType.DROPPED_9);
                ev.Player.SetAmmo(AmmoType.DROPPED_9, ammo + 20);
            }
        }
    }
}