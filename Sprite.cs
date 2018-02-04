using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


public class Class1
{

    class Sprite
    {
        //----------------------------------------------------------------
        #region Declarations

        public Texture2D Texture;                       //Sprite sheet
        private Vector2 worldLocation = Vector2.Zero;   //Location in the whole world
        private Vector2 velocity = Vector2.Zero;        //Speed and Direction
        private List<Rectangle> frames = new List<Rectangle>(); //List of frames from sprite sheet
        private int currentFrame;                       //Current animation frame
        private float frameTime = 0.1f;                 //animation timing control value
        private float timeForCurrentFrame = 0.0f;       //Counter variable for animation speed
        private Color tintColor = Color.White;          // Color of Light 
        private float rotation = 0.0f;                  //Degrees of rotation
        public bool Expired = false;                    //Whether this sprite has expired
        public bool Animate = true;                     //To animate or not animate
        public bool AnimateWhenStopped = true;  //Animate when the sprite is not moving around?
        public bool Collidable = true;                  //Does this object collide with others?
        public int CollisionRadius = 0;                 //radius for collision detection
        public int BoundingXPadding = 0;                //padding when using rectangular collis.
        public int BoundingYPadding = 0;
        #endregion
        //----------------------------------------------------------------------


        //----------------------------------------------------------------------
        #region Constructor


        //Constructor
        public Sprite(Vector2 worldLocation, Texture2D texture, Rectangle initialFrame,
            Vector2 velocity)
        {
            this.worldLocation = worldLocation;     //our position within the world
            Texture = texture;                      //sprite sheet
            this.velocity = velocity;               //set velocity
            frames.Add(initialFrame);               //initial frame of possible animation
        }


        //----------------------------------------------------------------------        
        #endregion




        //----------------------------------------------------------------------        
        #region Drawing and Animation Properties

        //returns the Width of a frame
        public int FrameWidth
        {
            get { return frames[0].Width; }
        }

        //returns the height of a frame
        public int FrameHeight
        {
            get { return frames[0].Height; }
        }

        //Returns or sets the current light color
        public Color TintColor
        {
            get { return tintColor; }
            set { tintColor = value; }
        }

