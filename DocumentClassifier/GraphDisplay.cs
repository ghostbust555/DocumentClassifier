using DocumentClassifier.Imaging;
using DocumentClasssifier.Neural;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocumentClassifier
{
    public partial class GraphDisplay : Form
    { 
    
        Category root;

        public GraphDisplay()
        {
            InitializeComponent();
            Invalidate();

            new Thread(() =>
            {
                GenerateTrainingData();
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

        public void GenerateTrainingData()
        {
            var ts = new List<TrainingSet>();

            var root = new Category("documents");
            var doc_1040 = root.AddSubcategory("1040");
            var doc_990 = root.AddSubcategory("990");

            var doc_1040_2012 = doc_1040.AddSubcategory("2012");
            var doc_1040_2010 = doc_1040.AddSubcategory("2010");
            var doc_1040_other = doc_1040.AddSubcategory("other");

            ts.Add(new TrainingSet(path + "/trainingdata/documents/1040/other/tax1040.gif", doc_1040_other));
            ts.Add(new TrainingSet(path + "/trainingdata/documents/1040/other/tax1040_2.png", doc_1040_other));
            ts.Add(new TrainingSet(path + "/trainingdata/documents/1040/2012/tax1040_3.gif", doc_1040_2012));
            ts.Add(new TrainingSet(path + "/trainingdata/documents/1040/2010/tax1040_4.jpg", doc_1040_2010));
            ts.Add(new TrainingSet(path + "/trainingdata/documents/1040/other/tax1040_7.gif", doc_1040_other));
            ts.Add(new TrainingSet(path + "/trainingdata/documents/1040/2010/tax1040_9.jpg", doc_1040_2010));
            ts.Add(new TrainingSet(path + "/trainingdata/documents/1040/2012/tax1040_10.gif", doc_1040_2012));

            ts.Add(new TrainingSet(path + "/trainingdata/documents/990/tax990.jpg", doc_990));
            ts.Add(new TrainingSet(path + "/trainingdata/documents/990/tax990_2.jpg", doc_990));
            ts.Add(new TrainingSet(path + "/trainingdata/documents/990/tax990_3.jpg", doc_990));
            ts.Add(new TrainingSet(path + "/trainingdata/documents/990/tax990_4.jpg", doc_990));
            ts.Add(new TrainingSet(path + "/trainingdata/documents/990/tax990_5.jpg", doc_990));
            ts.Add(new TrainingSet(path + "/trainingdata/documents/990/tax990_6.jpg", doc_990));

            int count = 0;

            foreach (var td in ts)
            {
                var name = path + "/generateddata";
                var cat = td.Category;

                var paths = new List<string>();

                if (!Directory.Exists(name))
                    Directory.CreateDirectory(name);

                while (cat != null)
                {
                    paths.Add("/" + cat.Name);
                    cat = cat.Parent;
                }

                paths.Reverse();

                foreach (var p in paths)
                {
                    name += p;
                    if (!Directory.Exists(name))
                        Directory.CreateDirectory(name);
                }

                for (int i = 0; i < 10; i++)
                {
                    var im = ImageDistorter.DistortImage((Bitmap)Image.FromFile(td.ImageFile));
                    using (MemoryStream memory = new MemoryStream())
                    {
                        using (FileStream fs = new FileStream(name + "/" + count + ".jpg", FileMode.Create, FileAccess.ReadWrite))
                        {
                            im.Save(memory, ImageFormat.Jpeg);
                            byte[] bytes = memory.ToArray();
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }

                    count++;
                }
            }
        }

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
