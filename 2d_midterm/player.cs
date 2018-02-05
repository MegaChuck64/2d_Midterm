using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace _2d_midterm
{
    static class Player
    {
        //--------------------------------------------
        #region Declarations

        public static Sprite sprite;
        public static int score;
        private static float playerSpeed = 90f;             //player's speed
        private static Vector2 angle = Vector2.Zero;    //Angle of rotation for tank base
        #endregion
        //--------------------------------------------

        // Declare an area that the camera will keep the player
        private static Rectangle scrollArea = new Rectangle(150, 150, 300, 300);
        //---------------------------------------------------------------------------
        #region Initialization

        public static void Initialize(Texture2D texture, Rectangle initialFrame,
            int frameCount, Vector2 worldLocation, float scale)
        {
            //Use the first frame of animation to get the sprite width and height
            int frameWidth = initialFrame.Width;
            int frameHeight = initialFrame.Height;

            //Create a new sprite for the base of the tank
            sprite = new Sprite(worldLocation, texture, initialFrame, frameCount,Vector2.Zero,scale);

            //Set Collision Padding for the sides of the tank base
            sprite.BoundingXPadding = 4;
            sprite.BoundingYPadding = 4;

            
            sprite.AnimateWhenStopped = false;  //If tank is stopped, do not animat

           
        }

        #endregion
        //---------------------------------------------------------------------------------




        //---------------------------------------------------------------------------------

        #region Input Handling


        private static Vector2 handleKeyboardMovement(KeyboardState keyState)
        {
            //This vector2 will hold the direction to move the tank
            Vector2 keyMovement = Vector2.Zero;

            //Based on keypresses, set the direction (both x,y)
            if (keyState.IsKeyDown(Keys.W))
                keyMovement.Y--;
            if (keyState.IsKeyDown(Keys.A))
                keyMovement.X--;
            if (keyState.IsKeyDown(Keys.S))
                keyMovement.Y++;
            if (keyState.IsKeyDown(Keys.D))
                keyMovement.X++;

            return keyMovement; // return this direction
        }


        //Returns the direction to take the tank based on the gamepad's left thumbstick
        private static Vector2 handleGamePadMovement(GamePadState gamepadState)
        {
            //Return the direction of the tank movement 
            // Y-component is negative because the Thumbstick's Y has a 
            //  positive value for up and negative value for down
            return new Vector2(gamepadState.ThumbSticks.Left.X,
                -gamepadState.ThumbSticks.Left.Y);
        }


        //private static Vector2 handleKeyboardShots(KeyboardState keyState)
        //{
        //    //Create a vector2 that represents the velocity of a shot from the tank
        //    Vector2 keyShots = Vector2.Zero;

        //    //all the numeric keys around #5 on the numberpad- Horiz, Vertic, Diagonal
        //    if (keyState.IsKeyDown(Keys.NumPad1))
        //        keyShots = new Vector2(-1, 1);
        //    if (keyState.IsKeyDown(Keys.NumPad2))
        //        keyShots = new Vector2(0, 1);
        //    if (keyState.IsKeyDown(Keys.NumPad3))
        //        keyShots = new Vector2(1, 1);
        //    if (keyState.IsKeyDown(Keys.NumPad4))
        //        keyShots = new Vector2(-1, 0);
        //    if (keyState.IsKeyDown(Keys.NumPad6))
        //        keyShots = new Vector2(1, 0);
        //    if (keyState.IsKeyDown(Keys.NumPad7))
        //        keyShots = new Vector2(-1, -1);
        //    if (keyState.IsKeyDown(Keys.NumPad8))
        //        keyShots = new Vector2(0, -1);
        //    if (keyState.IsKeyDown(Keys.NumPad9))
        //        keyShots = new Vector2(1, -1);

        //    return keyShots;    //Return the velocity
        //}


        ////Return the right thumbpad direction to control direction to fire shots
        //private static Vector2 handleGamePadShots(GamePadState gamepadState)
        //{
        //    //Return the right thumbpad direction to control direction to fire shots
        //    return new Vector2(gamepadState.ThumbSticks.Right.X,
        //        -gamepadState.ThumbSticks.Right.Y);
        //}


        //Function to handle/manage all the smaller input-based functions from above
        private static void handleInput(GameTime gameTime)
        {
            //Tracked elapsed time between updates
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //create vector2's to handle the movement and firing angles (x,y)
            Vector2 moveAngle = Vector2.Zero;
            //Vector2 fireAngle = Vector2.Zero;

            //Adjust the movement angle based on either KB or GamePad input
            moveAngle += handleKeyboardMovement(Keyboard.GetState());
            moveAngle += handleGamePadMovement(GamePad.GetState(PlayerIndex.One));

            //Adjust the firing angle based on either KB or GamePad input
            //fireAngle += handleKeyboardShots(Keyboard.GetState());
            //fireAngle += handleGamePadShots(GamePad.GetState(PlayerIndex.One));

            //if the move angle is not zero...
            //(This will allow us to keep the tank facing the same way when we let go)
            if (moveAngle != Vector2.Zero)
            {
                moveAngle.Normalize();      //normalize the angle to one unit of movement
                angle = moveAngle;      //set the tank's base angle to the new angle

                //------------------------------------------------------------
                // Adjust position based on tile boundaries
                moveAngle = checkTileObstacles(elapsed, moveAngle);
                //------------------------------------------------------------
            }


            //if the firing angle is not zero...
            ////(This will allow us to keep the turret facing the same way when we let go)
            //if (fireAngle != Vector2.Zero)
            //{
            //    fireAngle.Normalize();      //normalize the angle to one unit of movement
            //    //turretAngle = fireAngle;    //set the turrent angle to the new angle

            //    //Part 1 - if enough time has passed since last shot...
            //    if (WeaponManager.CanFireWeapon)
            //    {
            //        //fire the shot from the location of the turret, at the firing angle and speed
            //        WeaponManager.FireWeapon(TurretSprite.WorldLocation,
            //            fireAngle * WeaponManager.WeaponSpeed);
            //    }

            //}

            //Rotate the base sprite to the base angle
            sprite.RotateTo(angle);
            //Rotate the turret sprite to the turret angle
            //TurretSprite.RotateTo(turretAngle);
            //Set velocity by multiplying the movement angle by the player's speed
            sprite.Velocity = moveAngle * playerSpeed;

            repositionCamera(gameTime, moveAngle); // Adjust camera based on player
        }

        #endregion

        //---------------------------------------------------------------------------------






        //-------------------------------------------------------------------------
        #region Movement Limitations

        //Keep the tank within the boundaries of the world
        private static void clampToWorld()
        {
            //Get the current x and y components of the BaseSprite's location
            float currentX = sprite.WorldLocation.X;
            float currentY = sprite.WorldLocation.Y;

            //Adjust the x component if needed by clamping the tank within
            // the left and right sides of the world
            currentX = MathHelper.Clamp(currentX, 0,
                Camera.WorldRectangle.Right - sprite.FrameWidth);

            //Adjust the y component if needed by clamping the tank within
            // the top and bottom of the world
            currentY = MathHelper.Clamp(currentY, 0,
                Camera.WorldRectangle.Bottom - sprite.FrameHeight);

            //Set the base-sprite's location to the clamped coordinates
            sprite.WorldLocation = new Vector2(currentX, currentY);
        }


        //Adjust the camera's position based on a smaller rectangle in the viewing
        //  area.  As we scroll too close to the boundaries of the viewable area, we 
        // drag the camera with us.
        // Think of being a kid with a cardboard box on top of you...as you move and
        // hit the edges of the box, the box (aka the camera) moves with you.
        private static void repositionCamera(GameTime gameTime, Vector2 moveAngle)
        {
            //track elapsed time since last update
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Movement of the camera is scaled based on speed and elapsed time
            float moveScale = playerSpeed * elapsed;


            //if the base of the tank's screen coord is past the left scroll area AND
             //  the angle for movement is towards the left...
            if ((sprite.ScreenRectangle.X < scrollArea.X) && (moveAngle.X < 0))
                {
                    //Adjust the camera's position to move with the tank
                    Camera.Move(new Vector2(moveAngle.X, 0) * moveScale);
                }


            // if the base of the tank's screen coord is past the right scroll area AND
            //   the angle for movement is towards the right...
            if ((sprite.ScreenRectangle.Right > scrollArea.Right) && (moveAngle.X > 0))
            {
                //Adjust the camera's position to move with the tank
                Camera.Move(new Vector2(moveAngle.X, 0) * moveScale);
            }

            //if the base of the tank's screen coord is past the top scroll area AND
              // the angle for movement is towards the top...
            if ((sprite.ScreenRectangle.Y < scrollArea.Y) && (moveAngle.Y < 0))
                {
                    //Adjust the camera's position to move with the tank
                    Camera.Move(new Vector2(0, moveAngle.Y) * moveScale);
                }

            // if the base of the tank's screen coord is past the bottom scroll area AND
            //   the angle for movement is towards the bottom...
            if ((sprite.ScreenRectangle.Bottom > scrollArea.Bottom) && (moveAngle.Y > 0))
            {
                //Adjust the camera's position to move with the tank
                Camera.Move(new Vector2(0, moveAngle.Y) * moveScale);
            }

        }


       // Collisions with walls
       //Method to return a modified movement vector if there is either a vertical or
       //   horizontal collision with a wall obstacle
        private static Vector2 checkTileObstacles(float elapsedTime, Vector2 moveAngle)
        {
           // Calculate the new location just using the horizontal part of movement
            Vector2 newHorizontalLocation = sprite.WorldLocation +
                (new Vector2(moveAngle.X, 0) * (playerSpeed * elapsedTime));

//            Calculate the new location just using the vertical part of movement
            Vector2 newVerticalLocation = sprite.WorldLocation +
                (new Vector2(0, moveAngle.Y) * (playerSpeed * elapsedTime));

  //          Create a rectangle in the new Horizontal- only position
             Rectangle newHorizontalRect = new Rectangle(
                 (int)newHorizontalLocation.X, (int)sprite.WorldLocation.Y,
                 sprite.FrameWidth, sprite.FrameHeight);

            //Create a rectangle in the new Vertical- only position
             Rectangle newVerticalRect = new Rectangle(
                 (int)sprite.WorldLocation.X, (int)newVerticalLocation.Y,
                 sprite.FrameWidth, sprite.FrameHeight);

           /* ------------------------------------------------------------------------------
             Create integers to hold the pixel coordinate components
              (left - most, right - most, top - most, bottom - most)
             In essence, we are going to be calculating the portion of the rectangle
             that is not connected to the original starting point's rectangle.  From there,
              we will do pixel - by - pixel collision detection only for those pixels that
    
                 are not in a known part of the rectangle
                ------------------------------------------------------------------------------
    */
                int horizLeftPixel = 0;
            int horizRightPixel = 0;
            int vertTopPixel = 0;
            int vertBottomPixel = 0;


            //if we are moving left...
            if (moveAngle.X < 0)
            {
                //Get the leftmost part of the new "horizontal" rectangle from above
                horizLeftPixel = (int)newHorizontalRect.Left;
                //for the right-most side, take the left - most side of our sprite's world position
                horizRightPixel = (int)sprite.WorldRectangle.Left;
            }

            //if we are moving right...
            if (moveAngle.X > 0)
            {
              //  the left-most side is based on the sprite's right-most side
                horizLeftPixel = (int)sprite.WorldRectangle.Right;
                //the right-most side is the right - most side of the new "horizontal"
                 //rectangle from above
                horizRightPixel = (int)newHorizontalRect.Right;
            }

            //if we are moving up...
            if (moveAngle.Y < 0)
            {
                //top - most pixel to scan is the new "vertical" rectangle's top
                vertTopPixel = (int)newVerticalRect.Top;
              //  bottom - most pixel is set to the top - most pixel of the tank's world rectangle
                vertBottomPixel = (int)sprite.WorldRectangle.Top;
            }

            //fi we are moving down...
            if (moveAngle.Y > 0)
            {
               // Top - most pixel to check is the bottom of the tank's world rectangle
                vertTopPixel = (int)sprite.WorldRectangle.Bottom;
               // Bottom - most pixel is the bottom of the new "vertical" rectangle
                  vertBottomPixel = (int)newVerticalRect.Bottom;
            }

            //if there is movement on the X - axis...
            if (moveAngle.X != 0)
            {
                //loop from left-most to right-most pixel (columns)
                for (int x = horizLeftPixel; x < horizRightPixel; x++)
                {
                    //For each column, now we are going to loop thru the rows pixel-by-pixel
                    for (int y = 0; y < sprite.FrameHeight; y++)
                    {
                        //if the current pixel position is part of a wall...
                        if (TileMap.IsWallTileByPixel(new Vector2(x, newHorizontalLocation.Y + y)))
                        {
                            moveAngle.X = 0;    //no horizontal movement
                            break;              //break out of the inner for-loop
                        }
                    }
                    if (moveAngle.X == 0)
                    {   //if we had a collision, break out of outer for-loop
                        break;
                    }
                }
            }


            //Same thing will apply to this section as above, except we will
             //be focusing only on the vertical movement portion
            if (moveAngle.Y != 0)
            {
                for (int y = vertTopPixel; y < vertBottomPixel; y++)
                {
                    for (int x = 0; x < sprite.FrameWidth; x++)
                    {
                        if (TileMap.IsWallTileByPixel(
                        new Vector2(newVerticalLocation.X + x, y)))
                        {
                            moveAngle.Y = 0;
                            break;
                        }
                    }
                    if (moveAngle.Y == 0)
                    {
                        break;
                    }
                }
            }

            //Return the possibly modified (collsion - based) movement angle
            return moveAngle;
        }



        #endregion
        //-------------------------------------------------------------------------








        //---------------------------------------------------------------------------------
        #region Update and Draw

        //Update the tank
        public static void Update(GameTime gameTime)
        {

            handleInput(gameTime);  //gathering input for tank and turret            

            //Update the base sprite
            sprite.Update(gameTime);

            clampToWorld(); //Makes sure player stays within the world

            //Position the turret within the center of the tank, wherever it is in the world
            //TurretSprite.WorldLocation = sprite.WorldLocation;
        }

        //Draw the base and turret
        public static void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
            //TurretSprite.Draw(spriteBatch);
        }
        #endregion
        //---------------------------------------------------------------------------------






    }
}
