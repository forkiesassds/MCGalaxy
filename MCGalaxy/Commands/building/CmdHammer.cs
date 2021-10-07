/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using MCGalaxy.Games;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdHammer : DrawCmd {
        public override string name { get { return "Hammer"; } }
        
        protected override DrawMode GetMode(string[] parts) 
        {
            return DrawMode.normal;
        }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) 
        {
            return new HammerDrawOp();
        }
        
        protected override void GetBrush(DrawArgs dArgs) 
        {
            dArgs.BrushName = "Normal";
            dArgs.BrushArgs = dArgs.Message.Splice(dArgs.ModeArgsCount, 0);
        }

        public override void Help(Player p) 
        {
            p.Message("&T/Hammer <brush args>");
            p.Message("&HAllows you to build faster.");
            p.Message(BrushHelpLine);
        }
    }
    public class HammerDrawOp : DrawOp
    {
        public override string Name { get { return "Hammer"; } }
        
        public override long BlocksAffected(Level lvl, Vec3S32[] marks) 
        {
            return (Max.X - Min.X + 1) * (Max.Y - Min.Y + 1) * (Max.Z - Min.Z + 1);
        }
        
        public override void Perform(Vec3S32[] marks, Brush brush, DrawOpOutput output) 
        {
            Vec3U16 p1 = Clamp(Min), p2 = Clamp(Max);
            for (ushort y = p1.Y; y <= p2.Y; y++)
                for (ushort z = p1.Z; z <= p2.Z; z++)
                    for (ushort x = p1.X; x <= p2.X; x++)
            {
                output(Place(x, y, z, brush));
            }
        }

        public override bool CanDraw(Vec3S32[] marks, Player p, long affected) 
        {
            if (affected <= (LSGame.Get(p).HammerBlocks)) 
            {
                (LSGame.Get(p).HammerBlocks) -= (int)affected;
                return true;
            }
            p.Message("You tried to draw " + affected + " blocks.");
            p.Message("But your hammer can only draw " + (LSGame.Get(p).HammerBlocks) + " blocks.");
            return false;
        }
    } 
}
