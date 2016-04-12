using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DocumentClasssifier.Neural;
using System.Collections.Generic;
using System.Drawing;
using DocumentClassifier.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace ClassifierTest
{
    [TestClass]
    public class UnitTestDataCreator
    {
        [TestMethod]
        public void CreateDistortedImage()
        {
            var im = "trainingdata/documents/1040/other/tax1040.gif";
            var f = new ImageView((Bitmap)Image.FromFile(im), ImageDistorter.DistortImage((Bitmap)Image.FromFile(im)));
            
            f.Show();
            Application.Run(f);     
        }

        [TestMethod]
        public void GenerateTrainingData()
        {
            var ts = new List<TrainingSet>();

            var root = new Category("documents");
            var doc_1040 = root.AddSubcategory("1040");
            var doc_990 = root.AddSubcategory("990");

            var doc_1040_2012  = doc_1040.AddSubcategory("2012");
            var doc_1040_2010 = doc_1040.AddSubcategory("2010");
            var doc_1040_other = doc_1040.AddSubcategory("other");

            ts.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040.gif", doc_1040_other));
            ts.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040_2.png", doc_1040_other));
            ts.Add(new TrainingSet("trainingdata/documents/1040/2012/tax1040_3.gif", doc_1040_2012));
            ts.Add(new TrainingSet("trainingdata/documents/1040/2010/tax1040_4.jpg", doc_1040_2010));
         //   ts.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040_5.gif", doc_1040_other));
          //  ts.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040_6.gif", doc_1040_other));
            ts.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040_7.gif", doc_1040_other));
         //   ts.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040_8.gif", doc_1040_other));
            ts.Add(new TrainingSet("trainingdata/documents/1040/2010/tax1040_9.jpg", doc_1040_2010));
            ts.Add(new TrainingSet("trainingdata/documents/1040/2012/tax1040_10.gif", doc_1040_2012));

            ts.Add(new TrainingSet("trainingdata/documents/990/tax990.jpg", doc_990));
            ts.Add(new TrainingSet("trainingdata/documents/990/tax990_2.jpg", doc_990));
            ts.Add(new TrainingSet("trainingdata/documents/990/tax990_3.jpg", doc_990));
            ts.Add(new TrainingSet("trainingdata/documents/990/tax990_4.jpg", doc_990));
            ts.Add(new TrainingSet("trainingdata/documents/990/tax990_5.jpg", doc_990));
            ts.Add(new TrainingSet("trainingdata/documents/990/tax990_6.jpg", doc_990));
  
            int count = 0;

            foreach (var td in ts)
            {
                var name = "generateddata";
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
                        using (FileStream fs = new FileStream(name + "/" + count+".jpg", FileMode.Create, FileAccess.ReadWrite))
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
    }
}
