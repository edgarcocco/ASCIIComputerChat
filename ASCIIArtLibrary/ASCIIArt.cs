using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIArtLibrary
{
    public class ASCIIArt
    {
        public string Art { get; set; }

        public ASCIIArt(string art)
        {
            Art = art;
        }

        public Rectangle GetArtBounds(SpriteFont ASCIIArtFont)
        {
            Vector2 measuredArt = ASCIIArtFont.MeasureString(Art);
            Rectangle bounds = new Rectangle(0,0, (int)measuredArt.X, (int)measuredArt.Y);
            
            return bounds;
        }
    }
}
