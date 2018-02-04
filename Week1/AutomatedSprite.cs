using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Week1
{
    //Inherit from the Sprite Base class
    class AutomatedSprite : Sprite
    {
        // Sprite is automated. Direction is same as speed
        public override Vector2 direction
        {
            get { return speed; }
            set { direction = value; }
        }


        //Constructor
        public AutomatedSprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed)
        {
        }

        //Constructor using ms Per Frame
        public AutomatedSprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
            int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame)
        {
        }



        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Move sprite based on direction
            position += direction;

            //Call base update (Sprite class)
            base.Update(gameTime, clientBounds);
        }
    }
}
