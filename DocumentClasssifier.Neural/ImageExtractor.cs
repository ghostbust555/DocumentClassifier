using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentClasssifier.Neural
{
    //extracts the feature vector from an image by resizing it to 200x200 pixels and then concateninting the 2d array into a 1d array row by row
    public class ImageExtractor
    {
        public const int IMAGE_SIZE = 200;
        const int NO_OF_CHANNELS = 1;

        public static double[] ExtractImageFeatures(string imageFile)
        {
            var image = GrayScale(ResizeImage(IMAGE_SIZE, IMAGE_SIZE, imageFile));

            var pd = GetPixelArray(image);
            var dpd = new double[pd.Length];

            for(int i = 0; i < pd.Length; i++)
            {
                dpd[i] = pd[i] / 255.0;
            }

            return dpd;
        }

        public static byte[] GetPixelArray(Bitmap img)
        {
            byte[] output = new byte[IMAGE_SIZE * IMAGE_SIZE];
            BitmapData data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, img.PixelFormat);
            int bi = 0;

            unsafe
            {
                byte* ptr = (byte*)data.Scan0;
                for (int j = 0; j < data.Height; j++)
                {
                    byte* scanPtr = ptr + (j * data.Stride);
                    for (int i = 0; i < data.Width; i++, scanPtr += NO_OF_CHANNELS)
                    {
                        for (int m = 0; m < NO_OF_CHANNELS; m++)
                        {
                            output[bi]=*scanPtr; // value of each channel
                            bi++;
                        }
                    }
                }
            }

            img.UnlockBits(data);

            return output;
        }

        public static Bitmap GrayScale(Bitmap Bmp)
        {
            int rgb;
            Color c;

            for (int y = 0; y < Bmp.Height; y++)
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    rgb = (int)((c.R + c.G + c.B) / 3);
                    Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            return Bmp;
        }

        public static Bitmap ResizeImage(int newWidth, int newHeight, string stPhotoPath)
        {
            Image imgPhoto = Image.FromFile(stPhotoPath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            //Consider vertical pics
            if (sourceWidth < sourceHeight)
            {
                int buff = newWidth;

                newWidth = newHeight;
                newHeight = buff;
            }

            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = ((float)newWidth / (float)sourceWidth);
            nPercentH = ((float)newHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((newWidth -
                          (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((newHeight -
                          (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                          PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                         imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return bmPhoto;
        }
    }
}
