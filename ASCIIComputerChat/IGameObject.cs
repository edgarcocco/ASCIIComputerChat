using ASCIIComputerChat.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIComputerChat
{
    interface IGameObject
    {
        bool SkipDraw { get; set; }
        void Initialize();
        void LoadContent(ContentManager content);
        void Update(GameTime gameTime);
        void HandleInput(InputState inputState);
        void Draw(SpriteBatch spriteBatch, GameTime gameTime);
    }
}
