using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardsGL
{
    abstract public class Sprite
    {
        private Vector2 position;

        public Game1 Game { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Depth { get; set; }
        public float Rotation { get; set; }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float PositionX
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float PositionY
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
    }
}
