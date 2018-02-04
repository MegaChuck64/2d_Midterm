using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;


namespace Week1
{
    class BouncingSprite : AutomatedSprite
    {

        SoundEffectInstance[] soundInstances = new SoundEffectInstance[3];

        public BouncingSprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, SoundEffect[] soundEffects)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed)
        {
            for (int i = 0; i < soundEffects.Length; i++)
            {
                soundInstances[i] = soundEffects[i].CreateInstance();
            }
        }


        public BouncingSprite(Texture2D textureImage, Vector2 position, Point frameSize,
            int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, SoundEffect[] soundEffects, int millisecondsPerFrame)
            : base(textureImage, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame)
        {
            for (int i = 0; i < soundEffects.Length; i++)
            {
                soundInstances[i] = soundEffects[i].CreateInstance();
            }
        }


        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {

            position += direction;

            Random random = new Random();

            if (position.X >= clientBounds.Width - frameSize.X || position.X <= 0)
            {
                int r = random.Next(0,soundInstances.Length);
                soundInstances[r].Play();
                speed.X *= -1;
            }

            if (position.Y >= clientBounds.Height - frameSize.Y || position.Y <= 0)
            {
                int r = random.Next(0, soundInstances.Length);
                soundInstances[r].Play();
                speed.Y *= -1;
            }
            base.Update(gameTime, clientBounds);
        }
    }
}

