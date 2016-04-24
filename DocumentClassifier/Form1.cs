using DocumentClasssifier.Neural;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocumentClassifier
{
    public partial class Form1 : Form
    { 
    
        Category root;

        public Form1()
        {
            InitializeComponent();
            Invalidate();

            new Thread(() =>
            {
                RunNetwork();
            }).Start();
        }

        int bubbleOffset = 50;
        int bubbleSize = 50;
        int yOffset = 20;
        int levelOffset = 120;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (root != null)
            {
                var bounds = Size;
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                var font = new Font(DefaultFont.FontFamily, 10, FontStyle.Regular);
                var bubble = new RectangleF(bubbleOffset, (float)(bounds.Height / 2.0 - bubbleSize / 2.0 - yOffset), bubbleSize, bubbleSize);

                PaintBubble(root, 0, font, stringFormat, bubble, e.Graphics);
            }
        }

        public void PaintBubble(Category cat, int level, Font font, StringFormat stringFormat, RectangleF bubble, Graphics g)
        {
            g.FillEllipse(Brushes.DarkSlateBlue, bubble);
            g.DrawString(cat.Name, font, Brushes.White, bubble, stringFormat);

            var thisBubbleOffset = (int)((bubbleOffset*1.0) / cat.Children.Count) * 2;

            int yCentering = (int)(cat.Children.Count * (bubble.Height + thisBubbleOffset-1) / 4);

            int i = 0;
            foreach(var child in cat.Children)
            {
                var newBounds = new RectangleF(bubble.X+levelOffset, bubble.Y + i * 2* thisBubbleOffset - yCentering, bubble.Width, bubble.Height);
                g.DrawLine(Pens.Black, bubble.X+bubbleSize, bubble.Y + bubbleSize / 2, newBounds.X, newBounds.Y+bubbleSize / 2);
                PaintBubble(child, level+1, font, stringFormat, newBounds, g);
                i++;
            }
        }

        const string path = "../../../ClassifierTest/bin/Debug";


        public void RunNetwork()
        {
            var validationSet = TrainingSet.FromDirectory(path+"/trainingdata");
            var trainingSet = TrainingSet.FromDirectory(path +"/generateddata");

            root = trainingSet.Item2;
            Invoke(new MethodInvoker(Invalidate));

            var t = new Trainer();

            var nc = new NetworkCreator();
            var nl = nc.CreateNetworks(root);

            t.Train(nl, trainingSet.Item1, validationSet.Item1);

            var o = t.Run(nl, trainingSet.Item2, path + "/trainingdata/documents/990/tax990_poor.gif");
            o = t.Run(nl, trainingSet.Item2, path + "/trainingdata/documents/1040/2012/tax1040_10.gif");
            o = t.Run(nl, trainingSet.Item2, path + "/trainingdata/documents/1040/2010/tax1040_4.jpg");
            var x = o;
        }
    }
}
