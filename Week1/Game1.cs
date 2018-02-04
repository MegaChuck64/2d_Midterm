using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Week1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D backgroundTexture;
        SpriteManager spriteManager;

        KeyboardState kb;
        Song backgroundMusic;


        enum Location
        {

        }

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
            // TODO: Add your initialization logic here


            spriteManager = new SpriteManager(this);


            this.IsMouseVisible = true;
            graphics.PreferredBackBufferHeight = 750;
            graphics.PreferredBackBufferWidth = 750;
            graphics.ApplyChanges();
            //NEW ->This connects us to the SpriteManager class
            Components.Add(spriteManager);


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
            backgroundTexture = Content.Load<Texture2D>(@"Images/background");
            backgroundMusic = Content.Load<Song>(@"Audio/backgroundMusic");
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = .5f;

            // TODO: use this.Content to load your game content here
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


            kb = Keyboard.GetState();

          


            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.ForestGreen);

            spriteBatch.Begin();
            // Draw background image
            spriteBatch.Draw(backgroundTexture,
            new Rectangle(0, 0, Window.ClientBounds.Width,
            Window.ClientBounds.Height), null,
            Color.White, 0, Vector2.Zero,
            SpriteEffects.None, 0);
            // Draw fonts
            //spriteBatch.DrawString(scoreFont, "Score: " + currentScore,
            //new Vector2(10, 10), Color.DarkBlue, 0, Vector2.Zero,
            //1, SpriteEffects.None, 1);
            spriteBatch.End();
            // TODO: Add your drawing code here



            base.Draw(gameTime);
        }
    }
}
