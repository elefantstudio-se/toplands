using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace fpsrpg
{
    class Bullet2
    {
        bool return_value;
        int damage = 10;

        Matrix BulletTransform;
        Rectangle bulletRectangle;
        Color[] bulletTextureData;

        Texture2D texture;

        Vector2 position;
        Vector2 center;
        Vector2 velocity;

        float rotation;
        float scale;

        bool alive;

        int index;

        public List<Bullet2> bullets = new List<Bullet2>();

        public Bullet2(Texture2D texture)
        {
            this.texture = texture;

            position = Vector2.Zero;
            center = new Vector2(texture.Width /2, texture.Height /2);
            velocity = Vector2.Zero;

            Rotation = 0.0f;
            Scale = 0.5f;

            alive = false;

            index = 0;
        }

        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public Rectangle BulletRectangle
        {
            get { return bulletRectangle; }
            set { bulletRectangle = value; }
        }

        public Color[] BulletTextureData
        {
            get { return bulletTextureData; }
            set { bulletTextureData = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Center
        {
            get { return center; }
        }

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set 
            { 
                rotation = value;
                if (rotation < -MathHelper.TwoPi)
                    rotation = MathHelper.TwoPi;
                if (rotation > MathHelper.TwoPi)
                    rotation = -MathHelper.TwoPi;
            }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public bool Alive
        {
            get { return alive; }
        }

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public void Create()
        {
            alive = true;
        }

        public void Kill()
        {
            alive = false;
        }

        public bool UpdateBullets(Sprite EnemyToCheckCollisionWith, Rectangle EnemyCollisionRectangle, Color[] texturedata, Matrix enemytransform,int TheScreenWidth,int TheScreenHeight)
        {
            foreach (Bullet2 b in bullets)
            {
                b.Position += b.Velocity;
                if (b.Position.X < 0)
                    b.Kill();
                else if (b.Position.Y < 0)
                    b.Kill();
                else if (b.Position.X > TheScreenWidth)
                    b.Kill();
                else if (b.Position.Y > TheScreenHeight)
                    b.Kill();
            }

            for (int i = 0; i < bullets.Count; i++)
            {
                return_value = false;
                // Build the block's transform
                BulletTransform =
                    Matrix.CreateTranslation(new Vector3(-new Vector2(Texture.Width, Texture.Height / 2), 0.0f)) *
                    Matrix.CreateRotationZ(bullets[i].Rotation) *
                    Matrix.CreateTranslation(new Vector3(bullets[i].Position, 0.0f));

                // Calculate the bounding rectangle of this block in world space
                BulletRectangle = Sprite.CalculateBoundingRectangle(new Rectangle(0, 0, Texture.Width, Texture.Height), BulletTransform);

                // The per-pixel check is expensive, so check the bounding rectangles
                // first to prevent testing pixels when collisions are impossible.
                if (EnemyCollisionRectangle.Intersects(BulletRectangle))
                {
                    // Check collision with person
                    if (Collision.IntersectPixels(
                        enemytransform,
                        EnemyToCheckCollisionWith.Size.Width,
                        EnemyToCheckCollisionWith.Size.Height,
                        texturedata,
                        BulletTransform,
                        Texture.Width,
                        Texture.Height,
                        BulletTextureData))
                    {
                        return_value = true;
                        bullets[i].Kill();
                    }
               }
                if (!bullets[i].Alive)
                {
                    bullets.RemoveAt(i);
                    i--;
                }
            }
            return return_value;
        }

        public void FireBullet(Player PlayerThatShoots, SoundBank TheSoundBank)
        {
            Bullet2 newBullet = new Bullet2(Texture);

            newBullet.Velocity = new Vector2((float)Math.Cos(PlayerThatShoots.Rotation), (float)Math.Sin(PlayerThatShoots.Rotation)) * 100.0f;

            newBullet.Position = PlayerThatShoots.Position + newBullet.Velocity * 1.75f;
            newBullet.Rotation = PlayerThatShoots.Rotation;
            newBullet.Scale = 1.0f;
            newBullet.Create();
            TheSoundBank.PlayCue("MP5-1");
            bullets.Add(newBullet);

        }

    }
}
