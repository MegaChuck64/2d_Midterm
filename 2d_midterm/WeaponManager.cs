using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace _2d_midterm
{
        static class WeaponManager
        {
            //-------------------------------------------------------------------
            #region declarations

            //Part 1 - declarations
            static public List<Particle> Shots = new List<Particle>();  //List of shots
            static public Texture2D Texture;                            //Texture for sprite sheet
                                                                        //Rectangle to copy images from sprite sheet
            static public Rectangle shotRectangle = new Rectangle(0, 128, 32, 32);
            static public float WeaponSpeed = 600f;         //Weapon speed  - 600 pixels per second
            static private float shotTimer = 0f;            //time accumulator for next shot timing
            static private float shotMinTimer = 0.15f;      //value controlling time for next shot

            // Part 2 - Declararions
            static private float rocketMinTimer = 0.5f;         //min time for next rocket launch
            public enum WeaponType { Normal, Triple, Rocket };  //enumeration for 3 weapon types
            static public WeaponType CurrentWeaponType = WeaponType.Rocket; //create & set weapon
            static public float WeaponTimeRemaining = 30.0f;
            static private float weaponTimeDefault = 30.0f;
            static private float tripleWeaponSplitAngle = 15;   //angle for triple-shot

            // Part 4 - Declarations
            static public List<Sprite> PowerUps = new List<Sprite>();//List of Powerup Sprites
            static private int maxActivePowerups = 5;   //Max # of powerups avail at one time
            static private float timeSinceLastPowerup = 0.0f;   //Timer var to control offerings
            static private float timeBetweenPowerups = 2.0f;    //Actual time to allot between offerings 
            static private Random rand = new Random();          //Random # generator


            #endregion
            //-------------------------------------------------------------------



            //-------------------------------------------------------------------
            #region Part 1- Properties

            //Part 2 - (Updated) Represents the delay/time to wait between shots
            static public float WeaponFireDelay
            {
                get
                {
                    //if using rockets, use the rocket's timer variable
                    if (CurrentWeaponType == WeaponType.Rocket)
                    {
                        return rocketMinTimer;
                    }
                    //else...use the shot timer variable
                    else
                    {
                        return shotMinTimer;
                    }
                }
            }


            //Has the minimum time passed for us to shoot again?
            static public bool CanFireWeapon
            {
                get
                {
                    return (shotTimer >= WeaponFireDelay);
                }
            }

            #endregion
            //-------------------------------------------------------------------




            //-------------------------------------------------------------------
            #region Part 1 - Effects Management Methods

            //Method to add a new shot to the list.  The last param (int) controls
            // whether it is a normal shot of a missile (missile firing code coming soon)
            private static void AddShot(Vector2 location, Vector2 velocity, int frame)
            {
                //Create a new particle representing a "shot"
                Particle shot = new Particle(
                location,
                Texture,
                shotRectangle,
                velocity,
                Vector2.Zero,
                400f,
                120,
                Color.White,
                Color.White);

                //Add an animation frame
                shot.AddFrame(new Rectangle(
                shotRectangle.X + shotRectangle.Width,
                shotRectangle.Y,
                shotRectangle.Width,
                shotRectangle.Height));
                shot.Animate = false;
                shot.Frame = frame;
                shot.RotateTo(velocity);

                //Add this shot to the list of shots
                Shots.Add(shot);
            }

            //Part 3 - Create a larger explosion (for use with rocket collisions)
            private static void createLargeExplosion(Vector2 location)
            {
                //Create a series of explosions at and around the collision point
                EffectsManager.AddLargeExplosion(location);
                EffectsManager.AddLargeExplosion(location + new Vector2(-10, -10));
                EffectsManager.AddLargeExplosion(location + new Vector2(-10, 10));
                EffectsManager.AddLargeExplosion(location + new Vector2(10, 10));
                EffectsManager.AddLargeExplosion(location + new Vector2(10, -10));
            }


            #endregion
            //-------------------------------------------------------------------




            //-------------------------------------------------------------------
            #region Weapons Management Methods

            //Part 2 (Updated)
            //When we call FireWeapon, we are given the starting location and a velocity
            // for the shot, then the shot is added and the shot timer is reset to zero
            // in order to keep us from shooting another shot right away.
            public static void FireWeapon(Vector2 location, Vector2 velocity)
            {
                switch (CurrentWeaponType)
                {
                    case WeaponType.Normal:
                        //fire normal shot down center
                        AddShot(location, velocity, 0);
                        break;
                    case WeaponType.Triple:
                        //Fire shot down the center and...
                        AddShot(location, velocity, 0);
                        //Two shots angled to the sides
                        float baseAngle = (float)Math.Atan2(velocity.Y, velocity.X);
                        float offset = MathHelper.ToRadians(tripleWeaponSplitAngle);
                        AddShot(location,
                            new Vector2((float)Math.Cos(baseAngle - offset),
                                (float)Math.Sin(baseAngle - offset)) * velocity.Length(), 0);
                        AddShot(location,
                            new Vector2((float)Math.Cos(baseAngle + offset),
                                (float)Math.Sin(baseAngle + offset)) * velocity.Length(), 0);
                        break;
                    case WeaponType.Rocket:
                        //fire the rocket
                        AddShot(location, velocity, 1);
                        break;
                }

                shotTimer = 0.0f;   //Reset shot counter
            }



            //Part 2 - Check to see if the time has run out for our upgraded weapon
            private static void checkWeaponUpgradeExpire(float elapsed)
            {
                //if we are not using the normal weapon...
                if (CurrentWeaponType != WeaponType.Normal)
                {
                    //deduct the elapsed time since last update
                    WeaponTimeRemaining -= elapsed;
                    //if the remaining time variable reaches zero or below...
                    if (WeaponTimeRemaining <= 0)
                    {
                        //reset the weapon type back to normal
                        CurrentWeaponType = WeaponType.Normal;
                    }
                }
            }


            // Part 4 - Function to spawn powerups on the screen
            private static void tryToSpawnPowerup(int x, int y, WeaponType type)
            {
                //if we have reached or exceeded the max powerup offerings, exit function
                if (PowerUps.Count >= maxActivePowerups)
                {
                    return;
                }

                //Get the rectangle of the tile in the specified location in the world
                Rectangle thisDestination = TileMap.SquareWorldRectangle(new Vector2(x, y));

                //Loop thru the list of power ups...
                foreach (Sprite powerup in PowerUps)
                {
                    //If the new destination already has a powerup, return out of this method
                    if (powerup.WorldRectangle == thisDestination)
                    {
                        return;
                    }
                }

                //if we got here, we can assume no powerup exists in the location
                //
                //if the tile is not a wall tile...
                if (!TileMap.IsWallTile(x, y))
                {
                    //Create a new powerup sprite
                    Sprite newPowerup = new Sprite(
                        new Vector2(thisDestination.X, thisDestination.Y),
                        Texture,
                        new Rectangle(64, 128, 32, 32),
                        Vector2.Zero);

                    newPowerup.Animate = false;         //set animate to false
                    newPowerup.CollisionRadius = 14;    //Set a circular collision area
                    newPowerup.AddFrame(new Rectangle(96, 128, 32, 32)); //add a frame

                    //If it is a rocket, then use the second frame
                    if (type == WeaponType.Rocket)
                    {
                        newPowerup.Frame = 1;
                    }
                    //else...it must be the triple shot

                    PowerUps.Add(newPowerup);       //Add the powerup sprite to the list
                    timeSinceLastPowerup = 0.0f;    //reset the timer for next powerup
                }
            }


            //Part 4 - If enough time has passed,create a random powerup and location
            // and try to offer it at the specified location.
            private static void checkPowerupSpawns(float elapsed)
            {
                //Update the timer for last powerup spawn
                timeSinceLastPowerup += elapsed;

                //if enough time has passed...
                if (timeSinceLastPowerup >= timeBetweenPowerups)
                {
                    //Create a Triple-Shot weapon to start with
                    WeaponType type = WeaponType.Triple;

                    //Create a random number between 0 and 1
                    //if the random # is equal to one, change weapon to Rocket
                    if (rand.Next(0, 2) == 1)
                    {
                        type = WeaponType.Rocket;   //Set weapon to a rocket
                    }

                    //Attempt to spaw the powerup using a random column and row
                    tryToSpawnPowerup(
                    rand.Next(0, TileMap.MapWidth),
                    rand.Next(0, TileMap.MapHeight),
                    type);
                }
            }

            #endregion
            //-------------------------------------------------------------------




            //-------------------------------------------------------------------
            #region Part 3 - Collision Detection
            private static void checkShotWallImpacts(Sprite shot)
            {
                //if shot has expired, no need to go any further, exit this method
                if (shot.Expired)
                {
                    return;
                }

                //Use the shot's world coord, to get the Square/Tile coord, then
                // check to see if it is a wall tile...
                if (TileMap.IsWallTile(TileMap.GetSquareAtPixel(shot.WorldCenter)))
                {
                    //if it is a wall tile, then...
                    shot.Expired = true;    //Expire the shot
                    if (shot.Frame == 0)    //check to see if the shot is single/triple
                    {
                        //Add a new Spark effect at the location of the shot
                        EffectsManager.AddSparksEffect(shot.WorldCenter, shot.Velocity);
                    }
                    //else (if not a normal/triple shot), then it must be a rocket
                    // so create a larger explosion
                    else
                    {
                        createLargeExplosion(shot.WorldCenter);  //make a big explosion
                    }
                }
            }

            //Part 4 - Handles the collision between the Player and possible Powerups
            private static void checkPowerupPickups()
            {
                // Loop thru the powerups list (backwards)
                for (int x = PowerUps.Count - 1; x >= 0; x--)
                {
                    //if the player collides with the current powerup...
                    if (Player.BaseSprite.IsCircleColliding(
                    PowerUps[x].WorldCenter,
                    PowerUps[x].CollisionRadius))
                    {
                        //Check which frame the powerup is using...
                        switch (PowerUps[x].Frame)
                        {
                            //If frame 0, then set weapon to Triple Shot
                            case 0:
                                CurrentWeaponType = WeaponType.Triple;
                                break;
                            //If frame 1, then set weapon to Rocket
                            case 1:
                                CurrentWeaponType = WeaponType.Rocket;
                                break;
                        }
                        //Reset the countdown timer
                        WeaponTimeRemaining = weaponTimeDefault;
                        //Remove this powerup from the time map
                        PowerUps.RemoveAt(x);
                    }
                }
            }


            #endregion
            //-------------------------------------------------------------------



            //-------------------------------------------------------------------
            #region Part 1 - Update and Draw

            static public void Update(GameTime gameTime)
            {

                //Accumulate elapsed time between updates to manage delays for shots
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                shotTimer += elapsed;

                //Part 2 - Check to see if any upgrades need to expire
                checkWeaponUpgradeExpire(elapsed);


                // Part 3 (Modified)
                //Loop thru the existing shots (backwards to easily removed expired shots)
                for (int x = Shots.Count - 1; x >= 0; x--)
                {
                    Shots[x].Update(gameTime);          //Update the shot
                    checkShotWallImpacts(Shots[x]);     //Check to see if it hit the wall
                    if (Shots[x].Expired)               //if it has expired, remove it
                    {
                        Shots.RemoveAt(x);
                    }
                }

                //---------------------------------------
                //Part 4 - Place Powerups on the board
                checkPowerupSpawns(elapsed);
                //Part 4 - Look to see if player should get powerup
                checkPowerupPickups();
                //---------------------------------------

            }


            //Loops thru the list of Shots and Powerups and draws each existing one.
            static public void Draw(SpriteBatch spriteBatch)
            {
                //Draw existing Shots
                foreach (Particle sprite in Shots)
                {
                    sprite.Draw(spriteBatch);
                }

                //Draw existing Powerups
                foreach (Sprite sprite in PowerUps)
                {
                    sprite.Draw(spriteBatch);
                }
            }

            #endregion
            //-------------------------------------------------------------------





        }
    }
