using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace VirtualBrightPlayz.SCPSL.Mod11
{
    internal class Mod11EventHandler : IEventHandlerCheckEscape, IEventHandlerSpawn, IEventHandlerRoundStart, IEventHandlerLCZDecontaminate, IEventHandlerRoundEnd, IEventHandlerCheckRoundEnd, IEventHandlerPlayerPickupItem, IEventHandlerSetRole, IEventHandlerUpdate, IEventHandlerShoot, IEventHandlerPlayerJoin, IEventHandlerPlayerHurt
    {
        private Mod11 mod11;
        public List<Transform> rooms;
        public Dictionary<string, int> ammo9;
        public Dictionary<string, int> ammo7;
        public Dictionary<string, int> ammo5;
        public List<string> players;
        public int[] items;
        public bool hasended;
        public bool roundstarted;
        public const float MinDist = 300;

        public Mod11EventHandler(Mod11 mod11)
        {
            this.mod11 = mod11;
            items = new int[] { };
            rooms = new List<Transform>();
            items = ConfigManager.Manager.Config.GetIntListValue("battleroyale_weapons", new int[] { (int)ItemType.COM15, (int)ItemType.FRAG_GRENADE, (int)ItemType.MP4, (int)ItemType.P90 });
        }

        void IEventHandlerCheckEscape.OnCheckEscape(PlayerCheckEscapeEvent ev)
        {
            ev.AllowEscape = false;
            //ev.ChangeRole = Smod2.API.Role.CHAOS_INSUGENCY;
        }

        void GetRooms()
        {
            var lczrooms = GameObject.Find("LightRooms").transform;
            var hczrooms = GameObject.Find("Heavy rooms").transform;
            var ezrooms = GameObject.Find("EntranceRooms").transform;
            rooms = new List<Transform>();
            for (int i = 0; i < lczrooms.childCount; i++)
            {
                Transform room = lczrooms.GetChild(i);
                Vector3 pos = new Vector3(lczrooms.position.x, room.position.y, lczrooms.position.y);
                if (room.name.StartsWith("Root_") && Vector3.Distance(pos, room.position) < MinDist && !room.name.Contains("173"))
                    rooms.Add(room);
            }
            for (int i = 0; i < hczrooms.childCount; i++)
            {
                Transform room = hczrooms.GetChild(i);
                Vector3 pos = new Vector3(hczrooms.position.x, room.position.y, hczrooms.position.y);
                if (room.name.StartsWith("Root_") && Vector3.Distance(pos, room.position) < MinDist && !room.name.Contains("Tesla"))
                    rooms.Add(room);
            }
            for (int i = 0; i < ezrooms.childCount; i++)
            {
                Transform room = ezrooms.GetChild(i);
                Vector3 pos = new Vector3(ezrooms.position.x, room.position.y, ezrooms.position.y);
                if (room.name.StartsWith("Root_") && Vector3.Distance(pos, room.position) < MinDist && !room.name.Contains("Shelter") && !room.name.Contains("CollapsedTunnel"))
                    rooms.Add(room);
            }
        }

        void IEventHandlerRoundStart.OnRoundStart(RoundStartEvent ev)
        {
            roundstarted = true;
            hasended = false;
            //ammo5 = new Dictionary<string, int>();
            //ammo7 = new Dictionary<string, int>();
            //ammo9 = new Dictionary<string, int>();
            //foreach (Player player in mod11.Server.GetPlayers())
            //{
            //    ammo5.Add(player.SteamId, player.GetAmmo(AmmoType.DROPPED_5));
            //    ammo7.Add(player.SteamId, player.GetAmmo(AmmoType.DROPPED_7));
            //    ammo9.Add(player.SteamId, player.GetAmmo(AmmoType.DROPPED_9));
            //}
            if (rooms == null)
            {
                GetRooms();
            }
            var lczrooms = GameObject.Find("LightRooms").transform;
            var hczrooms = GameObject.Find("Heavy rooms").transform;
            var ezrooms = GameObject.Find("EntranceRooms").transform;
            rooms = new List<Transform>();
            for (int i = 0; i < lczrooms.childCount; i++)
            {
                Transform room = lczrooms.GetChild(i);
                Vector3 pos = new Vector3(lczrooms.position.x, room.position.y, lczrooms.position.y);
                if (room.name.StartsWith("Root_") && Vector3.Distance(pos, room.position) < MinDist && !room.name.Contains("173"))
                    rooms.Add(room);
            }
            for (int i = 0; i < hczrooms.childCount; i++)
            {
                Transform room = hczrooms.GetChild(i);
                Vector3 pos = new Vector3(hczrooms.position.x, room.position.y, hczrooms.position.y);
                if (room.name.StartsWith("Root_") && Vector3.Distance(pos, room.position) < MinDist && !room.name.Contains("Tesla"))
                    rooms.Add(room);
            }
            for (int i = 0; i < ezrooms.childCount; i++)
            {
                Transform room = ezrooms.GetChild(i);
                Vector3 pos = new Vector3(ezrooms.position.x, room.position.y, ezrooms.position.y);
                if (room.name.StartsWith("Root_") && Vector3.Distance(pos, room.position) < MinDist && !room.name.Contains("Shelter") && !room.name.Contains("CollapsedTunnel") && !room.name.Contains("GateB") && !room.name.Contains("GateA"))
                    rooms.Add(room);
            }
            players = new List<string>();
            foreach (var player in mod11.Server.GetPlayers())
            {
                //mod11.Info(rooms.Count.ToString());
                int roomnumber = Random.Range(0, rooms.Count);
                Vector3 roompos = rooms[roomnumber].position;
                //plugin.Info(rooms[roomnumber].name);
                player.Teleport(new Smod2.API.Vector(roompos.x, roompos.y + 2f, roompos.z));
                player.GiveItem(Smod2.API.ItemType.ZONE_MANAGER_KEYCARD);
                players.Add(player.SteamId);
            }
            for (int i = 0; i < rooms.Count / 3; i++)
            {
                //Medkit
                int roomnumber2 = Random.Range(0, rooms.Count);
                Vector3 roompos2 = rooms[roomnumber2].position;
                Vector roompos21 = new Smod2.API.Vector(roompos2.x, roompos2.y + 2f, roompos2.z);
                mod11.Server.Map.SpawnItem(Smod2.API.ItemType.MEDKIT, roompos21, Vector.Zero);
                //Guns
                int roomnumber3 = Random.Range(0, rooms.Count);
                Vector3 roompos3 = rooms[roomnumber3].position;
                Vector roompos31 = new Smod2.API.Vector(roompos3.x, roompos3.y + 2f, roompos3.z);
                mod11.Server.Map.SpawnItem((ItemType)items[Random.Range(0, items.Length)], roompos31, Vector.Zero);
                //Ammo
                int roomnumber4 = Random.Range(0, rooms.Count);
                Vector3 roompos4 = rooms[roomnumber4].position;
                Vector roompos41 = new Smod2.API.Vector(roompos4.x, roompos4.y + 2f, roompos4.z);
                //mod11.Server.Map.SpawnItem(Smod2.API.ItemType.DROPPED_5, roompos41, Vector.Zero);
                //mod11.Server.Map.SpawnItem(Smod2.API.ItemType.DROPPED_7, roompos41, Vector.Zero);
                //mod11.Server.Map.SpawnItem(Smod2.API.ItemType.DROPPED_9, roompos41, Vector.Zero);
            }
        }

        void IEventHandlerLCZDecontaminate.OnDecontaminate()
        {
            var hczrooms = GameObject.Find("Heavy rooms").transform;
            var ezrooms = GameObject.Find("EntranceRooms").transform;
            rooms = new List<Transform>();
            for (int i = 0; i < hczrooms.childCount; i++)
            {
                Transform room = hczrooms.GetChild(i);
                Vector3 pos = new Vector3(hczrooms.position.x, room.position.y, hczrooms.position.y);
                if (room.name.StartsWith("Root_") && Vector3.Distance(pos, room.position) < MinDist && !room.name.Contains("Tesla"))
                    rooms.Add(room);
            }
            for (int i = 0; i < ezrooms.childCount; i++)
            {
                Transform room = ezrooms.GetChild(i);
                Vector3 pos = new Vector3(ezrooms.position.x, room.position.y, ezrooms.position.y);
                if (room.name.StartsWith("Root_") && Vector3.Distance(pos, room.position) < MinDist && !room.name.Contains("Shelter") && !room.name.Contains("CollapsedTunnel"))
                    rooms.Add(room);
            }
        }

        void IEventHandlerSpawn.OnSpawn(PlayerSpawnEvent ev)
        {
            if (rooms == null)
            {
                GetRooms();
            }
        }

        void IEventHandlerRoundEnd.OnRoundEnd(RoundEndEvent ev)
        {
            //rooms = null;
            hasended = true;
        }

        void IEventHandlerCheckRoundEnd.OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            List<Role> roles = new List<Role>();
            foreach (Player player in mod11.Server.GetPlayers())
            {
                if (player.TeamRole.Role == Role.CLASSD)
                    roles.Add(player.TeamRole.Role);
            }
            if (roles.Count > 1)
            {
                ev.Status = ROUND_END_STATUS.ON_GOING;
            }
            else
            {
                ev.Status = ROUND_END_STATUS.CI_VICTORY;
            }
        }

        void IEventHandlerPlayerPickupItem.OnPlayerPickupItem(PlayerPickupItemEvent ev)
        {
            //int ammo5 = ev.Player.GetAmmo(AmmoType.DROPPED_5);
            //int ammo7 = ev.Player.GetAmmo(AmmoType.DROPPED_7);
            //int ammo9 = ev.Player.GetAmmo(AmmoType.DROPPED_9);
            //mod11.Info(ev.Item.ItemType.ToString());
            //if (ev.Item.ItemType == ItemType.DROPPED_5)
            //{
            //    ev.Item.Drop();
            //    ev.Allow = false;
            //    ev.ChangeTo = ItemType.CUP;
            //    ev.Player.SetAmmo(AmmoType.DROPPED_5, ammo5 + 20);
            //}
            //if (ev.Item.ItemType == ItemType.DROPPED_7)
            //{
            //    ev.Item.Drop();
            //    ev.Allow = false;
            //    ev.ChangeTo = ItemType.CUP;
            //    ev.Player.SetAmmo(AmmoType.DROPPED_7, ammo7 + 20);
            //}
            //if (ev.Item.ItemType == ItemType.DROPPED_9)
            //{
            //    ev.Item.Drop();
            //    ev.Allow = false;
            //    ev.ChangeTo = ItemType.CUP;
            //    ev.Player.SetAmmo(AmmoType.DROPPED_9, ammo9 + 20);
            //}
        }

        void IEventHandlerSetRole.OnSetRole(PlayerSetRoleEvent ev)
        {
            if (ev.Role == Role.CLASSD || ev.TeamRole.Team == Smod2.API.Team.SCP || ev.TeamRole.Team == Smod2.API.Team.NONE || ev.TeamRole.Team == Smod2.API.Team.TUTORIAL || ev.TeamRole.Team == Smod2.API.Team.SPECTATOR)
            { }
            else
                ev.Role = Role.CHAOS_INSUGENCY;
        }

        void IEventHandlerUpdate.OnUpdate(UpdateEvent ev)
        {
            List<string> roles = new List<string>();
            foreach (Player player in mod11.Server.GetPlayers())
            {
                if (player.TeamRole.Role == Role.CLASSD)
                    roles.Add(player.Name);
            }
            if (roles.Count == 1 && !hasended && roundstarted)
            {
                foreach (Player player in mod11.Server.GetPlayers())
                {
                    player.SendConsoleMessage(roles[0] + " Wins!", "red");
                }
                mod11.Server.Round.EndRound();
                hasended = true;
            }
            else if (roles.Count < 1 && !hasended && roundstarted)
            {
                foreach (Player player in mod11.Server.GetPlayers())
                {
                    player.SendConsoleMessage("No one wins.", "red");
                }
                mod11.Server.Round.EndRound();
                hasended = true;
            }
            /*foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Pickup"))
            {
                try
                {
                    obj.gett
                }
                catch (System.Exception e)
                {
                }
            }*/
            /*foreach (Smod2.API.Player player in mod11.Server.GetPlayers())
            {
                if (player.GetAmmo(AmmoType.DROPPED_5) == 0)
                {
                    player.SetAmmo(AmmoType.DROPPED_5, 100);
                }
                if (player.GetAmmo(AmmoType.DROPPED_7) == 0)
                {
                    player.SetAmmo(AmmoType.DROPPED_7, 100);
                }
                if (player.GetAmmo(AmmoType.DROPPED_9) == 0)
                {
                    player.SetAmmo(AmmoType.DROPPED_9, 100);
                }
            }*/
        }

        void IEventHandlerShoot.OnShoot(PlayerShootEvent ev)
        {
            ev.Player.SetAmmo(AmmoType.DROPPED_5, 500);
            ev.Player.SetAmmo(AmmoType.DROPPED_7, 500);
            ev.Player.SetAmmo(AmmoType.DROPPED_9, 500);
        }

        void IEventHandlerPlayerJoin.OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (players != null)
            {
                if (!players.Contains(ev.Player.SteamId))
                {
                    foreach (var player in mod11.Server.GetPlayers())
                    {
                        //mod11.Info(rooms.Count.ToString());
                        int roomnumber = Random.Range(0, rooms.Count);
                        Vector3 roompos = rooms[roomnumber].position;
                        //plugin.Info(rooms[roomnumber].name);
                        player.Teleport(new Smod2.API.Vector(roompos.x, roompos.y + 2f, roompos.z));
                        player.GiveItem(Smod2.API.ItemType.ZONE_MANAGER_KEYCARD);
                        players.Add(player.SteamId);
                    }
                }
            }
        }

        void IEventHandlerPlayerHurt.OnPlayerHurt(PlayerHurtEvent ev)
        {
            if (ev.Attacker.TeamRole.Role == Role.CHAOS_INSUGENCY && ev.Player.TeamRole.Role == Role.CHAOS_INSUGENCY)
            {
                ev.Damage = 0;
            }
        }
    }
}