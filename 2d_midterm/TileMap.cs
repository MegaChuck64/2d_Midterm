using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Implement XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace _2d_midterm
{
    static class TileMap
    {
        //-----------------------------------------------------
        #region Declarations
        public const int TileWidth = 32;        //Width of tile
        public const int TileHeight = 32;       //Height of tile
        public const int MapWidth = 50;         //Map is 50 tiles wide
        public const int MapHeight = 50;        //Map is 50 tiles high/deep

        //There are 4 floor tile sprites that we can walk on (0 thru 3)
        public const int FloorTileStart = 0;    //lowest floor index
        public const int FloorTileEnd = 0;      //greatest floor index

        //There are 4 wall tile sprite that we cannot go on or thru (4 thru 7)
        public const int WallTileStart = 1;    //lowest walk index
        public const int WallTileEnd = 1;       //greatest wall index

        static private Texture2D texture;       //sprite sheet

        //List of rectangles for each of the various tiles in the spritesheet
        static private List<Rectangle> tiles = new List<Rectangle>();

        //Two-dimensional array to map out the squares that make up the tile map
        static private int[,] mapSquares = new int[MapWidth, MapHeight];

        //Random # generator
        static private Random rand = new Random();

        #endregion
        //---------------------------------------------------------



        //------------------------------------------------------------
        #region  Initialization

        //Function that uses rectangles to copy specific pieces of the sprite sheet
        // to gather the different tiles we want to draw.
        static public void Initialize(Texture2D tileTexture)
        {
            //point to the passed in spritesheet
            texture = tileTexture;

            tiles.Clear();  //Clear the list

            //Grab the 8 different tiles for our map
            tiles.Add(new Rectangle(0, 32, TileWidth, TileHeight));
            tiles.Add(new Rectangle(32, 32, TileWidth, TileHeight));


            //--------------------------------------------------------------------------
            // Generate the random map
            GenerateRandomMap();

            //The following code can be remarked out now we have GenerateRandomMap()

            ////Loop through each of the squares in the map...    
            //for (int x = 0; x < MapWidth; x++)
            //    for (int y = 0; y < MapHeight; y++)
            //    {
            //        //Default the specified square to the default floor tile to start (0)
            //        mapSquares[x, y] = FloorTileStart;
            //    }
            //--------------------------------------------------------------------------

        }

        #endregion
        //------------------------------------------------------------




        //------------------------------------------------------------
        #region Information about Map Squares

        //Get the X position of the tile map
        static public int GetSquareByPixelX(int pixelX)
        {
            //by dividing the x position by tile width, we get the horiz. tile position
            return pixelX / TileWidth;
        }


        //Get the Y position of the tile map
        static public int GetSquareByPixelY(int pixelY)
        {
            //by dividing the y position by tile height, we get the vert. tile position
            return pixelY / TileHeight;
        }


        //Get the Tile Map square coord, based on the pixel coord.
        static public Vector2 GetSquareAtPixel(Vector2 pixelLocation)
        {
            //Use the two methods above to get both pieces of the coord.
            return new Vector2(
                GetSquareByPixelX((int)pixelLocation.X),
                GetSquareByPixelY((int)pixelLocation.Y));
        }


        //get the pixel coordinate at the center of a specified square/tile
        static public Vector2 GetSquareCenter(int squareX, int squareY)
        {
            return new Vector2(
                (squareX * TileWidth) + (TileWidth / 2),        //move to tile, plus half width
                (squareY * TileHeight) + (TileHeight / 2));     //move to tile, plus half height
        }


        //Overloaded method that takes coords via a Vector2
        static public Vector2 GetSquareCenter(Vector2 square)
        {
            return GetSquareCenter((int)square.X, (int)square.Y);
        }


        //Returns a tile-sized rectangle positioned x squares to the right, and y squares down
        static public Rectangle SquareWorldRectangle(int x, int y)
        {
            return new Rectangle(
                x * TileWidth,
                y * TileHeight,
                TileWidth,
                TileHeight);
        }


        //overloaded method that takes the coords via a Vector2
        static public Rectangle SquareWorldRectangle(Vector2 square)
        {
            return SquareWorldRectangle((int)square.X, (int)square.Y);
        }


        //Returns the rectangle for a a specified square/tile, but 
        //  using localized coords (in viewable area)
        static public Rectangle SquareScreenRectangle(int x, int y)
        {
            return Camera.Transform(SquareWorldRectangle(x, y));
        }

        //overloaded version that takes a Vector2
        static public Rectangle SquareScreenRectangle(Vector2 square)
        {
            return SquareScreenRectangle((int)square.X, (int)square.Y);
        }

        #endregion
        //------------------------------------------------------------------



        //------------------------------------------------------------------
        #region Information about Map Tiles


        //Returns the Tile# at a specified location
        static public int GetTileAtSquare(int tileX, int tileY)
        {
            //If the Map Square coordinates are legitimately in the map...
            if ((tileX >= 0) && (tileX < MapWidth) && (tileY >= 0) && (tileY < MapHeight))
            {
                //Return the numeric type of tile at that location
                //In essence, telling us which type of land or wall is in that position
                return mapSquares[tileX, tileY];
            }
            //else...
            else
            {
                return -1;  //return -1, signifying the coords were not found on the map
            }
        }


        //Sets the Tile at a specified location to a new tile value that is passed in
        static public void SetTileAtSquare(int tileX, int tileY, int tile)
        {
            //If the Map Square coordinates are legitimately in the...
            if ((tileX >= 0) && (tileX < MapWidth) && (tileY >= 0) && (tileY < MapHeight))
            {
                mapSquares[tileX, tileY] = tile;    //set the specified coord to the tile #
            }
        }


        //Using some helper functions, it will take the pixel coordinates and find the
        //tile map/square coordinates, then call another existing function to get the 
        // tile # at the calculated location.
        static public int GetTileAtPixel(int pixelX, int pixelY)
        {
            return GetTileAtSquare(GetSquareByPixelX(pixelX), GetSquareByPixelY(pixelY));
        }


        //Overloaded version of above function that recieves a Vector2 insted of separate X & Y
        static public int GetTileAtPixel(Vector2 pixelLocation)
        {
            return GetTileAtPixel((int)pixelLocation.X, (int)pixelLocation.Y);
        }


        //Function to determine if a tile at the specified coordinates is a wall tile
        static public bool IsWallTile(int tileX, int tileY)
        {
            //Get the tile type from the specified square on the map
            int tileIndex = GetTileAtSquare(tileX, tileY);

            //if the square/tile coordinate does not exist...
            if (tileIndex == -1)
            {
                return false;   //return false
            }

            //else...
            return tileIndex >= WallTileStart;  //return true if the tile value is a wall type
        }


        //Overloaded version of above that takes a Vector2
        static public bool IsWallTile(Vector2 square)
        {
            return IsWallTile((int)square.X, (int)square.Y);
        }


        //Receiving pixel coords, we get the tile location, then use this info to
        // find out if it is a wall tile
        static public bool IsWallTileByPixel(Vector2 pixelLocation)
        {
            return IsWallTile(
            GetSquareByPixelX((int)pixelLocation.X),
            GetSquareByPixelY((int)pixelLocation.Y));
        }

        #endregion
        //------------------------------------------------------------------




        //------------------------------------------------------------------
        #region Map Generation

        static public void GenerateRandomMap()
        {
            //Variable to control the ration of chances of a random tile being a wall
            int wallChancePerSquare = 10;
            //Generate a random # for both a Floor tile and a Wall tile
            int floorTile = rand.Next(FloorTileStart, FloorTileEnd + 1);
            int wallTile = rand.Next(WallTileStart, WallTileEnd + 1);

            //Loop thru each of the squares in the map (By columns and rows)...
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    //Set the initial value of the square to the floor tile
                    mapSquares[x, y] = floorTile;

                    //if the tile is a border of the world, make it a wall piece
                    if ((x == 0) || (y == 0) || (x == MapWidth - 1) || (y == MapHeight - 1))
                    {
                        mapSquares[x, y] = wallTile;
                        continue;   //move onto the next loop iteration
                    }

                    //if the tile is bordering the inside of the border, they will remain 
                    // floor tiles
                    if ((x == 1) || (y == 1) || (x == MapWidth - 2) || (y == MapHeight - 2))
                    {
                        continue;   //move onto the next loop iteration
                    }

                    //if we got here, we are not on the borders, by deductive reasoning
                    //Generate a random # between 1 and 100.  If the # is 10 or less, then
                    // set the tile to a wall tile.
                    if (rand.Next(0, 100) <= wallChancePerSquare)
                    {
                        mapSquares[x, y] = wallTile;
                    }
                }
            }
        }

        #endregion
        //------------------------------------------------------------------




        //------------------------------------------------------------------
        #region Drawing

        static public void Draw(SpriteBatch spriteBatch)
        {
            //Get the top-left Square coord by using the camera's top-left position
            int startX = GetSquareByPixelX((int)Camera.Position.X);
            int startY = GetSquareByPixelY((int)Camera.Position.Y);

            //Get the bottom-right Square coord by using the camera's bottom-right position
            int endX = GetSquareByPixelX((int)Camera.Position.X +
            Camera.ViewPortWidth);
            int endY = GetSquareByPixelY((int)Camera.Position.Y +
            Camera.ViewPortHeight);


            //Loop thru the tiles seen within the camera...
            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                {
                    //If the tile coord are legitimately within the map...
                    if ((x >= 0) && (y >= 0) && (x < MapWidth) && (y < MapHeight))
                    {
                        //Draw the tile
                        spriteBatch.Draw(
                        texture,                        //spritesheet
                        SquareScreenRectangle(x, y),    //Destination rectangle
                        tiles[GetTileAtSquare(x, y)],   //Source rectangle
                        Color.White);                   //Light color
                    }
                }
        }

        #endregion
        //------------------------------------------------------------------


    }
}
