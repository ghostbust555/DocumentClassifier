//using AForge.Imaging;
using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AForge.Math.Random;
using AForge;
using AForge.Imaging.Textures;

namespace DocumentClassifier.Imaging
{
    public class ImageDistorter
    {
        static Random r = new Random();

        public static Image DistortImage(Bitmap image)
        {
            Rectangle originalBounds = new Rectangle(0, 0, image.Width, image.Height);

            if (r.NextDouble() > .5)
                image = Crop(Scale(Wave(image)), originalBounds);

            if (r.NextDouble() > .5)
                image = Crop(Scale(Rotate(image)), originalBounds);

            if (r.NextDouble() > .5)
                image = Speckle(image);

            if (r.NextDouble() > .5)
                image = Filter(image);

            if (r.NextDouble() > .5)
                image = Smooth(image);

            if (r.NextDouble() > .2)
                image = Speckle(image);

            if (r.NextDouble() > .3)
                image = Blur(image);

            return image;
        }

        private static Bitmap Speckle(Bitmap image)
        {
            IRandomNumberGenerator generator = new GaussianGenerator(0f, 5f);
             
            // create filter
            AdditiveNoise filter = new AdditiveNoise(generator);
            // apply the filter

            return filter.Apply(image);
        }

        private static Bitmap Rotate(Bitmap image)
        {
            var angle = r.NextDouble()*.8 - .4;
            RotateBicubic filter = new RotateBicubic(angle, false);
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Sharpen(Bitmap image)
        {
            // create filter
            Sharpen filter = new Sharpen();
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Smooth(Bitmap image)
        {
            // create filter
            var filter = new ConservativeSmoothing();
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Blur(Bitmap image)
        {
            // create filter
            var filter = new GaussianBlur(r.NextDouble()/2.0,3);
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Wave(Bitmap image)
        {
            var h = (int)(r.NextDouble() * 3);
            var v = (int)(r.NextDouble() * 3);

            WaterWave filter = new WaterWave();
            filter.HorizontalWavesCount = h;
            filter.HorizontalWavesAmplitude = 1;
            filter.VerticalWavesCount = v;
            filter.VerticalWavesAmplitude = 1;
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Filter(Bitmap image)
        {
            IRandomNumberGenerator generator = new GaussianGenerator(0.05f, 0.4f);
            
            // create filter
            Texturer filter = new Texturer(new CloudsTexture(), generator.Next(), .98);
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Scale(Bitmap image)
        {
            IRandomNumberGenerator generator = new GaussianGenerator(1.05f, 0.1f);

            ResizeBicubic filter = new ResizeBicubic((int)(image.Width*1.07), (int)(image.Height * 1.07));
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Crop(Bitmap image, Rectangle originalBounds)
        {
            int offset = (int)((image.Width-originalBounds.Width)/2.0 + .5);
            Crop filter = new Crop(new Rectangle(offset, offset, originalBounds.Width + offset, originalBounds.Height + offset));
            // apply the filter
            return filter.Apply(image);
        }
    }
}
