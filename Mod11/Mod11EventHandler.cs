using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace VirtualBrightPlayz.SCPSL.Mod11
{
    internal class Mod11EventHandler : IEventHandlerCheckEscape, IEventHandlerInitialAssignTeam, IEventHandlerTeamRespawn, IEventHandlerRoundRestart, IEventHandlerSpawn, IEventHandlerRoundStart, IEventHandlerLCZDecontaminate, IEventHandlerRoundEnd, IEventHandlerCheckRoundEnd, IEventHandlerPlayerPickupItem, IEventHandlerSetRole, IEventHandlerUpdate, IEventHandlerShoot, IEventHandlerPlayerJoin, IEventHandlerPlayerHurt
    {
        private Mod11 mod11;
        public List<Transform> rooms;
        public Dictionary<string, int> ammo9;
        public Dictionary<string, int> ammo7;
        public Dictionary<string, int> ammo5;
        public List<string> spawningPlayers;
        public List<string> tpPlayers;
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
            spawningPlayers = new List<string>();
            tpPlayers = new List<string>();
            items = ConfigManager.Manager.Config.GetIntListValue("battleroyale_weapons", new int[] { (int)ItemType.COM15, (int)ItemType.FRAG_GRENADE, (int)ItemType.MP4, (int)ItemType.P90 });
        }

        void IEventHandlerCheckEscape.OnCheckEscape(PlayerCheckEscapeEvent ev)
        {
            ev.AllowEscape = false;
            //ev.ChangeRole = Smod2.API.Role.CHAOS_INSUGENCY;
        }

        void GetRooms(bool lczd = false)
        {
            var lczrooms = GameObject.Find("LightRooms").transform;
            var hczrooms = GameObject.Find("HeavyRooms").transform;
            var ezrooms = GameObject.Find("EntranceRooms").transform;
            rooms = new List<Transform>();
            if (!lczd)
            {
                for (int i = 0; i < lczrooms.childCount; i++)
                {
                    Transform room = lczrooms.GetChild(i);
                    Vector3 pos = new Vector3(lczrooms.position.x, room.position.y, lczrooms.position.y);
                    if (room.name.StartsWith("Root_") && Vector3.Distance(pos, room.position) < MinDist && !room.name.Contains("173") && !room.name.Contains("Armory"))
                        rooms.Add(room);
                }
            }
            for (int i = 0; i < hczrooms.childCount; i++)
            {
                Transform room = hczrooms.GetChild(i);
                Vector3 pos = new Vector3(hczrooms.position.x, room.position.y, hczrooms.position.y);
                if (room.name.StartsWith("Root_") && Vector3.Distance(pos, room.position) < MinDist && !room.name.Contains("Tesla") && !room.name.Contains("Room3ar"))
                    rooms.Add(room);
            }
            for (int i = 0; i < ezrooms.childCount; i++)
            {
                Transform room = ezrooms.GetChild(i);
                Vector3 pos = new Vector3(ezrooms.position.x, room.position.y, ezrooms.position.y);
                if (room.name.StartsWith("Root_") && Vector3.Distance(pos, room.position) < MinDist && !room.name.Contains("Shelter") && !room.name.Contains("CollapsedTunnel") && !room.name.Contains("GateB") && !room.name.Contains("GateA"))
                    rooms.Add(room);
            }
        }

        void IEventHandlerRoundStart.OnRoundStart(RoundStartEvent ev)
        {
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
            if (rooms == null || true)
            {
                GetRooms();
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
                if (player.TeamRole.Team == Smod2.API.Team.CLASSD)
                {
                    GameObject.FindObjectOfType<Broadcast>().CallTargetAddElement(((GameObject)player.GetGameObject()).GetComponent<NetworkIdentity>().connectionToClient, "<size=50><color=#fca714>Welcome Subject D-[Redacted]</color></size>", 3, true);
                    GameObject.FindObjectOfType<Broadcast>().CallTargetAddElement(((GameObject)player.GetGameObject()).GetComponent<NetworkIdentity>().connectionToClient, "<size=20><color=#fca714>You have been selected to fight against other Class-D Personnel and SCP subjects.\n" +
                        "This is a fight to the death, and you must be the last survivor in order to be released from the facility.\n" +
                        "Supplies have been placed around the facility, and are free to use.</color></size>", 10, true);
                    GameObject.FindObjectOfType<Broadcast>().CallTargetAddElement(((GameObject)player.GetGameObject()).GetComponent<NetworkIdentity>().connectionToClient, "<size=100><color=#fca714>Good Luck!</color></size>", 2, true);
                }
                if (player.TeamRole.Team == Smod2.API.Team.SCP)
                {
                    GameObject.FindObjectOfType<Broadcast>().CallTargetAddElement(((GameObject)player.GetGameObject()).GetComponent<NetworkIdentity>().connectionToClient, "<size=50><color=#fca714>Welcome Subject SCP-[Redacted]</color></size>", 3, true);
                    GameObject.FindObjectOfType<Broadcast>().CallTargetAddElement(((GameObject)player.GetGameObject()).GetComponent<NetworkIdentity>().connectionToClient, "<size=20><color=#fca714>You have breached containment. Just kill the Class-D Personnel.</color></size>", 10, true);
                    GameObject.FindObjectOfType<Broadcast>().CallTargetAddElement(((GameObject)player.GetGameObject()).GetComponent<NetworkIdentity>().connectionToClient, "<size=100><color=#fca714>Good Luck!</color></size>", 2, true);
                }
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
            roundstarted = true;
        }

        void IEventHandlerLCZDecontaminate.OnDecontaminate()
        {
            GetRooms(true);
        }

        void IEventHandlerSpawn.OnSpawn(PlayerSpawnEvent ev)
        {
            if (tpPlayers.Contains(ev.Player.SteamId))
            {
                tpPlayers.Remove(ev.Player.SteamId);
                ev.SpawnPos = mod11.Server.Map.GetRandomSpawnPoint(Role.CHAOS_INSURGENCY);
            }
            return;
            if (spawningPlayers.Contains(ev.Player.SteamId) || (ev.Player.TeamRole.Role == Role.CLASSD || ev.Player.TeamRole.Role == Role.CHAOS_INSURGENCY || ev.Player.TeamRole.Role == Role.SCP_049 || ev.Player.TeamRole.Role == Role.SCP_096 || ev.Player.TeamRole.Role == Role.SCP_106 || ev.Player.TeamRole.Role == Role.SCP_173 || ev.Player.TeamRole.Role == Role.SPECTATOR)) // && (ev.Role == Role.CLASSD || ev.TeamRole.Team == Smod2.API.Team.SCP || ev.TeamRole.Team == Smod2.API.Team.NONE || ev.TeamRole.Team == Smod2.API.Team.TUTORIAL || ev.TeamRole.Team == Smod2.API.Team.SPECTATOR)
            {
                if (spawningPlayers.Contains(ev.Player.SteamId))
                {
                    spawningPlayers.Remove(ev.Player.SteamId);
                }
            }
            else
            {
                spawningPlayers.Add(ev.Player.SteamId);
                List<Role> roles = new List<Role>() { Role.SCP_173, Role.SCP_096, Role.SCP_106, Role.SCP_049 };
                if (UnityEngine.Random.Range(0, 1) == 0)
                {
                    ev.Player.ChangeRole(Role.CHAOS_INSURGENCY);
                    ev.Player.GiveItem(ItemType.O5_LEVEL_KEYCARD);
                }
                else
                {
                    ev.Player.ChangeRole(roles[UnityEngine.Random.Range(0, roles.Count - 1)]);
                    ev.Player.Teleport(mod11.Server.Map.GetRandomSpawnPoint(Role.CHAOS_INSURGENCY));
                }
                GameObject.FindObjectOfType<Broadcast>().CallTargetAddElement(((GameObject)ev.Player.GetGameObject()).GetComponent<NetworkIdentity>().connectionToClient, "<color=#00ff00>Kill the Class-D Personnel, or not.</color>", 5, true);
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
            List<Role> rolesd = new List<Role>();
            foreach (Player player in mod11.Server.GetPlayers())
            {
                if (player.TeamRole.Role == Role.CLASSD)
                    rolesd.Add(player.TeamRole.Role);
                if (player.TeamRole.Role == Role.CLASSD || player.TeamRole.Team == Smod2.API.Team.SCP)
                    roles.Add(player.TeamRole.Role);
            }
            if (!hasended)
            {
                ev.Status = ROUND_END_STATUS.ON_GOING;
            }
            else
            {
                //ev.Status = ROUND_END_STATUS.ON_GOING;
                ev.Status = ROUND_END_STATUS.CI_VICTORY;
                //ev.Round.EndRound();
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
            if (spawningPlayers.Contains(ev.Player.SteamId))
            {
                spawningPlayers.Remove(ev.Player.SteamId);
                List<Role> roles = new List<Role>() { Role.SCP_173, Role.SCP_096, Role.SCP_106, Role.SCP_049 };
                if (UnityEngine.Random.Range(0, 2) != 0)
                {
                    ev.Role = (Role.CHAOS_INSURGENCY);
                    ev.Items.Add(ItemType.O5_LEVEL_KEYCARD);
                }
                else
                {
                    ev.Role = (roles[UnityEngine.Random.Range(0, roles.Count)]);
                    tpPlayers.Add(ev.Player.SteamId);
                    ev.Player.Teleport(mod11.Server.Map.GetRandomSpawnPoint(Role.CHAOS_INSURGENCY));
                }
                GameObject.FindObjectOfType<Broadcast>().CallTargetAddElement(((GameObject)ev.Player.GetGameObject()).GetComponent<NetworkIdentity>().connectionToClient, "<color=#00ff00>Kill the Class-D Personnel, or not.</color>", 5, true);
            }
        }

        void IEventHandlerUpdate.OnUpdate(UpdateEvent ev)
        {
            List<string> roles = new List<string>();
            List<string> rolesd = new List<string>();
            foreach (Player player in mod11.Server.GetPlayers())
            {
                if (player.TeamRole.Role == Role.CLASSD)
                    rolesd.Add(player.Name);
                if (player.TeamRole.Role == Role.CLASSD || player.TeamRole.Team == Smod2.API.Team.SCP)
                    roles.Add(player.Name);
            }
            if (rolesd.Count == 2 && !hasended && roundstarted && !AlphaWarheadController.host.inProgress && !AlphaWarheadController.host.detonated)
            {
                AlphaWarheadOutsitePanel.nukeside.SetEnabled(true);
                //AlphaWarheadController.host.NetworktimeToDetonation = 120;
                AlphaWarheadController.host.StartDetonation();
            }
            else if (rolesd.Count == 1 && !hasended && roundstarted)
            {
                GameObject.FindObjectOfType<Broadcast>().CallRpcAddElement("<size=70><color=#ff0000>Class-D Winner: " + rolesd[0] + "</color></size>", 10, true);
                foreach (Player player in mod11.Server.GetPlayers())
                {
                    player.SendConsoleMessage(rolesd[0] + " Wins!", "red");
                }
                hasended = true;
            }
            else if (rolesd.Count < 1 && !hasended && roundstarted)
            {
                GameObject.FindObjectOfType<Broadcast>().CallRpcAddElement("<size=70><color=#ff0000>SCP subjects win</color></size>", 10, true);
                foreach (Player player in mod11.Server.GetPlayers())
                {
                    player.SendConsoleMessage("SCP subjects win", "red");
                }
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
            if (players != null && !AlphaWarheadController.host.detonated)
            {
                if (!players.Contains(ev.Player.SteamId))
                {
                    foreach (var player in mod11.Server.GetPlayers())
                    {
                        if (player.SteamId == ev.Player.SteamId)
                        {
                            //mod11.Info(rooms.Count.ToString());
                            int roomnumber = Random.Range(0, rooms.Count);
                            Vector3 roompos = rooms[roomnumber].position;
                            //plugin.Info(rooms[roomnumber].name);
                            player.ChangeRole(Role.CLASSD);
                            player.Teleport(new Smod2.API.Vector(roompos.x, roompos.y + 2f, roompos.z));
                            player.GiveItem(Smod2.API.ItemType.ZONE_MANAGER_KEYCARD);
                            players.Add(player.SteamId);
                        }
                    }
                }
            }
        }

        void IEventHandlerPlayerHurt.OnPlayerHurt(PlayerHurtEvent ev)
        {
            if (ev.Attacker.TeamRole.Role == Role.CHAOS_INSURGENCY && ev.Player.TeamRole.Role == Role.CHAOS_INSURGENCY)
            {
                ev.Damage = 0;
            }
        }

        void IEventHandlerRoundRestart.OnRoundRestart(RoundRestartEvent ev)
        {
            roundstarted = false;
            spawningPlayers = new List<string>();
            tpPlayers = new List<string>();
        }



        void IEventHandlerTeamRespawn.OnTeamRespawn(TeamRespawnEvent ev)
        {
            ev.SpawnChaos = true;
            foreach (Player player in ev.PlayerList)
            {
                spawningPlayers.Add(player.SteamId);
                /*List<Role> roles = new List<Role>() { Role.SCP_173, Role.SCP_096, Role.SCP_106, Role.SCP_049 };
                if (UnityEngine.Random.Range(0, 2) != 0)
                {
                    player.ChangeRole(Role.CHAOS_INSURGENCY);
                    player.GiveItem(ItemType.O5_LEVEL_KEYCARD);
                }
                else
                {
                    player.ChangeRole(roles[UnityEngine.Random.Range(0, roles.Count)]);
                    player.Teleport(mod11.Server.Map.GetRandomSpawnPoint(Role.CHAOS_INSURGENCY));
                }
                GameObject.FindObjectOfType<Broadcast>().CallTargetAddElement(((GameObject)player.GetGameObject()).GetComponent<NetworkIdentity>().connectionToClient, "<color=#00ff00>Kill the Class-D Personnel, or not.</color>", 5, true);
                */
            }
        }

        void IEventHandlerInitialAssignTeam.OnAssignTeam(PlayerInitialAssignTeamEvent ev)
        {
            if (ev.Team == Smod2.API.Team.CHAOS_INSURGENCY || ev.Team == Smod2.API.Team.SCP)
            {
                spawningPlayers.Add(ev.Player.SteamId);
                /*List<Role> roles = new List<Role>() { Role.SCP_173, Role.SCP_096, Role.SCP_106, Role.SCP_049 };
                if (UnityEngine.Random.Range(0, 2) != 0)
                {
                    ev.Player.ChangeRole(Role.CHAOS_INSURGENCY);
                    ev.Player.GiveItem(ItemType.O5_LEVEL_KEYCARD);
                }
                else
                {
                    ev.Player.ChangeRole(roles[UnityEngine.Random.Range(0, roles.Count)]);
                    ev.Player.Teleport(mod11.Server.Map.GetRandomSpawnPoint(Role.CHAOS_INSURGENCY));
                }
                GameObject.FindObjectOfType<Broadcast>().CallTargetAddElement(((GameObject)ev.Player.GetGameObject()).GetComponent<NetworkIdentity>().connectionToClient, "<color=#00ff00>Kill the Class-D Personnel, or not.</color>", 5, true);
                */
            }
        }
    }
}