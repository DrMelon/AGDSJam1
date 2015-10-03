using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
namespace AGDSJam1
{
    class Machine : Entity
    {
        Spritemap<string> gfxTop;
        Spritemap<string> gfxDetail;
        string Name;
        string Description;
        string FlavBroke;
        string FlavFixing;
        float FixTime;
        float BreakTime;
        int Status;
        bool BeingInspected;

        public Machine(float x, float y, string machineName, string description, string flav_broke, string flav_fixin, float fixtime, string asset_top, string asset_detail)
        {
            X = x;
            Y = y;
            gfxTop = new Spritemap<string>(asset_top, 64, 64);
            gfxDetail = new Spritemap<string>(asset_detail, 64, 64);
            gfxTop.Add("default", new Anim(new int[] { 0 }, new float[] { 1 }));
            gfxDetail.Add("normal", new Anim(new int[] { 0 }, new float[] { 1 }));
            gfxDetail.Add("broken", new Anim(new int[] { 1 }, new float[] { 1 }));
            gfxDetail.Add("fixing", new Anim(new int[] { 2 }, new float[] { 1 }));

            Name = machineName;
            Description = description;
            FlavBroke = flav_broke;
            FlavFixing = flav_fixin;
            BreakTime = Rand.Float(60 * 5, 60 * 60);
            FixTime = fixtime;

            Status = 1;
            
        }

        public void Inspect()
        {
            // Create window for detail view and text box.

            // Display detail view and text box.
        }

        public void BeginFix()
        {
            Status = 3;

        }

        public void Break()
        {
            Status = 2;
        }


    }
}
