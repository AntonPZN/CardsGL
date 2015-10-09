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
        //Started,

        PlayerTurn,
        PlayerBeat,
        PlayerTake,
        AITurn,
        AIBeat,
        AITake,
        End
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class CardGame : Game
    {
        //=====================================
        public readonly int CARD_SUIT_COUNT = 4;
        public readonly int NUMBER_OF_CARDS = 13;
        public readonly int CARDS_IN_HAND = 6;

        public readonly int CARD_WIDTH = 60;
        public readonly int CARD_HEIGHT = 83;

        public readonly int BORDER_SIZE = 10;

        public readonly int MAX_CARDS_IN_ROW = 12;
        //=====================================

        public readonly string[] mainMenuItemNamesEng = { "Resume", "New Game", "Options", "Exit" };
        public readonly string[] mainMenuItemNamesRus = { "К игре", "Новая игра", "Настройки", "Выход" };

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        RenderTarget2D renderTarget;

        Texture2D textureCommon;
        Texture2D textureTarots;
        Texture2D textureTable;
        //Texture2D textureEpic;
        public Texture2D textureSuits;
        Texture2D textureEndGame;
        Texture2D textureButtonUp;
        Texture2D textureButtonDown;

        SpriteFont mainFont;

        MouseState previousMouseState;
        KeyboardState previousState;



        //test ===

        public bool showEnemyCards = false;
        InputTextContainer cheat;

        //

        //Button throwOut;
        //Button takeTableDeck;

        GameStates prevGameState;


        ButtonState lastMouseState, currentMouseState;
        bool clickOccurred;
        GameStates state;

        Table cardTable;
        Menu gameMenu;

        EventHandler<TextInputEventArgs> onTextEntered; //event fot keyboard input

        //=================================================

        public GameStates State { get { return state; } }
        public GameStates PrevGameState { get { return prevGameState; } }

        public CardGame()
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

            #if OpenGL
                Window.TextInput += TextEntered;
                onTextEntered += HandleInput;
            #else
                Window.TextInput += HandleInput;
            #endif

            cardTable = new Table(this, 2, GraphicsDevice.Viewport.Width - 200, GraphicsDevice.Viewport.Height);

            gameMenu = new Menu(this);
            gameMenu.Update();

            //throwOut = new Button(this, 500, cardTable.Height / 2 - 120, 70, 134);
            //takeTableDeck = new Button(this, 500, cardTable.Height / 2 + 35, 70, 134);

            //cardTable.CardSetup();
            //cardTable.UpdateTable();

            clickOccurred = false;
            lastMouseState = ButtonState.Released;
            currentMouseState = ButtonState.Released;

            state = GameStates.MainMenu;
            prevGameState = GameStates.MainMenu;

            cheat = new InputTextContainer();

            base.Initialize();
        }

        private void TextEntered(object sender, TextInputEventArgs e)
        {
            if (onTextEntered != null)
                onTextEntered.Invoke(sender, e);
        }

        private void HandleInput(object sender, TextInputEventArgs e)
        {
            cheat.PutChar(e.Character);
            // Do stuff here
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
            textureCommon = Content.Load<Texture2D>("Images/Sprites");
            textureTarots = Content.Load<Texture2D>("Images/Tarots");
            textureTable = Content.Load<Texture2D>("Images/Table");
            textureButtonUp = Content.Load<Texture2D>("Images/FingerUp");
            textureButtonDown = Content.Load<Texture2D>("Images/FingerDown");
            textureSuits = Content.Load<Texture2D>("Images/Suits");

            textureEndGame = Content.Load<Texture2D>("Images/Endgame");

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

        private void cheating()
        {
            if (cheat.Contains("showcards"))
                showEnemyCards = true;
            else if (cheat.Contains("reset"))
                showEnemyCards = false;
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
            //=====================

            this.cheating();

            //=====================
            if (mouseLeftButtonClick(mouseState) && state == GameStates.End)
            {
                cardTable.NewGame();
                if (cardTable.CurrentPlayer == 0)
                {
                    prevGameState = state;
                    state = GameStates.PlayerTurn;
                }
                else
                {
                    prevGameState = state;
                    state = GameStates.AITurn;
                }
                //state = GameStates.New;
            }

            // TODO: Add your update logic here

            if (state == GameStates.MainMenu)
            {
                if (kbButtonPressed(Keys.Escape, kbState))
                {
                    state = prevGameState;
                }

                if (kbButtonPressed(Keys.Q, kbState))
                    Exit();

                //state = GameStates.MainMenu;
                //prevGameState = GameStates.Started;
                //gameMenu.Update();
                gameMenu.GetSelectedItem(mouseState);

                if (mouseLeftButtonClick(mouseState))
                {
                    switch (gameMenu.MenuItemClick())
                    {
                        case 0: state = prevGameState; break;

                        case 1: cardTable.NewGame();
                                if (cardTable.CurrentPlayer == 0)
                                {
                                    prevGameState = state;
                                    state = GameStates.PlayerTurn;
                                }
                                else
                                {
                                    prevGameState = state;
                                    state = GameStates.AITurn;
                                }
                                /*state = GameStates.New;*/ break;

                        case 3: Exit(); break;
                        default: break;
                    }
                }

            }
            else
            {
                if (kbButtonPressed(Keys.Escape, kbState) && state != GameStates.MainMenu)
                {
                    prevGameState = state;
                    state = GameStates.MainMenu;
                }

                if (mouseLeftButtonClick(mouseState) && cardTable.ClickOnTableDeck(mouseState))
                {
                    switch (state)
                    {
                        case GameStates.PlayerTurn: cardTable.EndRound(0);
                                                    this.cardTable.CheckWinner();
                                                    prevGameState = state;
                                                    state = GameStates.AITurn;
                                                    break;
                        case GameStates.PlayerBeat: prevGameState = state; 
                                                    state = GameStates.PlayerTake;
                                                    break;

                        case GameStates.PlayerTake: cardTable.EndRound(1);
                                                    this.cardTable.CheckWinner();
                                                    prevGameState = state;
                                                    state = GameStates.AITurn; 
                                                    break;

                        case GameStates.AITake: cardTable.EndRound(1);
                                                this.cardTable.CheckWinner();
                                                prevGameState = state;
                                                state = GameStates.PlayerTurn;
                                                break;

                        default:
                            break;
                    }
                }

                //if (kbState.IsKeyDown(Keys.T) && !previousState.IsKeyDown(Keys.T) && state == GameStates.PlayerTurn)
                //{
                //    cardTable.EndRound(0);
                //    this.cardTable.CheckWinner();
                //    prevGameState = state;
                //    state = GameStates.AITurn;
                //}

                //if (kbState.IsKeyDown(Keys.G) && !previousState.IsKeyDown(Keys.G) && state == GameStates.PlayerBeat)
                //{
                //    cardTable.EndRound(1);
                //    this.cardTable.CheckWinner();
                //    prevGameState = state;
                //    state = GameStates.AITurn;
                //}
                //===========================================

                if (state == GameStates.PlayerTurn)
                {
                    cardTable.GetCurrentCard(mouseState);

                    if (mouseLeftButtonClick(mouseState) && cardTable.IsCardChosen() && cardTable.GameRound() == 1)
                    {
                        prevGameState = state;
                        state = GameStates.AIBeat;
                    }
                }

                if (state == GameStates.PlayerBeat)
                {
                    cardTable.GetBeatCard(mouseState);

                    if (mouseLeftButtonClick(mouseState) && cardTable.IsCardChosen() && cardTable.PlayerBeatCard() == 1)
                    {
                        prevGameState = state;
                        state = GameStates.AITurn;
                    }
                }

                if (state == GameStates.AITurn)
                {
                    if (cardTable.GameRound() == 1)
                    {
                        prevGameState = state;
                        state = GameStates.PlayerBeat;

                    }
                    else
                    {
                        cardTable.EndRound(0);
                        this.cardTable.CheckWinner();
                        prevGameState = state;
                        state = GameStates.PlayerTurn;
                    }
                }

                if (state == GameStates.AIBeat)
                {
                    if (cardTable.AIBeatCard() == 0)
                    {
                        prevGameState = state;
                        state = GameStates.AITake;
                    }
                    else
                    {
                        prevGameState = state;
                        state = GameStates.PlayerTurn;
                    }
                    
                }

                if (state == GameStates.PlayerTake && prevGameState == GameStates.PlayerBeat)
                {
                    if (cardTable.TossCard() == 0)
                    {
                        cardTable.EndRound(1);
                        this.cardTable.CheckWinner();
                        prevGameState = state;
                        state = GameStates.AITurn;
                    }
                }

                if (state == GameStates.AITake)
                {
                    cardTable.GetCurrentCard(mouseState);

                    if (mouseLeftButtonClick(mouseState) && cardTable.IsCardChosen())
                    {
                        cardTable.GameRound();
                    }
                }
                //if (state == GameStates.PlayerTurn)
                ////if (cardTable.CurrentPlayer == 0)
                //{
                //    cardTable.GetCurrentCard(mouseState);

                //    if (mouseLeftButtonClick(mouseState))
                //    {
                //        if (cardTable.IsCardChosen())
                //        {
                //            if (cardTable.GameRound() == 1)
                //            {
                //                cardTable.EndRound(1);
                //            }

                //            //state = GameStates.PlayerTurn;
                //        }
                //    }
                //}
                //else
                //{
                //    if (state == GameStates.AITurn)
                //    {
                //        if (cardTable.GameRound2() == 0)
                //        {
                //            cardTable.EndRound(0);
                //        }

                //        state = GameStates.PlayerBeat;
                //    }
                //}

                ////if (cardTable.NextPlayer == 0)
                //if (state == GameStates.PlayerBeat)
                //{
                //    cardTable.GetBeatCard(mouseState);

                //    // Recognize a single click of the left mouse button
                //    if (mouseLeftButtonClick(mouseState) && state == GameStates.PlayerBeat)
                //    {
                //        if (cardTable.IsCardChosen())
                //        {
                //            if (cardTable.MakeStep() == 1)
                //            {
                //                state = GameStates.AITurn;
                //            }
                //        }
                //    }
                //}
                //=============================================================================




                //if (cardTable.TableDeck.Count == 0 && (cardTable.Players[0].CardDeck.Count == 0 || cardTable.Players[1].CardDeck.Count == 0) && state != GameStates.MainMenu)
                //{
                //    if (cardTable.Players[0].CardDeck.Count == 0)
                //    {
                //        winner = 0;
                //    }
                //    else
                //    {
                //        winner = 1;
                //    }

                //    state = GameStates.End;
                //}

                if (this.cardTable.GameResult != GameResult.None)
                {
                    state = GameStates.End;
                }

            }
            previousMouseState = mouseState;
            previousState = kbState;
            //prevGameState = prevGameState != state ? state : prevGameState;

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
            //throwOut.Draw(spriteBatch,  textureButtonUp);
            //takeTableDeck.Draw(spriteBatch, textureButtonDown);


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
                Rectangle currentRectangle = new Rectangle(300 * (int)this.cardTable.GameResult, 0, 300, 200);

                spriteBatch.Draw(textureEndGame, p, currentRectangle, Color.White, 0f, origin, 1f, effects, 0f);
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
