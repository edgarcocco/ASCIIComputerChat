using ASCIIComputerChat.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCIIComputerChat.GameObjects.Controls
{
    class TextArea
    {

        private Game game;
        public string Text { get; set; }
        private List<string> DrawText = new List<string>();
        private SpriteFont defaultFont;

        public Rectangle ControlRectangle { get { return new Rectangle((int)Position.X, (int)Position.Y, Dimensions.Width, Dimensions.Height); } }
        public Vector2 Position;
        public Dimensions Dimensions;

        private Texture2D whiteTexture;
        private RenderTarget2D TextRenderTarget;
        private int currentView;

        public TextArea(Game game, Vector2 position, Dimensions dimensions)
        {
            this.game = game;
            this.Position = position;
            this.Dimensions = dimensions;
            TextRenderTarget = new RenderTarget2D(game.GraphicsDevice, dimensions.Width, dimensions.Height);
        }

        public void LoadContent(ContentManager content)
        {
            defaultFont = ASCIIArtExtensionMethods.ASCIIArtFont;
            whiteTexture = new Texture2D(game.GraphicsDevice, 1, 1);
            whiteTexture.SetData<Color>(new Color[] { Color.White });
        }

        public void HandleInput(InputState input)
        {
            if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down) && input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                currentView += 10;
            if (input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up) && input.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.LeftControl))
                currentView -= 10;
        }

        public void Update(GameTime gameTime)
        {

        }
        
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            game.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            game.GraphicsDevice.SetRenderTarget(TextRenderTarget);
            game.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();
            for(int i = 0; i < DrawText.Count; i++)
            {
                spriteBatch.DrawString(defaultFont, DrawText[i], new Vector2(0, (defaultFont.LineSpacing * i) - currentView), Color.White);
            }
            spriteBatch.End();

            game.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            //spriteBatch.Draw(whiteTexture, ControlRectangle, Color.Red * .5f);
            spriteBatch.Draw(TextRenderTarget, ControlRectangle, new Rectangle(0, 0, ControlRectangle.Width, ControlRectangle.Height), Color.White);
            spriteBatch.End();
        }

        public void AddText(string text)
        {
            Text += text + Environment.NewLine;
            DrawText.Clear();
            string charactersCollected = String.Empty;
            for (int i = 0; i < Text.Length; i++)
            {
                charactersCollected += Text[i];
                if(defaultFont.MeasureString(charactersCollected).X > ControlRectangle.Width || Text[i] == '\n')
                {
                    DrawText.Add(charactersCollected);
                    charactersCollected = String.Empty;
                }
            }

            // Adjust the current drawing region
            if (DrawText.Count * defaultFont.LineSpacing > ControlRectangle.Height)
                currentView = (DrawText.Count * defaultFont.LineSpacing) - ControlRectangle.Height;

            if (charactersCollected != String.Empty)
                DrawText.Add(charactersCollected);
        }


        public void SetPosition(Vector2 position)
        {
            Position = position;
        }
        public void SetDimensions(Dimensions dimensions)
        {
            Dimensions.Width = dimensions.Width;
            Dimensions.Height = dimensions.Height;
            TextRenderTarget.Dispose();
            TextRenderTarget = new RenderTarget2D(game.GraphicsDevice, dimensions.Width, dimensions.Height);
        }
    }
}
