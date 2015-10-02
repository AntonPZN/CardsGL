using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CardsGL
{
    public enum CardColor
    {
        Clubs = 0,
        Spades,
        Diamonds     ,
        Hearts
        
        
    }

    public enum CardValue
    {
        Two = 0,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,  
        Ten,
        Jack,
        Queen,
        King,
        Ace

    }

    public class Card : Sprite, IEquatable<Card>, IComparable<Card>
    {
        private bool empty;

        public CardColor CardColor { get; set; }
        public CardValue CardValue { get; set; }
        public bool Current { get; set; }
        public bool Empty { get { return empty; } }

        public Rectangle GetRect { get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); } }

        public Card(CardGame game)
        {
            this.Game = game;
            empty = true;
        }

        public Card(CardGame game, CardColor aCardColor, CardValue aCardValue)
        {
            this.Game = game;

            CardColor = aCardColor;
            CardValue = aCardValue;
            empty = false;
            Depth = 1 - ((int)this.CardColor * this.Game.NUMBER_OF_CARDS + (int)this.CardValue + 1) / 100f;
            Width = 60;
            Height = 83;
        }

        public Card(CardColor aCardColor, CardValue aCardValue, Vector2 vect)
        {
            CardColor = aCardColor;
            CardValue = aCardValue;
            empty = false;
            Depth = 1 - ((int)this.CardColor * this.Game.NUMBER_OF_CARDS + (int)this.CardValue + 1) / 100f;
            Position = vect;
            Width = 60;
            Height = 83;
        }

        public Card(CardColor aCardColor, CardValue aCardValue, float x, float y)
        {
            CardColor = aCardColor;
            CardValue = aCardValue;
            empty = false;
            Depth = 1 - ((int)this.CardColor * this.Game.NUMBER_OF_CARDS + (int)this.CardValue + 1) / 100f;
            Position = new Vector2(x, y);
            Width = 60;
            Height = 83;
        }

        public override string ToString()
        {
            return Empty ? "" : CardColor.ToString() + "  " + CardValue.ToString();
        }

        //public int GetCardValue()
        //{
        //    int value = 0;
        //    switch (CardValue)
        //    {
        //        case CardValue.Ace: value = 11; break;
        //        case CardValue.Ten: value = 10; break;
        //        case CardValue.King: value = 4; break;
        //        case CardValue.Queen: value = 3; break;
        //        case CardValue.Jack: value = 2; break;

        //        default: value = 0; break;
        //    }

        //    return value;
        //}

        public override int GetHashCode()
        {
            return (int)CardValue;
        }

        public bool Equals(Card other)
        {
            if (this.CardColor == other.CardColor && this.CardValue == other.CardValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int CompareTo(Card compareCard)
        {
            if ((int)this.CardColor * this.Game.NUMBER_OF_CARDS + (int)this.CardValue <
                (int)compareCard.CardColor * this.Game.NUMBER_OF_CARDS + (int)compareCard.CardValue)
                return -1;
            else
                return 1;
        }

        //public int CompareToColor(Card compareCard)
        //{
        //    if (compareCard == null)
        //        return 1;

        //    else
        //        return this.CardColor.CompareTo(compareCard.CardColor);
        //}

        //public int CompareToValue(Card compareCard)
        //{
        //    if (compareCard == null)
        //        return 1;

        //    else
        //        return this.CardValue.CompareTo(compareCard.CardValue);
        //}

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, bool showCards)
        {
            SpriteEffects effects = SpriteEffects.None;
            //var origin = new Vector2(this.Width / 2, this.Height / 2);
            Vector2 origin = new Vector2();
            Vector2 p   = new Vector2();
            Rectangle currentRectangle;

            if (this.Rotation != 0)
            {
                origin = new Vector2(this.Width / 2, this.Height / 2);
                p = new Vector2(this.Position.X + this.Width, this.Position.Y + this.Height / 2);
            }
            else
            {
                origin = new Vector2(0, 0);
                p = this.Position;
            }

            if (showCards)
            {
                currentRectangle = new Rectangle((int)this.CardValue * (this.Width + 1), (int)this.CardColor * (this.Height + 1), this.Width, this.Height);
            }
            else
            {
                currentRectangle = new Rectangle((this.Width + 1) * 2, 0, this.Width, this.Height);
            }

            if (this.Current)
            {
                p.Y -= 10;
            }

            spriteBatch.Draw(texture, p, currentRectangle, Color.White, Rotation, origin, 1f, effects, this.Depth);
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, float rotation)
        {
            var effects = SpriteEffects.None;
            var origin = new Vector2(this.Width / 2, this.Height / 2);
            //var origin = new Vector2(0, 0);
            var p = new Vector2(this.Position.X + this.Width * 2 / 3, this.Position.Y + this.Height * 2 / 3);
            Rectangle currentRectangle = new Rectangle((int)this.CardValue * (this.Width + 1), (int)this.CardColor * (this.Height + 1), this.Width, this.Height);

            spriteBatch.Draw(texture, p, currentRectangle, Color.White, rotation, origin, 1f, effects, this.Depth);
        }
    }
}
