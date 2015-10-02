using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

using System.Collections.Generic;

namespace CardsGL
{
    public enum Difficulty
    {
        Easy = 0,
        Normal,
        Pro
    }

    public enum GameStates
    { 
        New = 0,
        MainMenu,
        VideoMenu,
        Started,

        PlayerTurn,
        AITurn,
        End
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //=====================================
        public const int CARD_SUIT_COUNT = 4;
        public const int NUMBER_OF_CARDS = 13;
        public const int CARDS_IN_HAND = 6;

        public const int CARD_WIDTH = 60;
        public const int CARD_HEIGHT = 83;

        public const int BORDER_SIZE = 10;

        public const int MAX_CARDS_IN_ROW = 12;
        //=====================================

        public readonly string[] mainMenuItemNamesEng = { "Resume", "New Game", "Options", "Exit" };
        public readonly string[] mainMenuItemNamesRus = { "К игре", "Новая игра", "Настройки", "Выход" };

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget;

        Texture2D textureCommon;
        Texture2D textureTarots;
        Texture2D textureTable;
        Texture2D textureEpic;
        Texture2D textureButtonUp;
        Texture2D textureButtonDown;

        SpriteFont mainFont;

        MouseState previousMouseState;
        KeyboardState previousState;


        int winner;

        Button throwOut;
        Button takeTableDeck;

        GameStates prevGameState;


        ButtonState lastMouseState, currentMouseState;
        bool clickOccurred;
        GameStates state;

        Table cardTable;
        Menu gameMenu;

        //=================================================

        public GameStates State { get { return state; } }
        public GameStates PrevGameState { get { return prevGameState; } }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
            Content.RootDirectory = "Content";
            Window.Title = "Durak Classic";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            cardTable = new Table(2, GraphicsDevice.Viewport.Width - 200, GraphicsDevice.Viewport.Height);

            gameMenu = new Menu(this);
            gameMenu.Update();

            throwOut = new Button(this, 500, cardTable.Height / 2 - 120, 70, 134);
            takeTableDeck = new Button(this, 500, cardTable.Height / 2 + 35, 70, 134);

            //cardTable.CardSetup();
            //cardTable.UpdateTable();

            clickOccurred = false;
            lastMouseState = ButtonState.Released;
            currentMouseState = ButtonState.Released;

            state = GameStates.MainMenu;
            prevGameState = GameStates.MainMenu;

            winner = -1;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            textureCommon = Content.Load<Texture2D>("Images/Sprites.png");
            textureTarots = Content.Load<Texture2D>("Images/Tarots.png");
            textureTable = Content.Load<Texture2D>("Images/Table.png");
            textureEpic = Content.Load<Texture2D>("Images/Final.png");
            textureButtonUp = Content.Load<Texture2D>("Images/FingerUp.png");
            textureButtonDown = Content.Load<Texture2D>("Images/FingerDown.png");

