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
        public static Image DistortImage(Bitmap image)
        {
            return Crop(Scale(Filter(Speckle(Rotate(Wave(image))))),image);
        }

        private static Bitmap Speckle(Bitmap image)
        {
            IRandomNumberGenerator generator = new GaussianGenerator(0.0f, 2.5f);
            // create filter
            AdditiveNoise filter = new AdditiveNoise(generator);
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Rotate(Bitmap image)
        {
            RotateBicubic filter = new RotateBicubic(.2, true);
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Wave(Bitmap image)
        {
            WaterWave filter = new WaterWave();
            filter.HorizontalWavesCount = 2;
            filter.HorizontalWavesAmplitude = 1;
            filter.VerticalWavesCount = 2;
            filter.VerticalWavesAmplitude = 1;
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Filter(Bitmap image)
        {
            // create filter
            Texturer filter = new Texturer(new CloudsTexture(), 0.05, .98);
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Scale(Bitmap image)
        {
            ResizeBicubic filter = new ResizeBicubic((int)(image.Width*1.03), (int)(image.Height * 1.03));
            // apply the filter
            return filter.Apply(image);
        }

        private static Bitmap Crop(Bitmap image, Bitmap originalImage)
        {
            int offset = (image.Width-originalImage.Width)/2;
            Crop filter = new Crop(new Rectangle(offset, offset, originalImage.Width + offset, originalImage.Height + offset));
            // apply the filter
            return filter.Apply(image);
        }
    }
}
