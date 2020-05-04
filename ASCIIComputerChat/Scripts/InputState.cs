#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;
using System.Threading;
#endregion

namespace ASCIIComputerChat.Scripts
{
    /// <summary>
    /// Helper for reading input from keyboard, gamepad, and touch input. This class 
    /// tracks both the current and previous state of the input devices, and implements 
    /// query methods for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region Fields
        public Game Game;
        public const int MaxInputs = 4;

        public MouseState CurrentMouseState;
        public KeyboardState CurrentKeyboardStates;
        
        public MouseState LastMouseState;
        public KeyboardState LastKeyboardStates;

        public TouchCollection TouchState;

        public readonly List<GestureSample> Gestures = new List<GestureSample>();

        public bool ControlPressed { get; set; }

        public Vector2 CursorPosition;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState(Game game)
        {
            Game = game;
            CurrentKeyboardStates = new KeyboardState();
            LastKeyboardStates = new KeyboardState();
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            LastMouseState = CurrentMouseState;

            LastKeyboardStates = CurrentKeyboardStates;

            CurrentKeyboardStates = Keyboard.GetState();

            CurrentMouseState = Mouse.GetState();

            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            foreach (Keys key in keys)
                if (key == Keys.RightControl || key == Keys.LeftControl)
                {
                    ControlPressed = true;
                    break;
                }
                else
                    ControlPressed = false;


            CursorPosition = Mouse.GetState().Position.ToVector2();
            UpdateMousePosition();
            TouchState = TouchPanel.GetState();

            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update. The
        /// controllingPlayer parameter specifies which player to read input for.
        /// If this is null, it will accept input from any player. When a keypress
        /// is detected, the output playerIndex reports which player pressed it.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex? controllingPlayer,
                                            out PlayerIndex playerIndex)
        {
            if (controllingPlayer.HasValue)
            {
                // Read input from the specified player.
                playerIndex = controllingPlayer.Value;

                int i = (int)playerIndex;

                return (CurrentKeyboardStates.IsKeyDown(key) &&
                        LastKeyboardStates.IsKeyUp(key));
            }
            else
            {
                // Accept input from any player.
                return (IsNewKeyPress(key, PlayerIndex.One, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Two, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Three, out playerIndex) ||
                        IsNewKeyPress(key, PlayerIndex.Four, out playerIndex));
            }
        }

        public bool IsKeyPressed(Keys Key)
        {
            if (CurrentKeyboardStates.IsKeyDown(Key) && LastKeyboardStates.IsKeyUp(Key))
                return true;
            return false;
        }

        public bool IsKeyDown(Keys Key)
        {
            if (CurrentKeyboardStates.IsKeyDown(Key))
                return true;
            return false;
        }

        public bool LeftMouseButtonPressed()
        {
            if (CurrentMouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released)
                return true;
            return false;
        }

        public bool RightMouseButtonPressed()
        {
            if (CurrentMouseState.RightButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released)
                return true;
            return false;
        }
        public void UpdateMousePosition()
        {
            CursorPosition = new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
            if (CursorPosition.X >= Game.GraphicsDevice.Viewport.Width)
                CursorPosition.X = Game.GraphicsDevice.Viewport.Width;
            if (CursorPosition.Y >= Game.GraphicsDevice.Viewport.Height)
                CursorPosition.Y = Game.GraphicsDevice.Viewport.Height;
        }

        #endregion
    }
}
