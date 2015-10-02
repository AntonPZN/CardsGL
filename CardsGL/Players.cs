using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardsGL
{
    public enum Alignment
    {
        Bottom = 0,
        Lift,
        Top,
        Right


    }

    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public bool Status { get; set; }
        public int CurrentBet { get; set; }
        public int MaxBet { get; set; }
        public Alignment Alignment { get; set; }
        public List<Card> CardDeck { get; set; }
        public Difficulty Difficulty { get; set; }

        public Player()
        {
            Name = "";
            Score = 0;
            Status = false;
            CurrentBet = 0;
            MaxBet = 0;
            CardDeck = new List<Card>();
            Difficulty = CardsGL.Difficulty.Easy;
        }

        public Player(string aName, Alignment alignment)
        {
            Name = aName;
            Score = 0;
            Status = false;
            CurrentBet = 0;
            MaxBet = 0;
            Alignment = alignment;
            CardDeck = new List<Card>();
            Difficulty = CardsGL.Difficulty.Easy;
        }

        public void SetupCardPosition(int width, int height)
        {
            this.CardDeck.Sort();

            float x = 0, y = 0, xMax = 0, yMax = 0, xDelta = 0, yDelta = 0, depth = 0.8f;

            int i = 0, j = 0, rows = 0;

            rows = this.CardDeck.Count % Game1.MAX_CARDS_IN_ROW == 0 ? this.CardDeck.Count / Game1.MAX_CARDS_IN_ROW - 1 : this.CardDeck.Count / Game1.MAX_CARDS_IN_ROW;

            if (this.Alignment == CardsGL.Alignment.Bottom)
            {
                xDelta = Game1.CARD_WIDTH / 2;
                yDelta = Game1.CARD_HEIGHT / 2;
            }
            else
            {
                xDelta = Game1.CARD_WIDTH / 4;
                yDelta = Game1.CARD_HEIGHT / 4;
            }

            xMax = (width - Game1.CARD_WIDTH - (Game1.MAX_CARDS_IN_ROW - 1) * xDelta) / 2;
            yMax = (height - Game1.CARD_HEIGHT - (Game1.MAX_CARDS_IN_ROW - 1) * yDelta) / 2;

            switch (this.Alignment)
            {
                case Alignment.Bottom:  x = (width - Game1.CARD_WIDTH - (this.CardDeck.Count - rows * Game1.MAX_CARDS_IN_ROW - 1) * xDelta) / 2;
                                        y = height - Game1.CARD_HEIGHT - Game1.BORDER_SIZE - yDelta * rows;

                                        foreach (Card item in this.CardDeck)
                                                {
                                                    if (j == rows)
                                                        item.PositionX = x + xDelta * (i % Game1.MAX_CARDS_IN_ROW);
                                                    else
                                                        item.PositionX = xMax + xDelta * (i % Game1.MAX_CARDS_IN_ROW);

                                                    item.PositionY = y + yDelta * j;
                                                    i++;

                                                    if (i % Game1.MAX_CARDS_IN_ROW == 0)
                                                       j++;

                                                    item.Rotation = 0f;
                                                    depth -= 0.01f;
                                                    item.Depth = depth;

                                                }  break;

                case Alignment.Lift:    x = Game1.BORDER_SIZE;
                                        y = (height - Game1.CARD_HEIGHT - (this.CardDeck.Count - rows * Game1.MAX_CARDS_IN_ROW - 1) * yDelta) / 2;
                                        
                                        foreach (Card item in this.CardDeck)
                                                {
                                                    item.PositionX = x + xDelta * j;

                                                    if (j == rows)
                                                        item.PositionY = y + yDelta * (i % Game1.MAX_CARDS_IN_ROW);
                                                    else
                                                        item.PositionY = yMax + yDelta * (i % Game1.MAX_CARDS_IN_ROW);

                                                    i++;

                                                    if (i % Game1.MAX_CARDS_IN_ROW == 0)
                                                        j++;

                                                    item.Rotation = 0f;
                                                    depth -= 0.01f;
                                                    item.Depth = depth;

                                                } break;

                case Alignment.Right:   x = width - Game1.CARD_WIDTH - Game1.BORDER_SIZE - xDelta * rows;
                                        y = (height - Game1.CARD_HEIGHT - (this.CardDeck.Count - rows * Game1.MAX_CARDS_IN_ROW - 1) * yDelta) / 2;
                                        
                                        foreach (Card item in this.CardDeck)
                                                {
                                                    item.PositionX = x + xDelta * j;

                                                    if (j == rows)
                                                        item.PositionY = y + yDelta * (i % Game1.MAX_CARDS_IN_ROW);
                                                    else
                                                        item.PositionY = yMax + yDelta * (i % Game1.MAX_CARDS_IN_ROW);

                                                    i++;

                                                    if (i % Game1.MAX_CARDS_IN_ROW == 0)
                                                        j++;

                                                    item.Rotation = 0f;
                                                    depth -= 0.01f;
                                                    item.Depth = depth;

                                                } break;

                case Alignment.Top:     x = (width - Game1.CARD_WIDTH - (this.CardDeck.Count - rows * Game1.MAX_CARDS_IN_ROW - 1) * xDelta) / 2;
                                        y = Game1.BORDER_SIZE;
                                        foreach (Card item in this.CardDeck)
                                                {
                                                    if (j == rows)
                                                        item.PositionX = x + xDelta * (i % Game1.MAX_CARDS_IN_ROW);
                                                    else
                                                        item.PositionX = xMax + xDelta * (i % Game1.MAX_CARDS_IN_ROW);

                                                    item.PositionY = y + yDelta * j;
                                                    i++;

                                                    if (i % Game1.MAX_CARDS_IN_ROW == 0)
                                                        j++;

                                                    item.Rotation = 0f;
                                                    depth -= 0.01f;
                                                    item.Depth = depth;

                                                } break;
                default: break;
            }
        }

        public Card MakeStep()
        {
            Card temp = new Card();

            foreach (Card item in this.CardDeck)
	            {
		            if (item.Current == true)
	                    {
                            item.Current = false;
		                    temp = item;
	                    }
	            }

            this.CardDeck.Remove(temp);

            return temp;
        }

        public Card GetCard(List<Card> deck)
        {
            Card temp = new Card();
            Random rand = new Random();

            if (this.CardDeck.Count > 0)
            {
                if (this.Difficulty == CardsGL.Difficulty.Easy)
                {
                    if (deck.Count == 0)
                    {
                        temp = this.CardDeck[rand.Next(this.CardDeck.Count)];
                    }
                    else
                    {
                        foreach (Card item in deck)
                        {
                            foreach (Card card in this.CardDeck)
                            {
                                if (card.CardValue == item.CardValue)
                                {
                                    temp = card;
                                    break;
                                }
                            }
                        }
                    }
                    
                }
                this.CardDeck.Remove(temp);

            }

            return temp;
        }

        public Card BeatCard(Card card, CardColor trump)
        {
            Card temp = new Card();

            foreach (Card item in this.CardDeck)
            {
                if (card.CardColor == item.CardColor && card.CardValue < item.CardValue)
                {
                    temp = item;
                    this.CardDeck.Remove(temp);
                    break;
                }
            }

            if (temp.Empty == true)
            {
                foreach (Card item in this.CardDeck)
                {
                    if (item.CardColor == trump && card.CardColor != trump)
                    {
                        temp = item;
                        this.CardDeck.Remove(temp);
                        break;
                    }
                }
            }

            return temp;
        }

        public void TakeDeck(List<Card> deck)
        {
            foreach (Card item in deck)
            {
                this.CardDeck.Add(item);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, bool showCards)
        {
            foreach (Card item in this.CardDeck)
            {
                item.Draw(spriteBatch, texture, showCards);
            }
        }
    }
}
