using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

using System.Collections.Generic;

namespace CardsGL
{
    public enum MainMenuItems
    {
        Resume = 0,
        New_game,
        Options,
        Exit
    }

    public class MenuItem : Sprite, IEquatable<MenuItem>
    {
        public string Name { get; set; }
        public MainMenuItems Type { get; set; }
        public Color ItemColor { get; set; }
        public bool Enable { get; set; }
        public bool Selected { get; set; }

        public Rectangle GetRect { get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); } }

        public MenuItem(CardGame game, MainMenuItems type)
        {
            this.Name = game.mainMenuItemNamesEng[(int)type];
            this.Type = type;
            this.ItemColor = Color.Red;
            this.Enable = false;
            this.Selected = false;
            this.Width = this.Name.Length * 72;
            this.Height = 88;
        }

        public MenuItem(string name, bool enable)
        {
            this.Name = name;
            this.ItemColor = Color.Red;
            this.Enable = enable;
            this.Selected = false;
            this.Width = name.Length * 72;
            this.Height = 88;
        }

        public bool Equals(MenuItem other)
        {
            if (this.Name == other.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (int)Name.Length;
        }
    }

    public class Menu : Sprite, IEquatable<Menu>
    {
        public string Name { get; set; }
        public List<MenuItem> Items { get; set; }

        public Menu(CardGame game)
        {
            this.Game = game;

            Items = new List<MenuItem>();

            for (int count = 0; count < 4; count++)
            {
                Items.Add(new MenuItem(game, (MainMenuItems)count));
            }

            int maxWidth = 0, i = 0;

            foreach (MenuItem item in Items)
            {
                maxWidth = item.Width > maxWidth ? item.Width : maxWidth;
            }

            foreach (MenuItem item in Items)
            {
                i++;
                item.PositionX = 400 - maxWidth / 2;
                item.PositionY = item.Height * i;
            }

            this.Width = maxWidth;
            this.Height = this.Items.Count * this.Items[0].Height;
        }

        public bool Equals(Menu other)
        {
            if (this.Name == other.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (int)Name.Length;
        }

        public void GetSelectedItem(MouseState ms)
        {
            foreach (MenuItem item in Items)
            {
                if (item.Enable == false)
                    continue;

                if (item.GetRect.Contains(ms.Position))
                {
                    item.Selected = true;
                }
                else
                {
                    item.Selected = false;
                }
            }

            Update();
        }

        public int MenuItemClick()
        {
            int selected = -1;

            foreach (MenuItem item in Items)
            {
                if (item.Selected)
                {
                    switch (item.Name)
                    {
                        case "Resume": selected = 0; break;
                        case "New Game": selected = 1; break;
                        case "Exit": selected = 3; break;
                        default: break;
                    }

                    break;
                }
            }

            return selected;
        }

        public void Update()
        {
            foreach (MenuItem item in Items)
            {
                if (item.Type != MainMenuItems.Options)
                {
                    if (item.Type == MainMenuItems.Resume && this.Game.State == GameStates.MainMenu && this.Game.PrevGameState == GameStates.MainMenu)
                    {
                        item.Enable = false;
                    }
                    else
                    {
                        item.Enable = true;
                    }

                }
                
            }

            foreach (MenuItem item in Items)
            {
                if (item.Enable == true)
                {
                    if (item.Selected == true)
                    {
                        item.ItemColor = Color.Green;
                    }
                    else
                    {
                        item.ItemColor = Color.Red;
                    }
                }
                else
                {
                    item.ItemColor = Color.Gray;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, SpriteFont font)
        {
            var effects = SpriteEffects.None;
            var origin = new Vector2(0, 0);
            var p = new Vector2(0, 0);
            Rectangle currentRectangle = new Rectangle(0, 0, 800, 480);

            spriteBatch.Draw(texture, p, currentRectangle, new Color(Color.Black, 170), 0f, origin, 1f, effects, 0.1f);
            
            foreach (MenuItem item in Items)
            {
                spriteBatch.DrawString(font, item.Name, item.Position, item.ItemColor, 0f, origin, 1f, effects, 0f);
            }

        }
    }
}
