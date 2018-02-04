using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace _2d_midterm
{
    public static class Camera
    {
        //------------------------------------------------------------------
        #region Declarations
        //Camera's position in the world (Upper-Left corner of the camera)
        private static Vector2 position = Vector2.Zero;
        //Size of the viewing area (pixels to the right and downward)
        private static Vector2 viewPortSize = Vector2.Zero;
        //Rectangle the Size of the whole world (visible & out of sight)
        private static Rectangle worldRectangle = new Rectangle(0, 0, 0, 0);
        #endregion
        //------------------------------------------------------------------


        //------------------------------------------------------------------
        #region Properties

        public static Vector2 Position
        {
            get { return position; }
            set
            {
                //Makes sure that the position keeps our camera within the world boundaries
                // This is calculated by taking the width/height of the camera Viewport and
                // subtracting it from the World's width/height to reach the right/bottom 
                // boundaries as max.  Min is the leftmost/topmost part of the world.
                position = new Vector2(
                    MathHelper.Clamp(value.X, worldRectangle.X,
                    worldRectangle.Width - ViewPortWidth),
                    MathHelper.Clamp(value.Y, worldRectangle.Y,
                    worldRectangle.Height - ViewPortHeight));
            }
        }


        //Sets/Gets the World's dimensions
        public static Rectangle WorldRectangle
        {
            get { return worldRectangle; }
            set { worldRectangle = value; }
        }


        public static int ViewPortWidth
        {
            get { return (int)viewPortSize.X; }     // returns the width of the viewport (X)
            set { viewPortSize.X = value; }         // sets the width of the viewport (X)
        }


        public static int ViewPortHeight
        {
            get { return (int)viewPortSize.Y; }     // returns the height of the viewport (Y)
            set { viewPortSize.Y = value; }         // sets the height of the viewport (Y)
        }


        public static Rectangle ViewPort
        {
            get
            {
                //Returns a rectangle positioned at coord X,Y, with the dimensions 
                //  of the viewport
                return new Rectangle(
                (int)Position.X, (int)Position.Y,
                ViewPortWidth, ViewPortHeight);
            }
        }

        #endregion
        //------------------------------------------------------------------




        //------------------------------------------------------------------
        #region Public Methods

        //Adjust the camera position by telling it how far to move horiz & vertically
        public static void Move(Vector2 offset)
        {
            Position += offset;
        }


        //Is an object within our Viewport's rectangle...is it visible?
        //If not visible, we will not need to draw it.
        public static bool ObjectIsVisible(Rectangle bounds)
        {
            return (ViewPort.Intersects(bounds));
        }


        //Returns the position of an object in relation to the viewable screen
        // rather than in relation to the whole world
        public static Vector2 Transform(Vector2 point)
        {
            return point - position;
        }


        //Returns the rectangle of an object in relation to the viewable screen
        // rather than in relation to the whole world
        public static Rectangle Transform(Rectangle rectangle)
        {
            return new Rectangle(
            rectangle.Left - (int)position.X,
            rectangle.Top - (int)position.Y,
            rectangle.Width,
            rectangle.Height);
        }


        #endregion
        //------------------------------------------------------------------
    }
}
