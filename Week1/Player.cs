using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Week1
{
    class Player : UserControlledSprite
    {
        public bool isMoving;

        public FireBall fireBall;
        enum State
        {
            idleBack, idleFront,
            walkingLeft, walkingRight,
            walkingUp, walkingDown,
            idleRight, idleLeft
        }

        State currentState;

        Point leftStart = new Point(0,3);
        Point leftEnd = new Point(4,3);

        Point rightStart = new Point(0, 4);
        Point rightEnd = new Point(4, 4);

        Point upStart = new Point(0, 2);
        Point upEnd = new Point(4, 2);

        Point downStart = new Point(0, 5);
        Point downEnd = new Point(4, 5);

        Point idleBackStart = new Point(0, 0);
        Point idleBackEnd = new Point(4, 0);

        Point idleFrontStart = new Point(0, 1);
        Point idleFrontEnd = new Point(4, 1);

        Point idleLeftStart = new Point(3, 3);
        Point idleLeftEnd = new Point(4, 3);

        Point idleRightStart = new Point(3, 4);
        Point idleRightEnd = new Point(3, 4);


        public override Vector2 direction
        {
            get
            {
                Vector2 inputDirection = Vector2.Zero;

                // If player pressed arrow keys, move the sprite
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    inputDirection.X -= 1;
                    currentState = State.walkingLeft;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    inputDirection.X += 1;
                    currentState = State.walkingRight;
                }
                else
                {
                    if (currentState == State.walkingLeft)
                    {
                        currentState = State.idleLeft;
                    }
                    else if (currentState == State.walkingRight)
                    {
                        currentState = State.idleRight;
                    }
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    inputDirection.Y -= 1;
                    currentState = State.walkingUp;
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    inputDirection.Y += 1;
                    currentState = State.walkingDown;
                }
                else
                {
                    if (currentState == State.walkingDown)
                    {
                        currentState = State.idleFront;
                    }
                    else if (currentState == State.walkingUp)
                    {
                        currentState = State.idleBack;
                    }
                }

                // If player pressed the gamepad thumbstick, move the sprite
                GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
                if (gamepadState.ThumbSticks.Left.X != 0)
                    inputDirection.X += gamepadState.ThumbSticks.Left.X;
                if (gamepadState.ThumbSticks.Left.Y != 0)
                    inputDirection.Y -= gamepadState.ThumbSticks.Left.Y;

                return inputDirection * speed;
            }
            set { direction = value; }
        }

        public Player(Texture2D textureImage, Vector2 position,
          Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
          Vector2 speed)
          : base(textureImage, position, frameSize, collisionOffset, currentFrame,
          sheetSize, speed)
        {
        }

        //Constrcutor with Framerate
        public Player(Texture2D textureImage, Vector2 position,
            Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize,
            Vector2 speed, int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame)
        {
        }


        public void Fire(Vector2 dir, float vel)
        {
            fireBall.direction = dir * vel;
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            // Move the sprite based on direction
            position += direction;



            // If sprite is off the screen, move it back within the game window
            if (position.X < 0)
                position.X = 0;
            if (position.Y < 0)
                position.Y = 0;
            if (position.X > clientBounds.Width - frameSize.X)
                position.X = clientBounds.Width - frameSize.X;
            if (position.Y > clientBounds.Height - frameSize.Y)
                position.Y = clientBounds.Height - frameSize.Y;


            MouseState ms = Mouse.GetState();
            bool lbDown = false;
            if (ms.LeftButton == ButtonState.Pressed)
            {
                lbDown = true;
            }

            if (lbDown && ms.LeftButton == ButtonState.Released)
            {
                lbDown = false;
                Fire(this.direction, 4);
            }


            Point startPoint = Point.Zero;
            Point endPoint = Point.Zero;

            if (currentState == State.walkingDown)
            {
                startPoint = downStart;
                endPoint = downEnd;
            }
            else if (currentState == State.walkingLeft)
            {
                startPoint = leftStart;
                endPoint = leftEnd;
            }
            else if (currentState == State.walkingRight)
            {
                startPoint = rightStart;
                endPoint = rightEnd;
            }
            else if (currentState == State.walkingUp)
            {
                startPoint = upStart;
                endPoint = upEnd;
            }
            else if (currentState == State.idleBack)
            {
                startPoint = idleBackStart;
                endPoint = idleBackEnd;
            }
            else if (currentState == State.idleFront)
            {
                startPoint = idleFrontStart;
                endPoint = idleFrontEnd;
            }
            else if (currentState == State.idleLeft)
            {
                startPoint = idleLeftStart;
                endPoint = idleLeftEnd;
            }
            else if (currentState == State.idleRight)
            {
                startPoint = idleRightStart;
                startPoint = idleRightEnd;
            }

            base.Play(gameTime, startPoint, endPoint);
        }
    }
}

