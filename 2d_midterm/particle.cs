using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _2d_midterm
{
    class Particle : Sprite
    {
        //----------------------------------------------------------------
        #region Part 1 - Declarations

        private Vector2 acceleration;       //the acceleration of the particle in both X & Y
        private float maxSpeed;             //maximum speed that we can accelerate to
        private int initialDuration;        //life expectancy of the sprite
        private int remainingDuration;      //How much life is left
        private Color initialColor;         //starting color of the particle
        private Color finalColor;           //color of the particle at the end of the transition

        #endregion
        //----------------------------------------------------------------



        //---------------------------------------------------------------------
        #region Part 1 - Properties

        //How much actual time has passed
        public int ElapsedDuration
        {
            get
            {
                return initialDuration - remainingDuration;
            }
        }


        //Returns the percentage/ratio of how long the particle has lived
        public float DurationProgress
        {
            get
            {
                return (float)ElapsedDuration / (float)initialDuration;
            }
        }

        //Lets us know if the particle is still active/alive
        public bool IsActive
        {
            get
            {
                return (remainingDuration > 0); //if time still remaining on life, it lives
            }
        }

        #endregion
        //---------------------------------------------------------------------




        //---------------------------------------------------------------------
        #region Part 1 - Constructor

        public Particle(
        Vector2 location,           //location for the particle to be
        Texture2D texture,          //sprite sheet we will be pulling images from
        Rectangle initialFrame,     //starting from from sprite sheet
        Vector2 velocity,           //current speed & direction
        Vector2 acceleration,       //increase in speed & direction
        float maxSpeed,             // maximum speed that can be reached
        int duration,               // life of the particle
        Color initialColor,         // starting color
        Color finalColor)           // ending color
            : base(location, texture, initialFrame, velocity)   //call construct for Sprite(base)
        {
            //Set default internal values based on passed in parameters
            //Some properties start with "this."  Why?
            // take a look at the parameter they receive the value from
            initialDuration = duration;
            remainingDuration = duration;
            this.acceleration = acceleration;
            this.initialColor = initialColor;
            this.maxSpeed = maxSpeed;
            this.finalColor = finalColor;
        }

        #endregion
        //---------------------------------------------------------------------



        //---------------------------------------------------------------------
        #region Part 1 - Update and Draw

        public override void Update(GameTime gameTime)
        {
            //If time has run out for the particle...
            if (remainingDuration <= 0)
            {
                Expired = true;     //set to expired
            }

            //if the particle has not expired...
            if (!Expired)
            {
                //increase velocity
                Velocity += acceleration;

                //If we have exceeded speed limit, reset back to max speed
                if (Velocity.Length() > maxSpeed)
                {
                    Vector2 vel = Velocity;
                    vel.Normalize();
                    Velocity = vel * maxSpeed;
                }

                //Use Lerp to transition to the next color within the color transition cycle
                // controled by the life duration of the particle
                TintColor = Color.Lerp(
                initialColor,
                finalColor,
                DurationProgress);
                remainingDuration--;
            }

            //Call sprite's update
            base.Update(gameTime);
        }

        //Method to draw living particle
        public override void Draw(SpriteBatch spriteBatch)
        {
            //if alive, draw it via the sprite draw
            if (IsActive)
            {
                base.Draw(spriteBatch);
            }
        }

        #endregion
        //---------------------------------------------------------------------


    }
}
