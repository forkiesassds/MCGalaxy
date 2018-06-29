/*
    Copyright 2011 MCForge
    Created by Techjar (Jordan S.)
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdLavaSurvival : RoundsGameCmd {
        public override string name { get { return "LavaSurvival"; } }
        public override string shortcut { get { return "LS"; } }
        protected override RoundsGame Game { get { return Server.lava; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage lava survival") }; }
        }

        protected override void HandleSet(Player p, RoundsGame game, string[] args) {
            if (!CheckExtraPerm(p, 1)) return;
            
            if (args.Length < 2) { Help(p, "set"); return; }
            string prop = args[1];
            
            if (prop.CaselessEq("add")) {
                HandleAdd(p, p.level);
            } else if (IsDeleteCommand(prop)) {
                HandleRemove(p, p.level);
            } else if (prop.CaselessEq("spawn")) {
                HandleSetSpawn(p, args);
            } else if (prop.CaselessEq("block")) {
                HandleSetBlock(p, args);
            } else if (prop.CaselessEq("other")) {
                HandleSetOther(p, args);
            } else {
                Help(p, "set");
            }
        }
        
        static void HandleAdd(Player p, Level lvl) {
            List<string> maps = LSGame.Config.Maps;
            if (maps.CaselessContains(lvl.name)) {
                Player.Message(p, "{0} %Sis already in the list of Lava survival maps", lvl.ColoredName);
            } else {
                Player.Message(p, "Added {0} %Sto the list of Lava survival maps", lvl.ColoredName);
                maps.Add(lvl.name);
                lvl.Config.LoadOnGoto = false;
                
                LSGame.Config.Save();
                Level.SaveSettings(lvl);
            }
        }
        
        static void HandleRemove(Player p, Level lvl) {
            if (!LSGame.Config.Maps.CaselessRemove(lvl.name)) {
                Player.Message(p, "{0} %Swas not in the list of Lava survival maps", lvl.ColoredName);
            } else {
                Player.Message(p, "Removed {0} %Sfrom the list of Lava survival maps", lvl.ColoredName);
                lvl.Config.AutoUnload = true;
                lvl.Config.LoadOnGoto = true;
                
                LSGame.Config.Save();
                Level.SaveSettings(lvl);
            }
        }

        static bool ParseChance(Player p, string arg, string[] args, ref int value) {
            if (!CommandParser.GetInt(p, args[3], "Chance", ref value, 0, 100)) return false;
            Player.Message(p, "{0} chance: &b{1}%", arg, value);
            return true;
        }
        
        static bool ParseTimespan(Player p, string arg, string[] args, ref float value) {
            TimeSpan span = default(TimeSpan);
            if (!CommandParser.GetTimespan(p, args[3], ref span, "set " + arg + " to", "m")) return false;
            
            value = (float)span.TotalMinutes;
            Player.Message(p, "{0}: &b{1}", arg, span.Shorten(true));
            return true;
        }
        
        
        void HandleSetSpawn(Player p, string[] args) {
            LSMapConfig cfg = RetrieveConfig(p);
            if (args.Length < 3) {
                Player.Message(p, "Flood position: &b" + cfg.FloodPos);
                Player.Message(p, "Layer position: &b" + cfg.LayerPos);
                Player.Message(p, "Layer flood chance: &b" + cfg.LayerChance + "%");
                Player.Message(p, "  &b{0} %Slayers, each &b{1} %Sblocks tall",
                               cfg.LayerCount, cfg.LayerHeight);
                return;
            }
            
            string prop = args[2];
            if (prop.CaselessEq("flood")) {
                Player.Message(p, "Place or destroy the block you want to be the total flood block spawn point.");
                p.MakeSelection(1, cfg, SetFloodPos);
                return;
            } else if (prop.CaselessEq("layer")) {
                Player.Message(p, "Place or destroy the block you want to be the layer flood base spawn point.");
                p.MakeSelection(1, cfg, SetLayerPos);
                return;
            }
            
            if (args.Length < 4) { Help(p, "spawn"); return; }
            bool ok = false;
            
            if (prop.CaselessEq("height")) {
                ok = CommandParser.GetInt(p, args[3], "Height", ref cfg.LayerHeight, 0);
                if (ok) Player.Message(p, "Layer height: &b" + cfg.LayerHeight + " blocks");
            } else if (prop.CaselessEq("count")) {
                ok = CommandParser.GetInt(p, args[3], "Count", ref cfg.LayerCount, 0);
                if (ok) Player.Message(p, "Layer count: &b" + cfg.LayerCount);
            } else if (prop.CaselessEq("chance")) {
                ok = ParseChance(p, "Layer flood", args, ref cfg.LayerChance);
            } else {
                Help(p, "spawn");
            }
            
            if (ok) UpdateConfig(p, cfg);
        }
        
        static bool SetFloodPos(Player p, Vec3S32[] m, object state, BlockID block) {
            LSMapConfig cfg = (LSMapConfig)state;
            cfg.FloodPos = (Vec3U16)m[0];
            UpdateConfig(p, cfg);

            Player.Message(p, "Flood position set to &b({0})", m[0]);
            return false;
        }
        
        static bool SetLayerPos(Player p, Vec3S32[] m, object state, BlockID block) {
            LSMapConfig cfg = (LSMapConfig)state;
            cfg.LayerPos = (Vec3U16)m[0];
            UpdateConfig(p, cfg);

            Player.Message(p, "Layer position set to &b({0})", m[0]);
            return false;
        }
        
        void HandleSetBlock(Player p, string[] args) {
            LSMapConfig cfg = RetrieveConfig(p);
            if (args.Length < 3) {
                Player.Message(p, "Fast lava chance: &b" + cfg.FastChance + "%");
                Player.Message(p, "Killer lava/water chance: &b" + cfg.KillerChance + "%");
                Player.Message(p, "Destroy blocks chance: &b" + cfg.DestroyChance + "%");
                Player.Message(p, "Water flood chance: &b" + cfg.WaterChance + "%");
                return;
            }
            
            string prop = args[2];
            if (args.Length < 4) { Help(p, "block"); return; }
            bool ok = false;
            
            if (prop.CaselessEq("fast")) {
                ok = ParseChance(p, "Fast lava", args, ref cfg.FastChance);
            } else if (prop.CaselessEq("killer")) {
                ok = ParseChance(p, "Killer lava/water", args, ref cfg.KillerChance);
            } else if (prop.CaselessEq("destroy")) {
                ok = ParseChance(p, "Destroy blocks", args, ref cfg.DestroyChance);
            } else if (prop.CaselessEq("water")) {
                ok = ParseChance(p, "Water flood", args, ref cfg.WaterChance);
            } else {
                Help(p, "block");
            }
            
            if (ok) UpdateConfig(p, cfg);
        }
        
        void HandleSetOther(Player p, string[] args) {
            LSMapConfig cfg = RetrieveConfig(p);
            if (args.Length < 3) {
                Player.Message(p, "Layer time: &b" + cfg.LayerIntervalMins + " minutes");
                Player.Message(p, "Round time: &b" + cfg.RoundTimeMins + " minutes");
                Player.Message(p, "Flood time: &b" + cfg.RoundTimeMins + " minutes");
                Player.Message(p, "Safe zone: &b({0}) ({1})", cfg.SafeZoneMin, cfg.SafeZoneMax);
                return;
            }
            
            string prop = args[2];
            if (prop.CaselessEq("safe")) {
                Player.Message(p, "Place or break two blocks to determine the edges.");
                p.MakeSelection(2, cfg, SetSafeZone);
                return;
            }
            
            if (args.Length < 4) { Help(p, "other"); return; }
            bool ok = false;
            
            if (prop.CaselessEq("layer")) {
                ok = ParseTimespan(p, "Layer time", args, ref cfg.LayerIntervalMins);
            } else if (prop.CaselessEq("round")) {
                ok = ParseTimespan(p, "Round time", args, ref cfg.RoundTimeMins);
            } else if (prop.CaselessEq("flood")) {
                ok = ParseTimespan(p, "Flood time", args, ref cfg.FloodTimeMins);
            } else {
                Help(p, "other");
            }
            
            if (ok) UpdateConfig(p, cfg);
        }
        
        static bool SetSafeZone(Player p, Vec3S32[] m, object state, BlockID block) {
            LSMapConfig cfg = (LSMapConfig)state;
            cfg.SafeZoneMin = (Vec3U16)Vec3S32.Min(m[0], m[1]);
            cfg.SafeZoneMax = (Vec3U16)Vec3S32.Max(m[0], m[1]);
            UpdateConfig(p, cfg);

            Player.Message(p, "Safe zone set! &b({0}) ({1})", cfg.SafeZoneMin, cfg.SafeZoneMax);
            return false;
        }
        
        static LSMapConfig RetrieveConfig(Player p) {
            LSMapConfig cfg = new LSMapConfig();
            cfg.SetDefaults(p.level);
            cfg.Load(p.level.name);
            return cfg;
        }
        
        static void UpdateConfig(Player p, LSMapConfig cfg) {
            cfg.Save(p.level.name);
            if (p.level == Server.lava.Map) Server.lava.UpdateMapConfig();
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("set")) {
                Player.Message(p, "%T/Help LS spawn %H- Views help for lava spawn settings");
                Player.Message(p, "%T/Help LS block %H- Views help for lava block settings");
                Player.Message(p, "%T/Help LS other %H- Views help other settings");
            } else if (message.CaselessEq("spawn")) {
                Player.Message(p, "%T/LS set spawn %H- View lava spawns and layer info");
                Player.Message(p, "%T/LS set spawn flood %H- Set position lava floods from");
                Player.Message(p, "%T/LS set spawn layer %H- Set start position layers flood from");
                Player.Message(p, "%T/LS set spawn height [height] %H- Sets height of each layer");
                Player.Message(p, "%T/LS set spawn count [count] %H- Sets number of layers to flood");
                Player.Message(p, "%T/LS set spawn layer [chance] %H- Sets chance of layer flooding");
            } else if (message.CaselessEq("block")) {
                Player.Message(p, "%T/LS set block %H- View lava block type settings");
                Player.Message(p, "%T/LS set block fast [chance] %H- Sets chance of fast lava");
                Player.Message(p, "%T/LS set block water [chance]");
                Player.Message(p, "%HSets chance of water instead of lava flood");
                Player.Message(p, "%T/LS set block killer [chance]");
                Player.Message(p, "%HSets chance of killer lava/water");
                Player.Message(p, "%T/LS set block destroy [chance]");
                Player.Message(p, "%HSets chance of the lava/water destroying blocks");
            } else if (message.CaselessEq("other")) {
                Player.Message(p, "%T/LS set other %H- View times and safe zone location");
                Player.Message(p, "%T/LS set other safe %H- Sets safe area that can't be flooded");
                Player.Message(p, "%T/LS set other layer [timespan]");
                Player.Message(p, "%HSet interval between layer floods");
                Player.Message(p, "%T/LS set other flood [timespan]");
                Player.Message(p, "%HSet how long until the map is flooded");
                Player.Message(p, "%T/LS set other round [timespan]");
                Player.Message(p, "%HSets how long until the round ends");
            } else {
                base.Help(p, message);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/LS start <map> %H- Starts Lava Survival");
            Player.Message(p, "%T/LS stop %H- Stops Lava Survival");
            Player.Message(p, "%T/LS end %H- Ends current round of Lava Survival");
            Player.Message(p, "%T/LS set add/remove %H- Adds/removes current map from map list");
            Player.Message(p, "%T/LS set [property] %H- Sets a property. See %T/Help LS set");
            Player.Message(p, "%T/LS status %H- View current round info and time");
            Player.Message(p, "%T/LS go %H- Moves you to the current Lava Survival map");
        }
    }
}
