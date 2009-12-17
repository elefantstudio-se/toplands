using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace fpsrpg
{
    public class Player : Sprite
    {
        public ContentManager mContentManager;

        string WIZARD_ASSETNAME;
        const int START_POSITION_X = 125;
        const int START_POSITION_Y = 245;
        const int WIZARD_SPEED = 130;
        const int MOVE_UP = -1;
        const int MOVE_DOWN = 1;
        const int MOVE_LEFT = -1;
        const int MOVE_RIGHT = 1;

        Texture2D Customize_hat_texture;

        public int Customize_hat;
        public int Customize_body = 9;
        public Color Customize_hat_color;
        public Color Customize_body_color;

        enum State
        {
            Walking
        }

        State mCurrentState = State.Walking;

        Vector2 mDirection = Vector2.Zero;
        Vector2 mSpeed = Vector2.Zero;

        KeyboardState mPreviousKeyboardState;
        MouseState mPreviousMouseState;

        public void LoadContent(ContentManager theContentManager)
        {
            mContentManager = theContentManager;
            int HEALTH = 100;

            Position = new Vector2(START_POSITION_X, START_POSITION_Y);
            Customize_body_color = Color.Wheat;
            Customize_hat_color = Color.OliveDrab;

            switch (Customize_hat)
            {
                case 1:
                    Customize_hat_texture = mContentManager.Load<Texture2D>("customize_hat1");
                    break;
                case 2:
                    Customize_hat_texture = mContentManager.Load<Texture2D>("customize_hat2");
                    break;
                case 3:
                    Customize_hat_texture = mContentManager.Load<Texture2D>("customize_hat3");
                    break;
                case 4:
                    Customize_hat_texture = mContentManager.Load<Texture2D>("customize_hat4");
                    break;
                default:
                    Customize_hat_texture = mContentManager.Load<Texture2D>("customize_hat1");
                    break;
            }

            switch (Customize_body)
            {
                case 0:
                    break;
                default:
                    WIZARD_ASSETNAME = "customize_body0";
                    break;
            }

            base.LoadContent(mContentManager, WIZARD_ASSETNAME);
        }

        public void Update(GameTime theGameTime,ContentManager mContentManager)
        {
            KeyboardState aCurrentKeyboardState = Keyboard.GetState();
            MouseState aCurrentMouseState = Mouse.GetState();
            UpdateMovement(aCurrentKeyboardState);

            mPreviousKeyboardState = aCurrentKeyboardState;
            mPreviousMouseState = aCurrentMouseState;

            Customize_hat = Properties.Settings.Default.Customize_hat +1;

            switch (Customize_hat)
            {
                case 1:
                    Customize_hat_texture = mContentManager.Load<Texture2D>("customize_hat1");
                    break;
                case 2:
                    Customize_hat_texture = mContentManager.Load<Texture2D>("customize_hat2");
                    break;
                case 3:
                    Customize_hat_texture = mContentManager.Load<Texture2D>("customize_hat3");
                    break;
                case 4:
                    Customize_hat_texture = mContentManager.Load<Texture2D>("customize_hat4");
                    break;
                default:
                    break;
            }

            base.Update(theGameTime, mSpeed, mDirection);
        }

        public void UpdateMovement(KeyboardState aCurrentKeyboardState)
        {
            if (mCurrentState == State.Walking)
            {
                mSpeed = Vector2.Zero;
                mDirection = Vector2.Zero;

                if (aCurrentKeyboardState.IsKeyDown(Keys.Left) == true)
                {
                    mSpeed.X = WIZARD_SPEED;
                    mDirection.X = MOVE_LEFT;
                }

                else if (aCurrentKeyboardState.IsKeyDown(Keys.Right) == true)
                {
                    mSpeed.X = WIZARD_SPEED;
                    mDirection.X = MOVE_RIGHT;
                }

                if (aCurrentKeyboardState.IsKeyDown(Keys.Up) == true)
                {
                    mSpeed.Y = WIZARD_SPEED;
                    mDirection.Y = MOVE_UP;
                }

                else if (aCurrentKeyboardState.IsKeyDown(Keys.Down) == true)
                {
                    mSpeed.Y = WIZARD_SPEED;
                    mDirection.Y = MOVE_DOWN;
                }
            }
        }

        public override void Draw(SpriteBatch theSpriteBatch, Color OptionalColor)
        {
            if (OptionalColor == null)
                OptionalColor = Color.White;

            //Finally: Draw the body! :)
            base.Draw(theSpriteBatch, Customize_body_color);

            //Draw teh leet hat.
            if (Customize_hat > 0)
                theSpriteBatch.Draw(Customize_hat_texture, new Rectangle((int)Position.X, (int)Position.Y, Customize_hat_texture.Width, Customize_hat_texture.Height), null, Customize_hat_color, Rotation, new Vector2(Customize_hat_texture.Width / 2 + 5, Customize_hat_texture.Height / 2), SpriteEffects.None, 1);
        }
    }
}
