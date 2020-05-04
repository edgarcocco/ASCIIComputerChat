using ASCIIArtLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using ASCIIComputerChat.Scripts;
using ASCIIComputerChat.GameObjects;
using Lidgren.Network;
using System;

namespace ASCIIComputerChat
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Viewport viewport;

        InputState inputState;
        List<IGameObject> gameObjects = new List<IGameObject>();
        public NetClientManager netClientManager = new NetClientManager();
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            viewport = GraphicsDevice.Viewport;
            inputState = new InputState(this);
            base.Initialize();
        }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ASCIIArtExtensionMethods.ASCIIArtFont = Content.Load<SpriteFont>("Consolas");
            ComputerChat computerChat = new ComputerChat(this);
            computerChat.Initialize();
            computerChat.LoadContent(Content);
            computerChat.Position = new Vector2((viewport.Width / 2) - (computerChat.Dimensions.Width / 2),
                                                (viewport.Height / 2) - (computerChat.Dimensions.Height / 2));


            CommandProcessor.Initialize(this);
            gameObjects.Add(computerChat);
        }


        protected override void UnloadContent()
        {
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            inputState.Update();
            foreach (IGameObject gameObject in gameObjects)
                gameObject.HandleInput(inputState);

            foreach (IGameObject gameObject in gameObjects)
                gameObject.Update(gameTime);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            foreach (IGameObject gameObject in gameObjects)
                if (!gameObject.SkipDraw)
                    gameObject.Draw(spriteBatch, gameTime);

            base.Draw(gameTime);
        }
        
    }
}
