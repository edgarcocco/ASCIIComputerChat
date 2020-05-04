using ASCIIComputerChat.Scripts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        private Texture2D whiteTexture;
        private SpriteFont defaultFont;
        public string Text { get; set; }

        public Dimensions Dimensions { get { return new Dimensions(ControlRectangle.Width, ControlRectangle.Height); } }
        private Rectangle ControlRectangle;
        private RenderTarget2D TextRenderTarget;
        private int LastHeight;
        private int Y = 0;
        private int Max_Y;
        private int TextHeight;

        public bool Focus;

        private bool hide;
        public int LatestLinesWrapped = 1;

        public TextArea(Game game, Vector2 position, Dimensions dimensions)
        {
            this.game = game;
            ControlRectangle = new Rectangle((int)position.X, (int)position.Y, dimensions.Width, dimensions.Height);
        }

        public void LoadContent(ContentManager content)
        {
            GraphicsDevice GraphicsDevice = game.GraphicsDevice;
            whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            whiteTexture.SetData<Color>(new Color[] { Color.White });
            var pp = game.GraphicsDevice.PresentationParameters;

            defaultFont = ASCIIComputerChat.Scripts.ASCIIArtExtensionMethods.ASCIIArtFont;
            Text = String.Empty;

            TextRenderTarget = new RenderTarget2D(GraphicsDevice, ControlRectangle.Width, ControlRectangle.Height);
            LastHeight = TextRenderTarget.Height;
        }

        #region Event Handler
        void ScrollUp()
        {
            Y -= defaultFont.LineSpacing;
            Y = Math.Max(0, Y);
        }
        void ScrollUpPressed()
        {
            Y -= 2;
            Y = Math.Max(0, Y);
        }

        void ScrollDown()
        {
            Y += defaultFont.LineSpacing;
            Y = Math.Min(Max_Y, Y);
        }
        void ScrollDownPressed()
        {
            Y += 2;
            Y = Math.Min(Max_Y, Y);
        }
        #endregion

        public void HandleInput(InputState input)
        {
        }
        public void Update(GameTime gameTime)
        {
            if (ControlRectangle.Height != TextRenderTarget.Height)
            {
                TextRenderTarget.Dispose();
                TextRenderTarget = new RenderTarget2D(game.GraphicsDevice, ControlRectangle.Width, ControlRectangle.Height);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            game.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            game.GraphicsDevice.SetRenderTarget(TextRenderTarget);
            game.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
            string[] textArray = Text.Split('\n');
            int n_Y = 0;
            for (int i = 0; i < textArray.Length; i++)
            {
                if (textArray[i] != String.Empty)
                {
                    if (textArray[i].StartsWith("@"))
                    {
                        string[] rawText = textArray[i].Split(new[] { ':' }, 2);
                        string user = rawText[0] + ":";
                        float x = defaultFont.MeasureString(user).X;
                        spriteBatch.DrawString(defaultFont, user, new Vector2(0, -Y + n_Y), Color.Wheat);
                        string parsedText = rawText[1];
                        spriteBatch.DrawString(defaultFont, parsedText, new Vector2(x, -Y + n_Y), Color.WhiteSmoke);
                    }
                    else
                    {
                        //float newY = 0;
                        //if (n_Y != 0)
                        //    newY = -Y + n_Y;
                        spriteBatch.DrawString(defaultFont, textArray[i], new Vector2(0, n_Y - Y), Color.White);
                    }
                    n_Y += defaultFont.LineSpacing;
                }
            }
            spriteBatch.End();

            game.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin();
            if(!hide)
                spriteBatch.Draw(TextRenderTarget, new Vector2(3+ControlRectangle.X, ControlRectangle.Y), new Rectangle(0, 0, ControlRectangle.Width, ControlRectangle.Height), Color.White);

            spriteBatch.Draw(whiteTexture, ControlRectangle, Color.Red * .5f);

            spriteBatch.End();
            
        }

        public void AddText(string text)
        {
            try
            {
                TextHeight += defaultFont.LineSpacing;
                AdjustTextPosition();
                if (defaultFont.MeasureString(text).X > ControlRectangle.Width)
                    WrapText(text);
                else
                    this.Text += text + Environment.NewLine;
            }
            catch
            {
                string appender = String.Empty;
                try
                {
                    foreach (char c in text)
                    {
                        appender += c;
                        float test = defaultFont.MeasureString(appender).X;
                    }
                }
                catch
                {
                    Console.WriteLine("The part of the string that has the error is ");
                    Console.WriteLine(appender);
                }

            }
        }

        public void WrapText(string text)
        {
            LatestLinesWrapped = 1;
            string currentText = text;
            char[] charArray = currentText.ToCharArray();

            currentText = String.Empty;
            string container = String.Empty;

            foreach (char c in charArray)
            {
                if (defaultFont.MeasureString(container).X > ControlRectangle.Width - 20) 
                {
                    currentText += Environment.NewLine;
                    container = String.Empty;
                    if (Y < TextHeight)
                    {
                        TextHeight += defaultFont.LineSpacing;
                        Y += defaultFont.LineSpacing;
                    }
                    LatestLinesWrapped++;
                }
                currentText += c; 
                container += c;                
            }

            this.Text += currentText + Environment.NewLine;
        }
        public void AdjustTextPosition()
        {
            if (TextHeight > ControlRectangle.Height)
            {
                if (TextHeight > Y)
                {
                    Max_Y += defaultFont.LineSpacing;
                    Y = Max_Y;
                }
            }
        }

        public void SetDimensions(Dimensions dimensions)
        {
            ControlRectangle.Width = dimensions.Width;
            ControlRectangle.Height = dimensions.Height;
            TextRenderTarget.Dispose();
            TextRenderTarget = new RenderTarget2D(game.GraphicsDevice, dimensions.Width, dimensions.Height);

        }

        public void SetPosition(Vector2 position)
        {
            ControlRectangle.X = (int)position.X;
            ControlRectangle.Y = (int)position.Y;
        }

        public void Clear()
        {
            TextRenderTarget = new RenderTarget2D(game.GraphicsDevice, ControlRectangle.Width, ControlRectangle.Height);
            LastHeight = TextRenderTarget.Height;

            Y = 0;
            Text = String.Empty;
        }
        public void Hide()
        {
            hide = true;
        }

        public void Show()
        {
            hide = false;
        }
    }
}
