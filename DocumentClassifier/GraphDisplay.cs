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
        //This is the root category of the classification tree
        Category root;
        List<Result> results = null;

        public GraphDisplay()
        {
            //Initialize UI
            InitializeComponent();
            //Invalidate UI to recall OnPaint
            Invalidate();

            //Start a new thread to generate training data, create the classification tree and train the networks
            //new Thread(() =>
            //{
                
                ////Generate distorted images for training
                //GenerateTrainingData();
                ////Generate the classification tree and train it recursively
                RunNetwork();

            //}).Start();
        }


        int bubbleOffset = 50; //Tree bubble spacing
        int bubbleSize = 50; //tree bubble size
        int yOffset = 20; // total window height offset
        int levelOffset = 120; //space between parent and children in the tree

        //This is the main UI drawing method. Called whenever the form is invalidated
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (root != null)
            {
                var bounds = Size; //set = windows size

                //create a new string formatter for center alignment of text
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                var font = new Font(DefaultFont.FontFamily, 10, FontStyle.Regular);

                //calculate the root bubble bounds
                var bubble = new RectangleF(bubbleOffset, (float)(bounds.Height / 2.0 - bubbleSize / 2.0 - yOffset), bubbleSize, bubbleSize);

                //call the paintBubble recursive function to draw the tree
                PaintBubble(root, font, stringFormat, bubble, e.Graphics);
            }
        }

        //Draws the bubble for a current node and then recursively calls its children
        public void PaintBubble(Category cat, Font font, StringFormat stringFormat, RectangleF bubble, Graphics g)
        {
            var brush = Brushes.DarkSlateBlue;
            if (results != null && results.Select(x=>x.Category).Where(x=>x.Name == cat.Name).Count() > 0)
            {
                brush = Brushes.Red;
            }

            //draw the bubble and string
            g.FillEllipse(brush, bubble);
            g.DrawString(cat.Name, font, Brushes.White, bubble, stringFormat);

            //calculate the next layers offset
            var thisBubbleOffset = (int)((bubbleOffset*1.0) / cat.Children.Count) * 2;
            int yCentering = (int)(cat.Children.Count * (bubble.Height + thisBubbleOffset-1) / 4);

            //recursively call all child nodes
            int i = 0;
            foreach(var child in cat.Children)
            {
                var newBounds = new RectangleF(bubble.X+levelOffset, bubble.Y + i * 2* thisBubbleOffset - yCentering, bubble.Width, bubble.Height);
                g.DrawLine(Pens.Black, bubble.X+bubbleSize, bubble.Y + bubbleSize / 2, newBounds.X, newBounds.Y+bubbleSize / 2);
                PaintBubble(child, font, stringFormat, newBounds, g);
                i++;
            }
        }

        //location of the trainingdata and generateddata folders
        const string path = "../../../ClassifierTest/bin/Debug";

        //Take categroized data and generate random distortions on the dataset. This will generate 10 distorted images per input image
        public void GenerateTrainingData()
        {
            //create a new training set
            var ts = new List<TrainingSet>();

            //Create a root node with lable documents and give it two children, 1040 and 990
            var root = new Category("documents");
            var doc_1040 = root.AddSubcategory("1040");
            var doc_990 = root.AddSubcategory("990");

            //further define tree structure
            var doc_1040_2012 = doc_1040.AddSubcategory("2012");
            var doc_1040_2010 = doc_1040.AddSubcategory("2010");
            var doc_1040_other = doc_1040.AddSubcategory("other");

            //add data to the tree
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

            //loop through the tree and generate 10 distorted vesrions of every image
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
                    //Distort the image and save it to disk
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

        //Generate the classification tree and start training it recursively
        public void RunNetwork()
        {
            var validationSet = TrainingSet.FromDirectory(path+"/trainingdata");
            var trainingSet = TrainingSet.FromDirectory(path +"/generateddata");

            root = trainingSet.Item2;
            Invalidate();
            //Invoke(new MethodInvoker(Invalidate));

            var t = new Trainer();

            var nc = new NetworkCreator();
            var nl = nc.CreateNetworks(root);

            t.Train(nl, trainingSet.Item1, validationSet.Item1);
                       

            new Thread(() =>
            {
                var thisImagePath = path + "/trainingdata/documents/990/tax990.jpg";
                results = t.Run(nl, trainingSet.Item2, thisImagePath);
                Invalidate();
                var iv = new ImageView(Image.FromFile(thisImagePath));
                iv.Visible = false;
                iv.ShowDialog();

                thisImagePath = path + "/trainingdata/documents/1040/2012/tax1040_10.gif";
                results = t.Run(nl, trainingSet.Item2, thisImagePath);
                Invalidate();
                iv = new ImageView(Image.FromFile(thisImagePath));
                iv.Visible = false;
                iv.ShowDialog();

                thisImagePath = path + "/trainingdata/documents/1040/2010/tax1040_4.jpg";
                results = t.Run(nl, trainingSet.Item2, thisImagePath);
                Invalidate();
                iv = new ImageView(Image.FromFile(thisImagePath));
                iv.Visible = false;
                iv.ShowDialog();
            }).Start();
        }
    }
}
