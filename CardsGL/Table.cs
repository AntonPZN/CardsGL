using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CardsGL
{
    class Table : Sprite
    {
        public Player[] Players { get; set; }
        public List<Card> CardDeck { get; set; }
        public List<Card> TableDeck { get; set; }
        public List<Card> OutDeck { get; set; }
        public int NextPlayer { get; set; }
        public int CurrentPlayer { get; set; }

        public CardColor Trump { get; set; }

        public Table(CardGame game, int numberOfPlayers, int width, int height)
            {
                this.Game = game;
                this.CurrentPlayer = 0;
                this.NextPlayer = 1;
                this.Width = width;
                this.Height = height;

                this.Players = new Player[numberOfPlayers];

                switch (numberOfPlayers)
                {
                    case 2: Players[0] = new Player(this.Game, "Vasya", Alignment.Bottom);
                            Players[1] = new Player(this.Game, "Petya", Alignment.Top); 
                            break;
                    case 3: Players[0] = new Player(this.Game, "Vasya", Alignment.Bottom);
                            Players[1] = new Player(this.Game, "Petya", Alignment.Lift);
                            Players[2] = new Player(this.Game, "Fedya", Alignment.Right); 
                            break;
                    case 4: Players[0] = new Player(this.Game, "Vasya", Alignment.Bottom);
                            Players[1] = new Player(this.Game, "Petya", Alignment.Lift);
                            Players[2] = new Player(this.Game, "Fedya", Alignment.Top); 
                            Players[3] = new Player(this.Game, "Grisha", Alignment.Right); break;
                    default: break;
                }

                this.CardDeck = new List<Card>();
                this.TableDeck = new List<Card>();
                this.OutDeck = new List<Card>();
            }

        public void CardSetup()
        {
            CardDeck.Clear();
            TableDeck.Clear();
            OutDeck.Clear();

            foreach (Player item in Players)
            {
                item.CardDeck.RemoveRange(0, item.CardDeck.Count);
            }

            List<Card> deck = new List<Card>();

            for (int i = 0; i < this.Game.CARD_SUIT_COUNT; i++)
            {
                for (int j = 4; j < this.Game.NUMBER_OF_CARDS; j++)
                {
                    deck.Add(new Card(this.Game, (CardColor)i, (CardValue)j));
                }
            }

            Random randomazer = new Random();
            int cardNumber;
            int playerNumber = 0;
            Card currentCard;

            for (int i = this.Game.CARD_SUIT_COUNT * (this.Game.NUMBER_OF_CARDS - 4); 0 < i; i--)
            {
                cardNumber = randomazer.Next(i);

                currentCard = deck[cardNumber];

                if (i <= this.Game.CARDS_IN_HAND * Players.Length)
                {
                    //раздать игроку
                    playerNumber = playerNumber == Players.Length - 1 ? 0 : playerNumber + 1;
                    Players[playerNumber].CardDeck.Add(currentCard);
                }
                else
                {
                    CardDeck.Add(currentCard);
                }

                deck.RemoveRange(cardNumber, 1);
            }

            while (CardDeck[0].CardValue == CardValue.Ace)
            {
                cardNumber = randomazer.Next(CardDeck.Count);
                currentCard = CardDeck[0];
                CardDeck[0] = CardDeck[cardNumber];
                CardDeck[cardNumber] = currentCard;
            }

            foreach (Player item in Players)
            {
                item.CardDeck.Sort();
            }

            int ii = 0;
            foreach (Card item in CardDeck)
            {
                ii++;
                item.PositionX = Width + this.Game.CARD_HEIGHT / 2;
                item.PositionY = Height - this.Game.CARD_WIDTH * 2 + ii;
                item.Rotation = -(float)(Math.PI) / 2;
            }

            CardDeck[0].PositionX = Width + this.Game.CARD_WIDTH + this.Game.BORDER_SIZE;
            CardDeck[0].PositionY = Height - this.Game.CARD_HEIGHT * 2;
            CardDeck[0].Rotation = 0;

            this.Trump = CardDeck[0].CardColor;
        }

        public void ThrowOut()
        {
            foreach (var item in this.TableDeck)
            {
                this.OutDeck.Add(item);
            }
            this.TableDeck.Clear();

            Random rand = new Random();
            int x = (this.Width + this.Game.CARD_WIDTH), y = (this.Game.CARD_HEIGHT - this.Game.BORDER_SIZE * 2);
            Vector2 startPoint;


            foreach (var item in OutDeck)
            {
                startPoint = new Vector2(rand.Next(10, 20), rand.Next(-20, 20));
                item.Position = new Vector2(x - startPoint.X, y + startPoint.Y);
                item.Rotation = -(float)Math.PI / rand.Next(-8, 8);
            }
        }

        public void TakeDeck(int player)
        {
            this.Players[player].TakeDeck(TableDeck);
            this.TableDeck.Clear();

            UpdateTable();
        }

        public int GetBeatCard(MouseState ms)
        {
            float maxDepth = 0.8f;
            List<Card> selectedCards = new List<Card>();

            foreach (Card item in this.Players[0].CardDeck)
            {
                if (item.GetRect.Contains(ms.Position))
                {
                    if (TableDeck.Count != 0)
                    {
                        if (TableDeck[TableDeck.Count - 1].CardValue < item.CardValue && TableDeck[TableDeck.Count - 1].CardColor == item.CardColor)
                        {
                            selectedCards.Add(item);
                        }

                        if (TableDeck[TableDeck.Count - 1].CardColor != Trump && item.CardColor == Trump)
                        {
                            selectedCards.Add(item);
                        }
                    }
                    else
                    {
                        selectedCards.Add(item);
                    }
                }
            }

            foreach (Card item in selectedCards)
            {
                if (item.Depth < maxDepth)
                {
                    maxDepth = item.Depth;
                }
            }

            foreach (Card item in this.Players[0].CardDeck)
            {
                if (item.Depth == maxDepth)
                {
                    item.Current = true;
                }
                else
                {
                    item.Current = false;
                }
            }

            if (IsCardChosen())
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Check mouse click on cards on table
        /// </summary>
        /// <param name="ms">Mouse state.</param>

        public bool ClickOnTableDeck(MouseState ms)
        {
            bool result = false;
            Rectangle cardHeap;

            float x = this.Width, y = this.Height;

            foreach (Card item in this.TableDeck)
            {
                if (item.PositionX < x)
                {
                    x = item.PositionX;
                }

                if (item.PositionY < y)
                {
                    y = item.PositionY;
                }
            }

            cardHeap = new Rectangle((int)x, (int)y, this.Game.CARD_WIDTH * 2, this.Game.CARD_HEIGHT * 2);

            if (cardHeap.Contains(ms.Position))
            {
                result = true;
            }

            return result;
        }

        public void GetCurrentCard(MouseState ms)
        {
            float maxDepth = 0.8f;
            List<Card> selectedCards = new List<Card>();

            foreach (Card item in this.Players[0].CardDeck)
            {
                if (item.GetRect.Contains(ms.Position))
                {
                    if (TableDeck.Count != 0)
                    {
                        foreach (Card card in TableDeck)
                        {
                            if (card.CardValue == item.CardValue)
                            {
                                selectedCards.Add(item);
                            }
                        }
                    }
                    else
                    {
                        selectedCards.Add(item);
                    }
                }
            }

            foreach (Card item in selectedCards)
            {
                if (item.Depth < maxDepth)
                {
                    maxDepth = item.Depth;
                }
            }

            foreach (Card item in this.Players[0].CardDeck)
            {
                if (item.Depth == maxDepth)
                {
                    item.Current = true;
                }
                else
                {
                    item.Current = false;
                }
            }
        }

        public bool IsCardChosen()
        {
            bool result = false;

            foreach (Card item in Players[0].CardDeck)
            {
                if (item.Current)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public void UpdateTable()
        {
            foreach (Player item in Players)
            {
                item.SetupCardPosition(this.Width, this.Height);
            }

            float depth = 0.8f;

            foreach (Card item in TableDeck)
            {
                depth -= 0.01f;
                item.Depth = depth;
            }

            depth = 0.8f;
            foreach (Card item in CardDeck)
            {
                depth -= 0.01f;
                item.Depth = depth;
            }

            depth = 0.8f;
            foreach (Card item in OutDeck)
            {
                depth -= 0.01f;
                item.Depth = depth;
            }
        }

        public void TakeFromCardDeck()
        {
            if (this.CardDeck.Count == 0)
            {
                return;
            }

            Card temp = new Card(this.Game);
            int curPlayer = this.NextPlayer;

            while (true)
            {

                if (this.Players[curPlayer].CardDeck.Count < 6)
                {
                    temp = CardDeck[this.CardDeck.Count - 1];
                    this.Players[curPlayer].CardDeck.Add(temp);
                    CardDeck.Remove(temp);
                }

                if (this.CardDeck.Count == 0 || (this.Players[0].CardDeck.Count >= 6 && this.Players[1].CardDeck.Count >= 6))
                {
                    break;
                }

                curPlayer = curPlayer == this.Players.Length - 1 ? 0 : curPlayer + 1;
            }

            UpdateTable();
        }

        public int MakeStep()
        {
            if (Players[0].CardDeck.Count == 0)
            {
                return -1;
            }

            Card currentPlCard = this.Players[0].MakeStep();

            int x = (this.Width - this.Game.CARD_WIDTH) / 2, y = (this.Height - this.Game.CARD_HEIGHT) / 2, i = 0, count = 0;
            Random rand = new Random();
            Vector2 startPoint = new Vector2(rand.Next(10, 20), rand.Next(-30, 30));

            currentPlCard.Position = new Vector2(x - startPoint.X, y + startPoint.Y);
            currentPlCard.Rotation = (float)Math.PI / rand.Next(7, 15);

            TableDeck.Add(currentPlCard);

            UpdateTable();

            return 1;
        }

        public int GameRound()
        {
            if (Players[this.NextPlayer].CardDeck.Count == 0 || Players[this.CurrentPlayer].CardDeck.Count == 0)
            {
                return -1;
            }

            Card currentPlCard = this.Players[0].MakeStep();

            int x = (this.Width - this.Game.CARD_WIDTH) / 2, y = (this.Height - this.Game.CARD_HEIGHT) / 2, i = 0, count = 0;
            Random rand = new Random();
            Vector2 startPoint = new Vector2(rand.Next(10, 20), rand.Next(-30, 30));

            currentPlCard.Position = new Vector2(x - startPoint.X, y + startPoint.Y);
            currentPlCard.Rotation = -(float)Math.PI / rand.Next(7, 15);

            TableDeck.Add(currentPlCard);

            //Card nextPlCard = Players[this.NextPlayer].GetCard();

            Card nextPlCard = Players[this.NextPlayer].BeatCard(currentPlCard, Trump);

            if (nextPlCard.Empty == true)
            {
                this.TakeDeck(this.NextPlayer);
                return 1;
            }
            else
            {
                nextPlCard.Position = new Vector2(x + startPoint.X, y + startPoint.Y);
                nextPlCard.Rotation = (float)Math.PI / rand.Next(7, 15);

                TableDeck.Add(nextPlCard);

            }
            UpdateTable();

            return 0;
        }

        public int GameRound2()
        {
            if (Players[this.NextPlayer].CardDeck.Count == 0 || Players[this.CurrentPlayer].CardDeck.Count == 0)
            {
                return -1;
            }

            Card currentPlCard = this.Players[CurrentPlayer].GetCard(TableDeck);

            if (currentPlCard.Empty != true)
            {
                int x = (this.Width - this.Game.CARD_WIDTH) / 2, y = (this.Height - this.Game.CARD_HEIGHT) / 2, i = 0, count = 0;
                Random rand = new Random();
                Vector2 startPoint = new Vector2(rand.Next(10, 20), rand.Next(-30, 30));

                currentPlCard.Position = new Vector2(x - startPoint.X, y + startPoint.Y);
                currentPlCard.Rotation = -(float)Math.PI / rand.Next(7, 15);

                TableDeck.Add(currentPlCard);
            }
            else
            {
                return 0;
            }

            UpdateTable();

            return 1;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D textureCard, Texture2D textureTarot, Texture2D textureTable)
        {
            foreach (Player item in Players)
            {
                if (item.Alignment == Alignment.Bottom)
                {
                    item.Draw(spriteBatch, textureCard, true);
                }
                else
                {
                    item.Draw(spriteBatch, textureTarot, false);
                    //item.Draw(spriteBatch, textureCard, true);
                }
            }

            foreach (Card item in TableDeck)
            {
                item.Draw(spriteBatch, textureCard, true);
            }

            if (CardDeck.Count != 0)
            {
                CardDeck[0].Draw(spriteBatch, textureCard, true);
            }

            foreach (Card item in CardDeck)
            {
                if (item != CardDeck[0])
                {
                     item.Draw(spriteBatch, textureTarot, false);
                }
            }

            foreach (var item in OutDeck)
            {
                item.Draw(spriteBatch, textureTarot, false);
            }

            var effects = SpriteEffects.None;
            //var origin = new Vector2(this.Width / 2, this.Height / 2);
            var origin = new Vector2(0, 0);
            var p = new Vector2(0, 0);
            Rectangle currentRectangle = new Rectangle(0, 0, 800, 480);

            spriteBatch.Draw(textureTable, p, currentRectangle, Color.White, 0f, origin, 1f, effects, 0.99f);

        }
    }
}