            mainFont = Content.Load<SpriteFont>("mainFont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Check mouse left button click
        /// </summary>
        private bool mouseLeftButtonClick(MouseState mouseState)
        {
            return previousMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Check keyboard button pressed
        /// </summary>
        private bool kbButtonPressed(Keys key, KeyboardState kbState)
        {
            return kbState.IsKeyDown(key) && !previousState.IsKeyDown(key);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState kbState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            
            if (mouseLeftButtonClick(mouseState) && state == GameStates.End)
            {
                cardTable.CurrentPlayer = 0;
                cardTable.NextPlayer = 1;
                cardTable.CardSetup();
                cardTable.UpdateTable();
                winner = -1;
                state = GameStates.New;
            }

            if (kbButtonPressed(Keys.Escape, kbState) && state == GameStates.MainMenu)
            {
                state = prevGameState;
            }
            else
            {
                if (kbButtonPressed(Keys.Escape, kbState) && state != GameStates.MainMenu)
                {
                    prevGameState = state;
                    state = GameStates.MainMenu;
                }
            }

            if (kbButtonPressed(Keys.Q, kbState) && state == GameStates.MainMenu)
                Exit();

            // TODO: Add your update logic here

            if (state == GameStates.MainMenu)
            {
                //state = GameStates.MainMenu;
                //prevGameState = GameStates.Started;
                //gameMenu.Update();
                gameMenu.GetSelectedItem(mouseState);

                if (mouseLeftButtonClick(mouseState))
                {
                    switch (gameMenu.MenuItemClick())
                    {
                        case 0: state = prevGameState; break;

                        case 1: cardTable.CurrentPlayer = 0;
                                cardTable.NextPlayer = 1;
                                cardTable.CardSetup();
                                cardTable.UpdateTable();
                                throwOut.Enabled = true;
                                takeTableDeck.Enabled = true;
                                state = GameStates.New; break;

                        case 3: Exit(); break;
                        default: break;
                    }
                }

            }
            else
            {
                if (mouseLeftButtonClick(mouseState) && cardTable.ClickOnTableDeck(mouseState))
                {
                    if (state == GameStates.Started)
                    {
                        cardTable.ThrowOut();
                        cardTable.TakeFromCardDeck();
                        cardTable.CurrentPlayer = 1;
                        cardTable.NextPlayer = 0;
                        throwOut.Enabled = false;
                        takeTableDeck.Enabled = true;
                        state = GameStates.AITurn;
                    }
                    else
                    {
                        if (state == GameStates.PlayerTurn)
                        {
                            cardTable.TakeDeck(0);
                            cardTable.TakeFromCardDeck();
                            throwOut.Enabled = true;
                            takeTableDeck.Enabled = false;
                            state = GameStates.AITurn;
                        }
                    }
                }

                if (state == GameStates.Started)
                {
                    throwOut.GetSelected(mouseState);
                    throwOut.Update();

                    if (mouseLeftButtonClick(mouseState))
                    {
                        if (throwOut.Selected)
                        {
                            cardTable.ThrowOut();
                            cardTable.TakeFromCardDeck();
                            cardTable.CurrentPlayer = 1;
                            cardTable.NextPlayer = 0;
                            state = GameStates.AITurn;
                        }
                    }
                }

                if (state == GameStates.PlayerTurn)
                {
                    takeTableDeck.GetSelected(mouseState);
                    takeTableDeck.Update();

                    if (mouseLeftButtonClick(mouseState) && takeTableDeck.Selected)
                    {
                        cardTable.TakeDeck(0);
                        cardTable.TakeFromCardDeck();
                        state = GameStates.AITurn;
                    }
                }

                if (kbState.IsKeyDown(Keys.T) && !previousState.IsKeyDown(Keys.T) && state == GameStates.Started)
                {
                    cardTable.ThrowOut();
                    cardTable.TakeFromCardDeck();
                    cardTable.CurrentPlayer = 1;
                    cardTable.NextPlayer = 0;
                    state = GameStates.AITurn;
                }

                if (kbState.IsKeyDown(Keys.G) && !previousState.IsKeyDown(Keys.G) && state == GameStates.PlayerTurn)
                {
                    cardTable.TakeDeck(0);
                    cardTable.TakeFromCardDeck();
                    state = GameStates.AITurn;
                }


                //if (kbState.IsKeyDown(Keys.N) && !previousState.IsKeyDown(Keys.N) && state == GameStates.Started)
                //{
                //    cardTable.NextPlayer = cardTable.NextPlayer == 3 ? 0 : cardTable.NextPlayer + 1;
                //}

                if (cardTable.TableDeck.Count == 0 && (cardTable.Players[0].CardDeck.Count == 0 || cardTable.Players[1].CardDeck.Count == 0) && state != GameStates.MainMenu)
                {
                    if (cardTable.Players[0].CardDeck.Count == 0)
                    {
                        winner = 0;
                    }
                    else
                    {
                        winner = 1;
                    }

                    state = GameStates.End;
                }

                if (cardTable.CurrentPlayer == 0)
                {
                    if (state == GameStates.PlayerTurn)
                    {
                        cardTable.GetCurrentCard(mouseState);
                    }
                    else
                    {
                        cardTable.GetCurrentCard(mouseState);
                    }

                    // Recognize a single click of the left mouse button
                    if (mouseLeftButtonClick(mouseState))
                    {
                        if (cardTable.IsCardChosen())
                        {
                            if (cardTable.GameRound() == 1)
                            {
                                cardTable.TakeFromCardDeck();
                                throwOut.Enabled = true;
                                takeTableDeck.Enabled = true;
                            }

                            state = GameStates.Started;
                            //System.Threading.Thread.Sleep(1000);
                            //cardTable.CurrentPlayer = cardTable.CurrentPlayer == 3 ? 0 : cardTable.CurrentPlayer + 1;
                        }

                        clickOccurred = true;
                    }
                }
                else
                {
                    if (state == GameStates.AITurn)
                    {
                        if (cardTable.GameRound2() == 0)
                        {
                            cardTable.ThrowOut();
                            cardTable.TakeFromCardDeck();
                            cardTable.CurrentPlayer = 0;
                            cardTable.NextPlayer = 1;
                            throwOut.Enabled = false;
                            takeTableDeck.Enabled = true;
                        }

                        state = GameStates.PlayerTurn;
                    }
                }

                if (cardTable.NextPlayer == 0)
                {
                    cardTable.GetBeatCard(mouseState);

                    // Recognize a single click of the left mouse button
                    if (mouseLeftButtonClick(mouseState) && state == GameStates.PlayerTurn)
                    {
                        if (cardTable.IsCardChosen())
                        {
                            if (cardTable.MakeStep() == 1)
                            {
                                state = GameStates.AITurn;
                                //cardTable.TakeFromCardDeck();
                            }

                            //state = GameStates.Started;
                            //System.Threading.Thread.Sleep(1000);
                            //cardTable.CurrentPlayer = cardTable.CurrentPlayer == 3 ? 0 : cardTable.CurrentPlayer + 1;
                        }

                        clickOccurred = true;
                    }
                }
            }
            previousMouseState = mouseState;
            previousState = kbState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Black);
            //perform draw callsThen render this target (your whole screen) to the back buffer:

            spriteBatch.Begin(SpriteSortMode.BackToFront);


            cardTable.Draw(spriteBatch, textureCommon, textureTarots, textureTable);
            throwOut.Draw(spriteBatch,  textureButtonUp);
            takeTableDeck.Draw(spriteBatch, textureButtonDown);


            if (state == GameStates.MainMenu)
            {
                gameMenu.Draw(spriteBatch, textureTable, mainFont);

            }

            if (state == GameStates.End)
            {
                var effects = SpriteEffects.None;
                var origin = new Vector2(150, 100);
                //var origin = new Vector2(0, 0);
                var p = new Vector2(cardTable.Width / 2, cardTable.Height / 2);
                Rectangle currentRectangle = new Rectangle(300 * winner, 0, 300, 200);

                spriteBatch.Draw(textureEpic, p, currentRectangle, Color.White, 0f, origin, 1f, effects, 0f);
            }

            spriteBatch.End();

            //set rendering back to the back buffer
            //GraphicsDevice.SetRenderTarget(null);

            ////render target to back buffer
            //renderTargetBatch.Begin();
            //renderTargetBatch.Draw(renderTarget, new Rectangle(0, 0, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height), Color.White);
            //renderTargetBatch.End();

            base.Draw(gameTime);
        }
    }
}
