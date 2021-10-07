﻿/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.Threading;

namespace MCGalaxy.Games 
{
    public sealed partial class LSGame : RoundsGame 
    {

        protected override void DoRound() 
        {
            if (!Running) return;

            ResetPlayerDeaths();
            RoundStart = DateTime.UtcNow;
            RoundInProgress = true;
            Logger.Log(LogType.GameActivity, "[Lava Survival] Round started. Map: " + Map.ColoredName);
            
            Map.SetPhysics(destroyMode ? 2 : 1);           
            int secs = 0, layerSecs = 0;
            
            while (RoundInProgress && secs < roundTotalSecs) 
            {
                if (!Running) return;
                if ((secs % 60) == 0 && !flooded) { Map.Message(FloodTimeLeftMessage()); }
                
                if (secs >= floodDelaySecs) 
                {
                    if (layerMode && (layerSecs % layerIntervalSecs) == 0 && curLayer <= cfg.LayerCount) 
                    {
                        ushort y = (ushort)(cfg.LayerPos.Y + ((cfg.LayerHeight * curLayer) - 1));
                        Map.Blockchange(cfg.LayerPos.X, y, cfg.LayerPos.Z, floodBlock, true);
                        curLayer++;
                    } 
                    else if (!layerMode && secs == floodDelaySecs) 
                    {
                        Map.Message("&4Look out, here comes the flood!");
                        Logger.Log(LogType.GameActivity, "[Lava Survival] Starting map flood.");
                        Map.Blockchange(cfg.FloodPos.X, cfg.FloodPos.Y, cfg.FloodPos.Z, floodBlock, true);
                    }
                    
                    layerSecs++;
                    flooded = true;
                }
                
                secs++; Thread.Sleep(1000);
            }
        }

        public override void EndRound() 
        {
            if (!RoundInProgress) return;
            RoundInProgress = false;
            flooded = false;
            
            Map.SetPhysics(5);
            Map.Message("The round has ended!");
        }

        internal string FloodTimeLeftMessage() 
        {
            TimeSpan left = RoundStart.Add(cfg.FloodTime) - DateTime.UtcNow;
            return "&3" + left.Shorten(true) + " &Suntil the flood.";
        }
        
        internal string RoundTimeLeftMessage() 
        {
            TimeSpan left = RoundStart.Add(cfg.RoundTime) - DateTime.UtcNow;
            return "&3" + left.Shorten(true) + " &Suntil the round ends.";
        }

        public override void OutputStatus(Player p) 
        {
            string block = waterMode ? "water" : "lava";
            
            // TODO: send these messages if player is op
            //if (data.layer) {
            //    Map.ChatLevelOps("There will be " + mapSettings.LayerCount + " layers, each " + mapSettings.LayerHeight + " blocks high.");
            //    Map.ChatLevelOps("There will be another layer every " + mapSettings.layerInterval + " minutes.");
            //}
            
            if (waterMode) p.Message("The map will be flooded with &9water &Sthis round!");
            if (layerMode) p.Message("The " + block + " will &aflood in layers &Sthis round!");
            
            if (fastMode) p.Message("The lava will be &cfast &Sthis round!");
            if (killerMode) p.Message("The " + block + " will &ckill you &Sthis round!");
            if (destroyMode) p.Message("The " + block + " will &cdestroy plants " + (waterMode ? "" : "and flammable blocks ") + "&Sthis round!");
            
            if (!flooded) p.Message(FloodTimeLeftMessage());
            p.Message(RoundTimeLeftMessage());
        }

        protected override bool SetMap(string map) 
        {
            if (!base.SetMap(map)) return false;
            Map.Config.PhysicsOverload = 1000000;
            return true;
        }

        void KillPlayer(Player p) 
        {
            if (Config.MaxLives <= 0) return;
            Get(p).TimesDied++;
            if (!IsPlayerDead(p)) return;
            
            Chat.MessageFromLevel(p, "λNICK &4ran out of lives, and is out of the round!");
            if(Eco.Economy.EnabledItemNames().Contains("Life") && Eco.Economy.Enabled)
                p.Message("&4You can still watch, but you cannot build. Unless you buy a life using /buy life.");
            else
                p.Message("&4You can still watch, but you cannot build.");
        }
    }
}
