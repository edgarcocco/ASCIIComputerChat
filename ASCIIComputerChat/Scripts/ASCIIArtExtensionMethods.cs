using ASCIIArtLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIComputerChat.Scripts
{
    static class ASCIIArtExtensionMethods
    {
        public static SpriteFont ASCIIArtFont;
        public static void Draw(this SpriteBatch spriteBatch, ASCIIArt asciiArt, Vector2 Position, Color color)
        {
            string[] splittedArt = asciiArt.Art.Split('\n');
            for(int i = 0; i < splittedArt.Length; i++)
            {
                spriteBatch.DrawString(ASCIIArtFont, splittedArt[i], new Vector2(Position.X, Position.Y + (ASCIIArtFont.LineSpacing * i)), color);
            }
        }

    }
}
