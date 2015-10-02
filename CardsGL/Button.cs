using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

using System.Collections.Generic;

namespace CardsGL
{
    public class Button : Sprite
    {
        public int Name { get; set; }
        public Rectangle GetRect { get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); } }
        public bool Enabled { get; set; }
        public bool Selected { get; set; }
        public Color ButtonColor { get; set; }
        public float Scale { get; set; }

        public Button(CardGame game, int x, int y, int width, int height)
        {
            this.Position = new Vector2(x, y);
            this.Scale = 1f;
            this.Width = width;
            this.Height = height;
            this.Depth = 0.2f;
            this.ButtonColor = Color.Gray;

            this.Game = game;
        }

        public void GetSelected(MouseState ms)
        {
            if (this.Enabled == true)
            {
                if (this.GetRect.Contains(ms.Position))
                    this.Selected = true;
                else
                    this.Selected = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            SpriteEffects effects = SpriteEffects.None;
            Vector2 origin = new Vector2(0, 0);
            Rectangle currentRectangle = new Rectangle(0, 0, this.Width, this.Height);
            spriteBatch.Draw(texture, this.Position, currentRectangle, this.ButtonColor, 0f, origin, this.Scale, effects, this.Depth);

        }

        public void Update()
        {
            if (this.Enabled == true)
            {
                if (this.Selected)
                    this.ButtonColor = Color.Green;
                else
                    this.ButtonColor = Color.Black;
            }
            else
            {
                this.ButtonColor = Color.Gray;
            }
        }
    }
}
