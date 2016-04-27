using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocumentClassifier.Imaging
{
    //This form displays one or two images side by side for easy debugging purposes
    public partial class ImageView : Form
    {
        public ImageView(Image image1, Image image2=null)
        {
            InitializeComponent();
            this.Visible = true;
            pictureBox1.Image = image1;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            if (image2 != null)
            {
                pictureBox2.Image = image2;
                pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                SetBounds(0, 0, image1.Width + image2.Width, Math.Max(image1.Height,image2.Height));
                pictureBox2.SetBounds(image1.Width, 0, image2.Width, image2.Height);
            }
            else {
                SetBounds(0, 0, image1.Width, image1.Height);
            }
        }

        public void SetImages(Image image1, Image image2 = null)
        {
            InitializeComponent();
            this.Visible = true;
            pictureBox1.Image = image1;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            if (image2 != null)
            {
                pictureBox2.Image = image2;
                pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
                SetBounds(0, 0, image1.Width + image2.Width, Math.Max(image1.Height, image2.Height));
                pictureBox2.SetBounds(image1.Width, 0, image2.Width, image2.Height);
            }
            else {
                SetBounds(0, 0, image1.Width, image1.Height);
            }

            Invalidate();
        }
    }
}
