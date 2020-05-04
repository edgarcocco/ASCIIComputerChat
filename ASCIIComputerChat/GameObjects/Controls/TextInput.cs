using ASCIIArtComputerChat.Scripts;
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
    /// <summary>
    /// This TextInput has been tweaked to work with monospace fonts only.
    /// </summary>
    class TextInput : IGameObject
    {
        private Game Game;
        GraphicsDevice GraphicsDevice;
        private RenderTarget2D TextRenderTarget;
        //private Rectangle ControlRectangle;
        private Vector2 CursorPosition;
        private SpriteFont defaultFont;
        Rectangle ControlRectangle;

        private Texture2D whiteTexture;
        public Color BorderColor = Color.Black;
        public Dimensions CursorDimensions;
        public readonly int DefaultHeight;
        private float delay;
        private bool DrawCursor;
        private bool CursorPositionUpdated;
        private float view_X;

        private string value = String.Empty;
        private bool focus;
        public bool enabled = true;
        private const string CHAR_TO_MEASURE = "y";
        public bool IsMultiLine = false;

        public bool LineWrap = false;
        private int lineIndex = 1;
        private int maxCharsInLine = 0;

        #region Properties
        public string Value
        {
            get { return value; }
            set
            {
                this.value = String.Empty;
                foreach (char c in value)
                    AddCharacter(c);
            }
        }
        public bool Focus { get { return focus; } set { if (Enabled) focus = value; else focus = false; } }
        public bool Enabled { get { return enabled; } set { enabled = value; } }

        public bool SkipDraw { get; set; }
        #endregion


        public TextInput(Game game, Vector2 position, int width, int height)
        {
            if(!EventInput.initialized)
                EventInput.Initialize(game.Window);
            EventInput.CharEntered += EventInput_CharEntered;
            EventInput.KeyDown += EventInput_KeyDown;
            Game = game;
            GraphicsDevice = game.GraphicsDevice;
            ControlRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);
            SkipDraw = true;
            CursorDimensions = new Dimensions(width, height);
            DefaultHeight = height;
        }
        public void Initialize() { }
        void EventInput_ControlKeyPressed()
        {
            Console.WriteLine("true");
        }
        public void LoadContent(ContentManager Content) 
        {
            GraphicsDevice GraphicsDevice = Game.GraphicsDevice;
            
            defaultFont = Content.Load<SpriteFont>("Consolas");
            //defaultFont = ASCIIArtExtensionMethods.ASCIIArtFont;
            whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
            whiteTexture.SetData<Color>(new Color[] { Color.White });
            //ControlRectangle = ControlRectangle;
            TextRenderTarget = new RenderTarget2D(GraphicsDevice, ControlRectangle.Width, ControlRectangle.Height);
        }


        #region Special Event Handler
        void EventInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (focus)
            {
                    //value += System.Windows.Forms.Clipboard.GetText();
                if (e.KeyCode == Keys.Back)
                {
                    if (value.Length != 0)
                        value = value.Remove(value.Length - 1);
                    DownCursor();
                }
                //if (e.KeyCode == Keys.Left)
                //    DownCursor();
                //if (e.KeyCode == Keys.Right)
                //    UpCursor();
            }
        }

        void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (focus)
            {
                if (e.Character != '\b' && e.Character != 27 && e.Character != 17 && e.Character != 22 && e.Character != 13)
                    AddCharacter(e.Character);
                //if (e.Character == 13)
                //{
                //    AddCharacter(e.Character);
                //    GetCursorPosition();
                //    NewLine();
                //}

            }
        }
        #endregion


        public void HandleInput(InputState inputState)
        {
            if (Enabled)
            {
                if (ControlRectangle.Contains(inputState.CursorPosition))
                {
                    if (inputState.CurrentMouseState.LeftButton == ButtonState.Pressed &&
                        inputState.LastMouseState.LeftButton == ButtonState.Released)
                    {
                        focus = true;
                        CursorPosition.Y = ControlRectangle.Y;
                    }
                }
                else if (inputState.CurrentMouseState.LeftButton == ButtonState.Pressed &&
                        inputState.LastMouseState.LeftButton == ButtonState.Released)
                {
                    focus = false;
                    DrawCursor = false;
                }
                if (inputState.ControlPressed && inputState.IsKeyPressed(Keys.V))
                    foreach (char c in System.Windows.Forms.Clipboard.GetText())
                        AddCharacter(c);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (focus)
            {
                delay += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (delay > .5f)
                {
                    DrawCursor = (!DrawCursor) ? true : false;
                    delay = 0f;
                }
                if (String.IsNullOrEmpty(value))
                {
                    CursorPosition.X = ControlRectangle.X;
                    view_X = 0;
                }
            }
            else
                delay = 0;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            try
            {
                GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
                GraphicsDevice.SetRenderTarget(TextRenderTarget);
                GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                if(!LineWrap)
                    spriteBatch.DrawString(defaultFont, value, new Vector2(view_X, 0), Color.White);
                else
                {
                    string[] wrappedValue = value.Split('\r');
                    for(int i = 0; i < wrappedValue.Length; i++)
                    {
                        spriteBatch.DrawString(defaultFont, wrappedValue[i], new Vector2(view_X, (DefaultHeight * i)), Color.White);
                    }
                }
                spriteBatch.End();

                GraphicsDevice.SetRenderTarget(null);
                spriteBatch.Begin();
                // Draw the box container
                if (Enabled)
                    spriteBatch.Draw(whiteTexture, ControlRectangle, Color.Black * .8f);
                else
                    spriteBatch.Draw(whiteTexture, ControlRectangle, new Color(48, 48, 48) * .8f);

                DrawBorders(spriteBatch, BorderColor);

                // Draw the cursor
                if (DrawCursor)
                    spriteBatch.Draw(whiteTexture, new Rectangle((int)CursorPosition.X+1, (int)CursorPosition.Y, CursorDimensions.Width, CursorDimensions.Height), Color.White);

                // Draw the rendertarget containing text.
                spriteBatch.Draw(TextRenderTarget, new Vector2(ControlRectangle.X, ControlRectangle.Y), new Rectangle(0, 0, ControlRectangle.Width, ControlRectangle.Height), Color.White);
                spriteBatch.End();
            }
            catch
            {
                if (!String.IsNullOrEmpty(value))
                    value = value.Remove(value.Length - 1);
            }
        }

        public void DrawBorders(SpriteBatch spriteBatch, Color borderColor)
        {
            spriteBatch.Draw(whiteTexture, new Rectangle(ControlRectangle.Left, ControlRectangle.Top, 1, ControlRectangle.Height), borderColor);
            spriteBatch.Draw(whiteTexture, new Rectangle(ControlRectangle.Left, ControlRectangle.Top, ControlRectangle.Width, 1), borderColor);
            spriteBatch.Draw(whiteTexture, new Rectangle(ControlRectangle.Right, ControlRectangle.Top, 1, ControlRectangle.Height), borderColor);
            spriteBatch.Draw(whiteTexture, new Rectangle(ControlRectangle.Left, ControlRectangle.Bottom, ControlRectangle.Width, 1), borderColor);
        }

        #region Public Methods

        public void AddCharacter(char character)
        {
            //value = value.Insert((int)GetCursorPosition().X, character.ToString());
            value += character;

            // Handle visual character placement.
            string newValue = "";
            foreach (char c in value)
                newValue += CHAR_TO_MEASURE;
            try
            {

                if (!LineWrap)
                {
                    if (defaultFont.MeasureString(newValue).X >= ControlRectangle.Width)
                        view_X -= defaultFont.MeasureString(CHAR_TO_MEASURE).X;
                }
                else
                {
                    if (defaultFont.MeasureString(newValue).X >= ControlRectangle.Width)
                    {
                        GetCursorPosition();
                        NewLine();
                    }
                }
            }
            catch
            {
                value = value.Remove(value.Length - 1);
            }
            UpCursor();
        }

        public void NewLine()
        {
            lineIndex++;
            SetBounds(ControlRectangle.Width, DefaultHeight * lineIndex);
            //SetCursorPosition(0, lineIndex-1);
            //CursorPosition.Y;
        }

        private void UpCursor()
        {
            //string lastChar = Convert.ToString(value[value.Length - 1]);
            float newCursorPositionX = 0;
            try
            {
                //char[] charArray = value.ToCharArray();
                 newCursorPositionX = CursorPosition.X + defaultFont.MeasureString(CHAR_TO_MEASURE).X ;
                //if(CursorPosition.X > )

                if (newCursorPositionX - ControlRectangle.X < ControlRectangle.Width)
                    CursorPosition.X = newCursorPositionX;
            }
            catch
            {
                //If character cannot be handled just delete it
                value = value.Remove(value.Length - 1);
            }

        }

        public void DownCursor()
        {
            //float x = defaultFont.MeasureString(CHAR_TO_MEASURE).X;
            //if (CursorPosition.X <= ControlRectangle.X && view_X < 0)
            //    view_X += x;
            //CursorPosition.X -= x;

            if (CursorPosition.X <= ControlRectangle.X)
            {
                CursorPosition.X = ControlRectangle.X;
                if (value.Length != 0)
                    view_X += defaultFont.MeasureString(CHAR_TO_MEASURE).X;
                else
                    view_X = 0;
            }
            else
            {
                if (value.Length != 0)
                {
                    float x = defaultFont.MeasureString(CHAR_TO_MEASURE).X;
                    if (view_X != 0)
                        view_X += defaultFont.MeasureString(CHAR_TO_MEASURE).X;
                    else
                    {
                        view_X = 0;
                        CursorPosition.X -= x;
                    }
                }
            }
        }

        public void SetCursorPosition(int X, int Y)
        {
            var charWidth = defaultFont.MeasureString(CHAR_TO_MEASURE).X;
            CursorPosition.X = ControlRectangle.X + (X * charWidth);
            CursorPosition.Y = ControlRectangle.Y + (Y * DefaultHeight);
        }
        /// <summary>
        /// This method will get the currrent cursor position in characters.
        /// </summary>
        /// <returns>The cursor position in the string</returns>
        public Vector2 GetCursorPosition()
        {
            var charWidth = defaultFont.MeasureString(CHAR_TO_MEASURE).X;
            Vector2 CharCursorPosition = new Vector2((int)((CursorPosition.X - (ControlRectangle.X + view_X)) / charWidth), (CursorPosition.Y - ControlRectangle.Y) / DefaultHeight);
            return CharCursorPosition;
        }

        public void SetPosition(int X, int Y)
        {
            ControlRectangle.X = X;
            ControlRectangle.Y = Y;
            CursorPosition.Y = ControlRectangle.Y;
        }
        public void SetBounds(int Width, int Height)
        {
            if (ControlRectangle.Width != Width || ControlRectangle.Height != Height)
            {
                ControlRectangle.Width = Width;
                ControlRectangle.Height = Height;
                TextRenderTarget.Dispose();
                TextRenderTarget = new RenderTarget2D(GraphicsDevice, Width, Height);
            }
        }

        #endregion
    }
}
