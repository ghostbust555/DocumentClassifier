using Accord.Neuro.Learning;
using AForge.Neuro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocumentClasssifier.Neural
{
    public class Trainer
    {
        public double Train(List<CategoryNetwork> networks, List<TrainingSet> trainingSetList, List<TrainingSet> validationSetList = null)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var ng = new NetworkGraph();

            new Thread(() =>
            {
                ng.Show();
                Application.Run(ng);
            }).Start();

            foreach (var net in networks)
            {
                net.ClearData();
            }

            for (int i = 0; i < trainingSetList.Count; i++) {
                var cat = trainingSetList[i].Category;
                var imageData = ImageExtractor.ExtractImageFeatures(trainingSetList[i].ImageFile);
                while (cat.Parent != null)
                {
                    var n = GetNetwork(networks, cat.Parent);
                    n.AddData(cat, ref imageData);  
                    cat = cat.Parent;
                }
            }

            foreach(var net in networks)
            {
                var teacher = new ParallelResilientBackpropagationLearning(net.Network);

                teacher.UpdateUpperBound = 500;

                var inputs = net.ImageData.ToArray();
                var outputs = new double[net.Catetgories.Count][];

                for (int i = 0; i < net.Catetgories.Count; i++)
                {
                    outputs[i] = CategoryExtractor.ExtractCategoryFeature(net.Catetgories[i]);                    
                }

                double error = 100;
                int k = 0;


                ng.Series.Points.Clear();
                error = teacher.ComputeError(inputs, outputs);
                DateTime start = DateTime.Now;
                ng.AddPoint(error, DateTime.Now-start);
                

                while (error > .1)
                {
                    error = teacher.RunEpoch(inputs, outputs);
                    ng.AddPoint(error, DateTime.Now - start);
                    k++;
                }

                //ng.Invoke((MethodInvoker)delegate () { ng.Close(); });
            }
                        
            return 0;
        }

        public List<Result> Run(List<CategoryNetwork> networks, Category root, string imageFile)
        {
            var output = new List<Result>();

            var currentCategory = root;

            while (currentCategory.Children.Count > 0)
            {
                var net = GetNetwork(networks, currentCategory);
                var netout = net.Network.Compute(ImageExtractor.ExtractImageFeatures(imageFile));

                var max = netout.Max();
                var maxI = Array.IndexOf(netout, max);

                var result = new Result(net.Category.Children[maxI], max);
                output.Add(result);
                currentCategory = result.Category;
            }            

            return output;
        }

        public List<List<Result>> Run(List<CategoryNetwork> networks, Category root, List<string> imageFiles)
        {
            var output = new List<List<Result>>();

            foreach(var imageFile in imageFiles)
            {
                output.Add(Run(networks,root,imageFile));                
            }

            return output;
        }


        public CategoryNetwork GetNetwork(List<CategoryNetwork> networks, Category cat)
        {
            foreach(var cn in networks)
            {
                if(cn.Category == cat)
                {
                    return cn;
                }
            }

            return null;
        }
    }
}
