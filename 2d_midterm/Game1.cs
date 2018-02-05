using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _2d_midterm
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D titleScreen;
        Texture2D spriteSheet;
        SpriteFont fnt;

        int timeLeft = 60000;

        enum GameState
        {
            TitleScreen,
            Playing,
            Paused,
            GameOver
        }
        GameState currenState = GameState.TitleScreen;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
         

            this.IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 640;
            graphics.ApplyChanges();

            //Player.Initialize(playerTexture, new Rectangle(0, 0, 32, 32), 2, new Vector2(200, 200));
            Camera.WorldRectangle = new Rectangle(0, 0, 1600, 1600);
            //Camera's Viewable area dimensions
            Camera.ViewPortWidth = 640;
            Camera.ViewPortHeight = 640;

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

            titleScreen = Content.Load<Texture2D>(@"Textures/background");
            fnt = Content.Load<SpriteFont>(@"Fonts/Terminal");

            spriteSheet = Content.Load<Texture2D>(@"Textures/spritesheet");

            TileMap.Initialize(spriteSheet);

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
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState ks = Keyboard.GetState();

            switch (currenState)
            {
                case GameState.TitleScreen:
                    if (ks.IsKeyDown(Keys.Enter))
                    {
                        Player.Initialize(spriteSheet,new Rectangle(0, 0, 32, 32),2, new Vector2(200,400),1);

                        currenState = GameState.Playing;
                    }
                    break;
                case GameState.Playing:

                    timeLeft -= gameTime.ElapsedGameTime.Milliseconds;
                    if (timeLeft < 0)
                    {
                        currenState = GameState.GameOver;
                    }
                    Player.Update(gameTime);
                    break;
                case GameState.Paused:
                    break;
                case GameState.GameOver:
                    break;
                default:
                    break;
            }



           

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (currenState)
            {
                case GameState.TitleScreen:
                    spriteBatch.Draw(titleScreen, new Rectangle(0, 0, titleScreen.Width, titleScreen.Height), Color.White);
                    spriteBatch.DrawString(fnt, "Press Enter To Start.", new Vector2(200, 200), Color.Yellow);   
                    break;
                case GameState.Playing:

                    spriteBatch.Draw(titleScreen, new Rectangle(0, 0, titleScreen.Width, titleScreen.Height), Color.Green);
                    //spriteBatch.DrawString(fnt, "Game Has Started.", new Vector2(200, 200), Color.Yellow);
          

                    TileMap.Draw(spriteBatch);
                    Player.Draw(spriteBatch);

                    spriteBatch.DrawString(fnt, (timeLeft / 1000).ToString(), new Vector2(310, 40), Color.Yellow);

                    break;
                case GameState.Paused:
                    break;
                case GameState.GameOver:
                    spriteBatch.Draw(titleScreen, new Rectangle(0, 0, titleScreen.Width, titleScreen.Height), Color.White);
                    spriteBatch.DrawString(fnt, "Game Over.", new Vector2(200, 200), Color.Yellow);
                    spriteBatch.DrawString(fnt, "Score: " + Player.score.ToString(), new Vector2(200, 400), Color.YellowGreen);
                    break;
                default:
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
