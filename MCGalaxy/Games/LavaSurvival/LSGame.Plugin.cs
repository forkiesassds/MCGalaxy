/*
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
using MCGalaxy.Events;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.LevelEvents;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    public sealed partial class LSGame : RoundsGame {

        protected override void HookEventHandlers() {
            OnBlockChangingEvent.Register(HandleBlockChanging, Priority.High);  
            OnJoinedLevelEvent.Register(HandleJoinedLevel, Priority.High);           
            OnPlayerConnectEvent.Register(HandlePlayerConnect, Priority.High);
            OnPlayerDeathEvent.Register(HandlePlayerDeath, Priority.High);
            
            base.HookEventHandlers();
        }
        
        protected override void UnhookEventHandlers() {
            OnBlockChangingEvent.Unregister(HandleBlockChanging);  
            OnJoinedLevelEvent.Unregister(HandleJoinedLevel);            
            OnPlayerConnectEvent.Unregister(HandlePlayerConnect);
            OnPlayerDeathEvent.Unregister(HandlePlayerDeath);
            
            base.UnhookEventHandlers();
        }
        
        void HandleJoinedLevel(Player p, Level prevLevel, Level level, ref bool announce) {
            HandleJoinedCommon(p, prevLevel, level, ref announce);
            
            if (Map != level) return;            
            MessageMapInfo(p);
            if (RoundInProgress) OutputStatus(p);
        }

        void HandlePlayerConnect(Player p) {
            p.Message("&cLava Survival &Sis running! Type &T/ls go &Sto join");
        }
        
        void HandlePlayerDeath(Player p, BlockID block) 
        {
            if (p.level != Map) return;
         
            if (IsPlayerDead(p)) 
            {
                p.cancelDeath = true;
            } 
            else 
            {
                KillPlayer(p);
            }
        }

        void HandleBlockChanging (Player p, ushort x, ushort y, ushort z, BlockID block, bool placing, ref bool cancel)
        {
            if (LSGame.Instance.Map != p.level) return;
            LSData data = Get(p);
            ushort blockid = block;
            if (placing || (!placing && p.painting)) 
            {
                if (p.ModeBlock != Block.Invalid) blockid = p.ModeBlock; //workaround to prevent players from using /mode to bypass this restriction.
                if (blockid == Block.Water || blockid == Block.StillWater || blockid == Block.Sponge || blockid == Block.Door_Log)
                {
                    int blocks = 0;
                    switch(blockid) 
                    {
                        case Block.Water: 
                            blocks = data.WaterBlocks; break;
                        case Block.StillWater: 
                            blocks = data.WaterBlocks; break;
                        case Block.Sponge:
                            blocks = data.SpongeBlocks; break;
                        case Block.Door_Log:
                            blocks = data.DoorBlocks; break;
                    }

                    if (blocks <= 0) 
                    {
                        p.Message("You have no blocks left.");
                        p.RevertBlock(x, y, z); cancel = true; return;
                    }

                    //ugly copy paste!!!!!
                    switch(blockid) 
                    {
                        case Block.Water: 
                            data.WaterBlocks--; break;
                        case Block.StillWater: 
                            data.WaterBlocks--; break;
                        case Block.Sponge:
                            data.SpongeBlocks--; break;
                        case Block.Door_Log:
                            data.DoorBlocks--; break;
                    }
                    
                    if ((blocks % 10) == 0 || blocks <= 10) 
                    {
                        p.Message("Blocks Left: &4" + blocks);
                    }
                }
            }
        }
    }
}