        //returns the rotation degrees/radians
        //The set makes sure that the rotation is no more than 360 degrees
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value % MathHelper.TwoPi; }
        }


        //Returns or changes the frame of animation
        public int Frame
        {
            get { return currentFrame; }
            set
            {
                //Change the fram based on value, making sure we stay within the frames range
                currentFrame = (int)MathHelper.Clamp(value, 0, frames.Count - 1);
            }
        }


        //Returns or sets the frametime
        public float FrameTime
        {
            get { return frameTime; }
            //take the maximum value between 0 and the value set...makes sure we stay positive
            set { frameTime = MathHelper.Max(0, value); }
        }


        //Returns the current rectangle being used to get the current animation frame
        public Rectangle Source
        {
            get { return frames[currentFrame]; }
        }

        #endregion
        //----------------------------------------------------------------------        




        //----------------------------------------------------------------------        
        #region Positional Properties

        //Set & get our location in the world
        public Vector2 WorldLocation
        {
            get { return worldLocation; }
            set { worldLocation = value; }
        }


        //Use the world location to figure where we are in the visible screen
        public Vector2 ScreenLocation
        {
            get
            {
                return Camera.Transform(worldLocation);
            }
        }

        //Set or get the velocity (speed and direction)
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        //Returns the rectangle that was used for this sprite, place at a world location
        public Rectangle WorldRectangle
        {
            get
            {
                return new Rectangle((int)worldLocation.X, (int)worldLocation.Y,
                    FrameWidth, FrameHeight);
            }
        }


        //Using the World Rectangle, we can calculate the Screen Rectangle for our camera
        public Rectangle ScreenRectangle
        {
            get
            {
                return Camera.Transform(WorldRectangle);
            }
        }


        //Calculate the relative center of an animation frame
        public Vector2 RelativeCenter
        {
            get { return new Vector2(FrameWidth / 2, FrameHeight / 2); }
        }


        //Returns the center of the world including the adjustment for the center of the frame
        public Vector2 WorldCenter
        {
            get { return worldLocation + RelativeCenter; }
        }


        //Calculate the center of the viewable screen
        public Vector2 ScreenCenter
        {
            get
            {
                return Camera.Transform(worldLocation + RelativeCenter);
            }
        }

        #endregion
        //----------------------------------------------------------------------        



        //----------------------------------------------------------------------        
        #region Collision Related Properties

        //Returns a rectangle representing the Bounding Box in collision detect.
        public Rectangle BoundingBoxRect
        {
            get
            {
                //Use the world location plus padding, and the bottom-right is
                // based on the Frame's dimensions - padding
                return new Rectangle((int)worldLocation.X + BoundingXPadding,
                    (int)worldLocation.Y + BoundingYPadding,
                    FrameWidth - (BoundingXPadding * 2),
                    FrameHeight - (BoundingYPadding * 2));
            }
        }
        #endregion


        #region Collision Detection Methods

        //Returns whether or not a collision between this sprite's 
        //   rectangle and another sprite's rectangle
        public bool IsBoxColliding(Rectangle OtherBox)
        {
            //Only check if this is a collidable sprite and it has not expired
            if ((Collidable) && (!Expired))
            {
                //Do the two rectangles collide?
                return BoundingBoxRect.Intersects(OtherBox);
            }
            else
            {
                return false; //If expired or not collideable, then return false
            }
        }



        //Checks whether two items are colliding, using a circular collision detection
        // Pass in the other sprite' center point and radius
        public bool IsCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            //If the sprite is collidable and is not expired...
            if ((Collidable) && (!Expired))
            {
                //check for collision...(Distance between centers is less than
                // the sum of their 2 radius
                if (Vector2.Distance(WorldCenter, otherCenter) <
                (CollisionRadius + otherRadius))
                    return true;
                else
                    return false;
            }
            else
            {
                return false; //expired or not collideable...return false
            }
        }
        #endregion



        #region Animation-Related Methods

        //Add an animation frame
        public void AddFrame(Rectangle frameRectangle)
        {
            frames.Add(frameRectangle); //add frame to the list
        }

        //Set the rotation of the sprite according to the direction variable passed in
        public void RotateTo(Vector2 direction)
        {
            Rotation = (float)Math.Atan2(direction.Y, direction.X);
        }
        #endregion




        #region Update and Draw Methods


        public virtual void Update(GameTime gameTime)
        {
            //If sprite has not expired...
            if (!Expired)
            {
                //Get the elapsed time since last update
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Add this elapsed time to our time accumulator
                timeForCurrentFrame += elapsed;

                // if animation is turned on...
                if (Animate)
                {
                    //if the appropriate time has passed...
                    if (timeForCurrentFrame >= FrameTime)
                    {
                        //If they remain animated when stopped or if they are moving...
                        if ((AnimateWhenStopped) || (velocity != Vector2.Zero))
                        {
                            //Move to the next frame and make sure we stay within the list of frames
                            currentFrame = (currentFrame + 1) % (frames.Count);
                            //Reset the accumulator
                            timeForCurrentFrame = 0.0f;
                        }
                    }
                }
                //Adjust world location of sprite using velocity and time per update
                worldLocation += (velocity * elapsed);
            }
        }


        //Method to draw this sprite
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //if not expired...
            if (!Expired)
            {
                //If the sprite is in the visible part of the world...
                if (Camera.ObjectIsVisible(WorldRectangle))
                {
                    //Draw the sprite using the appropriate class properties
                    spriteBatch.Draw(Texture, ScreenCenter, Source, tintColor, rotation,
                        RelativeCenter, 1.0f, SpriteEffects.None, 0.0f);
                }
            }
        }
        #endregion
        //----------------------------------------------------------------------        







    }
}
