/*
    Copyright 2015 MCGalaxy
    
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
using MCGalaxy.Commands;
using MCGalaxy.DB;
using MCGalaxy.Games;

namespace MCGalaxy.Eco {
    
    public sealed class HammerItem : SimpleItem 
    {
        
        public HammerItem() 
        {
            Aliases = new string[] { "hammer", "ham", "z" };
            Price = 20;
        }
        
        public override string Name { get { return "100Hammers"; } }

        protected internal override void OnPurchase(Player p, string args) 
        {
            int count = 1;
            const string group = "Number of groups of 100 blocks";
            if (args.Length > 0 && !CommandParser.GetInt(p, args, group, ref count, 0, 100)) return;
            
            if (!CheckPrice(p, count * Price, (count * 100) + " blocks")) return;
            
            LSData data = LSGame.Get(p);
            data.HammerBlocks += 100 * count;
            Economy.MakePurchase(p, Price * count, "%3100Hammers: " + (100 * count));
        }

        protected internal override void OnStoreCommand(Player p) 
        {
            p.Message("&T/Buy 100Hammers [num]");
            p.Message("&HCosts &a{0} * [num] &H{1}", Price, Server.Config.Currency);
            p.Message("Increases the blocks you are able to draw by 100 * [num].");
        }
    }

    public sealed class LifeItem : SimpleItem 
    {
        
        public LifeItem() 
        {
            Aliases = new string[] { "life" };
            Price = 10;
        }
        
        public override string Name { get { return "Life"; } }

        protected internal override void OnPurchase(Player p, string args) 
        {
            if (!CheckPrice(p)) return;

            LSData data = LSGame.Get(p);
            data.TimesDied -= 1;

            Economy.MakePurchase(p, Price, "%3Life:");
        }

        protected internal override void OnStoreCommand(Player p) 
        {
            p.Message("&T/Buy Life");
            p.Message("&HCosts &a{0} &H{1}", Price, Server.Config.Currency);
            p.Message("Gives you one life.");
        }
    }

    public sealed class WaterItem : SimpleItem 
    {
        
        public WaterItem() 
        {
            Aliases = new string[] { "water", "wat", "w" };
            Price = 15;
        }
        
        public override string Name { get { return "20Water"; } }

        protected internal override void OnPurchase(Player p, string args) 
        {
            int count = 1;
            const string group = "Number of groups of 20 blocks";
            if (args.Length > 0 && !CommandParser.GetInt(p, args, group, ref count, 0, 20)) return;
            
            if (!CheckPrice(p, count * Price, (count * 20) + " blocks")) return;
            
            LSData data = LSGame.Get(p);
            data.WaterBlocks += 20 * count;
            Economy.MakePurchase(p, Price * count, "%320Water: " + (20 * count));
        }

        protected internal override void OnStoreCommand(Player p) 
        {
            p.Message("&T/Buy 20Water [num]");
            p.Message("&HCosts &a{0} * [num] &H{1}", Price, Server.Config.Currency);
            p.Message("Be able to place water.");
        }
    }

    public sealed class SpongeItem : SimpleItem 
    {
        
        public SpongeItem() 
        {
            Aliases = new string[] { "sponge", "spon", "sp" };
            Price = 20;
        }
        
        public override string Name { get { return "5Sponges"; } }

        protected internal override void OnPurchase(Player p, string args) 
        {
            int count = 1;
            const string group = "Number of groups of 5 blocks";
            if (args.Length > 0 && !CommandParser.GetInt(p, args, group, ref count, 0, 5)) return;
            
            if (!CheckPrice(p, count * Price, (count * 5) + " blocks")) return;
            
            LSData data = LSGame.Get(p);
            data.SpongeBlocks += 5 * count;
            Economy.MakePurchase(p, Price * count, "%35Sponges: " + (5 * count));
        }

        protected internal override void OnStoreCommand(Player p) 
        {
            p.Message("&T/Buy 5Sponges [num]");
            p.Message("&HCosts &a{0} * [num] &H{1}", Price, Server.Config.Currency);
            p.Message("Allows you to absorb water and lava, disolves quickly.");
        }
    }

    public sealed class DoorItem : SimpleItem 
    {
        
        public DoorItem() 
        {
            Aliases = new string[] { "door", "d" };
            Price = 20;
        }
        
        public override string Name { get { return "6Doors"; } }

        protected internal override void OnPurchase(Player p, string args) 
        {
            int count = 1;
            const string group = "Number of groups of 6 blocks";
            if (args.Length > 0 && !CommandParser.GetInt(p, args, group, ref count, 0, 6)) return;
            
            if (!CheckPrice(p, count * Price, (count * 6) + " blocks")) return;
            
            LSData data = LSGame.Get(p);
            data.SpongeBlocks += 6 * count;
            Economy.MakePurchase(p, Price * count, "%36Doors: " + (6 * count));
        }

        protected internal override void OnStoreCommand(Player p) 
        {
            p.Message("&T/Buy 6Doors [num]");
            p.Message("&HCosts &a{0} * [num] &H{1}", Price, Server.Config.Currency);
            p.Message("Allows you to place doors using /door.");
        }
    }

    public sealed class TeleportItem : SimpleItem 
    {
        
        public TeleportItem() 
        {
            Aliases = new string[] { "tp" };
            Price = 20;
        }
        
        public override string Name { get { return "Teleport"; } }

        protected internal override void OnPurchase(Player p, string args) 
        {
            if (!CheckPrice(p)) return;

            LSData data = LSGame.Get(p);
            data.Teleports += 1;

            Economy.MakePurchase(p, Price, "%3Teleport:");
        }

        protected internal override void OnStoreCommand(Player p) 
        {
            p.Message("&T/Buy Teleport");
            p.Message("&HCosts &a{0} &H{1}", Price, Server.Config.Currency);
            p.Message("Allows you to teleport to anyone.");
        }
    }
}
