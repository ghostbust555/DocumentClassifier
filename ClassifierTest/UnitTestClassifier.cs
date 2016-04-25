using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DocumentClasssifier.Neural;
using System.Collections.Generic;

namespace ClassifierTest
{
    [TestClass]
    public class UnitTestClassifier
    {
        [TestMethod]
        public void GetImageData()
        {
            ImageExtractor.ExtractImageFeatures("trainingdata/tax1040.gif");
        }

        [TestMethod]
        public void TestNetwork_FromDirectory()
        {
            var validationSet = TrainingSet.FromDirectory("trainingdata");
            var trainingSet = TrainingSet.FromDirectory("generateddata");

            var t = new Trainer();

            var nc = new NetworkCreator();
            var nl = nc.CreateNetworks(trainingSet.Item2);

            t.Train(nl, trainingSet.Item1, validationSet.Item1);

            var o = t.Run(nl, trainingSet.Item2, "trainingdata/documents/990/tax990_poor.gif");
            o = t.Run(nl, trainingSet.Item2, "trainingdata/documents/1040/2012/tax1040_10.gif");
            o = t.Run(nl, trainingSet.Item2, "trainingdata/documents/1040/2010/tax1040_4.jpg");
            var x = o;
        }

        [TestMethod]
        public void TestCategories_FromDirectory()
        {
            var o = TrainingSet.FromDirectory("trainingdata");            
        }

        [TestMethod]
        public void TestCategories()
        {
            var validationSet = new List<TrainingSet>();

            var trainingSet = new List<TrainingSet>();

            var root = new Category("document");
            var doc_1040 = root.AddSubcategory("1040");
            var doc_990 = root.AddSubcategory("990");

            var doc_1040_2012  = doc_1040.AddSubcategory("2012");
            var doc_1040_2010 = doc_1040.AddSubcategory("2010");
            var doc_1040_other = doc_1040.AddSubcategory("other");

            validationSet.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040.gif", doc_1040_other));
            validationSet.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040_2.png", doc_1040_other));
            validationSet.Add(new TrainingSet("trainingdata/documents/1040/2012/tax1040_3.gif", doc_1040_2012));
            validationSet.Add(new TrainingSet("trainingdata/documents/1040/2010/tax1040_4.jpg", doc_1040_2010));
            validationSet.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040_5.gif", doc_1040_other));
            validationSet.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040_6.gif", doc_1040_other));
            validationSet.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040_7.gif", doc_1040_other));
            validationSet.Add(new TrainingSet("trainingdata/documents/1040/other/tax1040_8.gif", doc_1040_other));
            validationSet.Add(new TrainingSet("trainingdata/documents/1040/2010/tax1040_9.jpg", doc_1040_2010));
            validationSet.Add(new TrainingSet("trainingdata/documents/1040/2012/tax1040_10.gif", doc_1040_2012));

            validationSet.Add(new TrainingSet("trainingdata/documents/990/tax990.jpg", doc_990));
            validationSet.Add(new TrainingSet("trainingdata/documents/990/tax990_2.jpg", doc_990));
            validationSet.Add(new TrainingSet("trainingdata/documents/990/tax990_3.jpg", doc_990));
            validationSet.Add(new TrainingSet("trainingdata/documents/990/tax990_4.jpg", doc_990));
            validationSet.Add(new TrainingSet("trainingdata/documents/990/tax990_5.jpg", doc_990));
            validationSet.Add(new TrainingSet("trainingdata/documents/990/tax990_6.jpg", doc_990));
            validationSet.Add(new TrainingSet("trainingdata/documents/990/tax990_7.jpg", doc_990));

            var t = new Trainer();

            var nc = new NetworkCreator();
            var nl = nc.CreateNetworks(root);

            t.Train(nl, validationSet);

            var o = t.Run(nl, root, "trainingdata/documents/990/tax990_poor.gif");
            o = t.Run(nl, root, "trainingdata/documents/1040/2012/tax1040_10.gif");
            o = t.Run(nl, root, "trainingdata/documents/1040/2010/tax1040_4.jpg");
            var x = o;
        }
    }
}
