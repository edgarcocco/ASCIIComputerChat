using ASCIIArtLibrary;
using ASCIIComputerChat.GameObjects.Controls;
using ASCIIComputerChat.Scripts;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ASCIIComputerChat.GameObjects
{
    class ComputerChat : IGameObject
    {
        Game1 game;
        Viewport viewport;
        GraphicsDevice graphicsDevice;
        ASCIIArt computerArt;

        TextInput computerMainTextInput;
        TextArea computerMainTextArea;
        Vector2 computerStartingPosition;
        Vector2 computerStartingPositionOffset = new Vector2(-150f, -165f);

        public string LastCommand = String.Empty;

        public Vector2 Position { get; set; }
        public Dimensions Dimensions
        {
            get
            {
                var bounds = computerArt.GetArtBounds(ASCIIArtExtensionMethods.ASCIIArtFont);
                return new Dimensions(bounds.Width, bounds.Height);
            }
        }

        public bool SkipDraw { get; set; }

        public ComputerChat(Game game)
        {
            this.game = game as Game1;
            graphicsDevice = game.GraphicsDevice;
            viewport = graphicsDevice.Viewport;
            SkipDraw = false;
        }
        public ComputerChat(Vector2 position, Game game)
        {
            this.game = game as Game1;
            Position = position;
            graphicsDevice = game.GraphicsDevice;
            viewport = graphicsDevice.Viewport;
            SkipDraw = false;

        }

        public void Initialize()
        {
            computerMainTextInput = new TextInput(game, Vector2.Zero, 286, 20);
            computerMainTextArea = new TextArea(game, Vector2.Zero, new Dimensions(286, 165));
            
        }

        public void LoadContent(ContentManager content)
        {
            computerArt = content.Load<ASCIIArt>("computer");

            computerMainTextInput.LoadContent(content);
            computerMainTextInput.BorderColor = Color.Black;
            computerMainTextInput.CursorDimensions = new Dimensions(1, 17);
            computerMainTextInput.Focus = true;
            computerMainTextArea.LoadContent(content);

            computerMainTextArea.AddText("# Welcome !");
            computerMainTextArea.AddText("# You can start by typing 'help'");
            computerMainTextArea.AddText("# To see what commands are available");

            game.netClientManager.RegisterMessagesCallback(new SendOrPostCallback(GotMessage));
        }

        public void HandleInput(InputState inputState)
        {
            computerMainTextInput.HandleInput(inputState);

            if(inputState.IsKeyPressed(Keys.Enter) && 
               (game.netClientManager.ConnectionStatus == NetConnectionStatus.None ||
               game.netClientManager.ConnectionStatus == NetConnectionStatus.Disconnected))
            {
                computerMainTextArea.AddText("# " + computerMainTextInput.Value);
                computerMainTextArea.AddText("# " + CommandProcessor.ExecuteCommand(computerMainTextInput.Value));
                LastCommand = computerMainTextInput.Value;
                computerMainTextInput.Value = String.Empty;
            }

            if (inputState.IsKeyPressed(Keys.Enter) &&
               game.netClientManager.ConnectionStatus == NetConnectionStatus.Connected)
            {
                game.netClientManager.SendMessage(computerMainTextInput.Value);
                computerMainTextInput.Value = String.Empty;
            }

            if (inputState.IsKeyPressed(Keys.Up) && !inputState.IsKeyPressed(Keys.LeftControl))
            {
                if (computerMainTextInput.Value != LastCommand)
                    computerMainTextInput.Value = LastCommand;
            }
            computerMainTextArea.HandleInput(inputState);
        }

        public void Update(GameTime gameTime)
        {
            computerMainTextInput.Update(gameTime);
            computerStartingPosition = new Vector2(((Position.X + computerStartingPositionOffset.X) + Dimensions.Width / 2), ((Position.Y + computerStartingPositionOffset.Y) + Dimensions.Height / 2));
            computerMainTextInput.SetPosition((int)computerStartingPosition.X, (int)computerStartingPosition.Y + computerMainTextArea.Dimensions.Height);
            computerMainTextArea.SetPosition(computerStartingPosition);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(computerArt, Position, Color.Green);
            spriteBatch.End();
            computerMainTextInput.Draw(spriteBatch, gameTime);
            computerMainTextArea.Draw(spriteBatch, gameTime);
        }

        public void GotMessage(object peer)
        {
			NetIncomingMessage im;
			while ((im = game.netClientManager.NetClient.ReadMessage()) != null)
			{
				// handle incoming message
				switch (im.MessageType)
				{
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.ErrorMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.VerboseDebugMessage:
						string text = im.ReadString();
                        System.Windows.Forms.MessageBox.Show(text);
                        break;
					case NetIncomingMessageType.StatusChanged:
						NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();

                        if (status == NetConnectionStatus.Connected)
                            computerMainTextArea.AddText("Connection established!");
						//if (status == NetConnectionStatus.Disconnected)
						//	s_form.button2.Text = "Connect";

						//string reason = im.ReadString();
						//Output(status.ToString() + ": " + reason);

						break;
					case NetIncomingMessageType.Data:
						string chat = im.ReadString();
                        computerMainTextArea.AddText(chat);
						break;
					default:
                        System.Windows.Forms.MessageBox.Show("Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes");
						break;
				}
				game.netClientManager.NetClient.Recycle(im);
			}
        }
        
    }
}
