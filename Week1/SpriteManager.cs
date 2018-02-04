using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;


namespace Week1
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// YOU MUST MAKE IT INHERIT "DrawableGameComponent" IN ORDER TO AUTO-DRAW
    /// </summary> 
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //SpriteBatch for drawing
        SpriteBatch spriteBatch;

        //A sprite for the player and a list of automated sprites
        Player player;
        // A list is like a zuped-up array
        List<Sprite> spriteList = new List<Sprite>();

        public Point bounds;


        public SpriteManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);


            //SoundEffect[] bouncySounds = new SoundEffect[]
            //    {
            //    Game.Content.Load<SoundEffect>(@"Sounds/bubble"),
            //    Game.Content.Load<SoundEffect>(@"Sounds/punch") ,
            //    Game.Content.Load<SoundEffect>(@"Sounds/gun")
            //};


            player = new Player(Game.Content.Load<Texture2D>(@"Images/player"), 
                                Vector2.Zero, new Point(128, 128), 10, 
                                Point.Zero, new Point(4, 6), new Vector2(4, 4), 256);


            FireBall fb = new FireBall(Game.Content.Load<Texture2D>(@"Images/fireball"), 
                                        new Vector2(player.collisionRect.X, player.collisionRect.Y),
                                        new Point(32, 32), 5, Point.Zero, new Point(4, 1), new Vector2(5,5));

            player.fireBall = fb;
            //Load the player sprite
            //player = new UserControlledSprite(
            //    Game.Content.Load<Texture2D>(@"Images/threerings"),
            //    Vector2.Zero, new Point(75, 75), 10, new Point(0, 0),
            //    new Point(6, 8), new Vector2(6, 6));

            ////Load several different automated sprites into the list
            //// This is done by creating AutomatedSprites and adding them to
            //// the list
            //spriteList.Add(new BouncingSprite(
            //    Game.Content.Load<Texture2D>(@"Images/skullball"),
            //    new Vector2(150, 150), new Point(75, 75), 10, new Point(0, 0),
            //    new Point(6, 8), new Vector2(-1, -1), bouncySounds));
            //spriteList.Add(new BouncingSprite(
            //    Game.Content.Load<Texture2D>(@"Images/skullball"),
            //    new Vector2(300, 150), new Point(75, 75), 10, new Point(0, 0),
            //    new Point(6, 8), Vector2.One, bouncySounds));


            //Call the parent-base
            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            // Update player
            player.Update(gameTime, Game.Window.ClientBounds);

            // Update all sprites if the game is still active
            foreach (Sprite s in spriteList)
            {
                if (Game.IsActive && Game.Window != null)  //Added specifically to manage MonoGame
                {
                    s.Update(gameTime, Game.Window.ClientBounds);

                    // Check for collisions and exit game if there is one
                    if (s.collisionRect.Intersects(player.collisionRect))
                        Game.Exit();
                }
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            // Draw the player
            player.Draw(gameTime, spriteBatch);

            // Draw all sprites
            foreach (Sprite s in spriteList)
                s.Draw(gameTime, spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

