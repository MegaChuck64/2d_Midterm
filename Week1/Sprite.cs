using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Week1
{
    abstract class Sprite
    {
        Texture2D textureImage;
        protected Point frameSize;
        Point currentFrame;
        Point sheetSize;

        int collisionOffset;

        int timeSinceLastFrame = 0;
        int millisecondsPerFrame;
        const int defaultMillisecondsPerFrame = 16;

        protected Vector2 speed;
        protected Vector2 position;

        public abstract Vector2 direction
        {
            get; set;
        }


        //This constructor has no "ms per frame"
        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed)
            : this(textureImage, position, frameSize, collisionOffset, currentFrame,
                  sheetSize, speed, defaultMillisecondsPerFrame)
        {

        }

        //this constructor does have a "ms per frame" field
        public Sprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, 
            int millisecondsPerFrame)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.frameSize = frameSize;
            this.collisionOffset = collisionOffset;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.speed = speed;
            this.millisecondsPerFrame = millisecondsPerFrame;
        }


        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            //sprite animation
            Play(gameTime, Point.Zero, sheetSize);
        }

        public void Play(GameTime gameTime, Point sheetStart, Point sheetEnd)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                // Increment to next frame
                timeSinceLastFrame = 0;
                currentFrame.X++;
                if (currentFrame.X >= sheetEnd.X)
                {
                    currentFrame.X = sheetStart.X;
                    currentFrame.Y++;
                    if (currentFrame.Y >= sheetEnd.Y)
                        currentFrame.Y = sheetStart.Y;
                }

                if (currentFrame.X < sheetStart.X || currentFrame.X > sheetEnd.X)
                {
                    currentFrame.X = sheetStart.X;
                }
                if (currentFrame.Y < sheetStart.Y || currentFrame.Y > sheetEnd.Y)
                {
                    currentFrame.Y = sheetStart.Y;
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Draw the sprite via the passed SpriteBatch
            spriteBatch.Draw(textureImage,
                position,
                new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X, frameSize.Y),
                Color.White, 0, Vector2.Zero,
                1f, SpriteEffects.None, 0);
        }



        // Gets the collision rect based on position, framesize and collision offset
        public Rectangle collisionRect
        {
            get
            {
                return new Rectangle(
                    (int)position.X + collisionOffset,
                    (int)position.Y + collisionOffset,
                    frameSize.X - (collisionOffset * 2),
                    frameSize.Y - (collisionOffset * 2));
            }

        }
    }
}
