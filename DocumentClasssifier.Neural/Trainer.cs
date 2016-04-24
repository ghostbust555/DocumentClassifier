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
            var ng = new NetworkGraph();

            new Thread(() =>
            {
                ng.ShowDialog();
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

                ng.AddTitle(net.Category.Name);
                ng.ResetData();

                teacher.UpdateUpperBound = 500;

                var inputs = net.ImageData.ToArray();
                var outputs = new double[net.Catetgories.Count][];

                var thisValSet = new List<TrainingSet>();
                if (validationSetList != null)
                {
                    foreach (var td in validationSetList)
                    {
                        var cat = td.Category;

                        while (cat != null)
                        {
                            if (cat.Parent == net.Category)
                            {
                                thisValSet.Add(new TrainingSet(td.ImageFile, cat));
                                break;
                            }

                            cat = cat.Parent;
                        }
                    }
                }


                var valOuts = thisValSet.Select(x => CategoryExtractor.ExtractCategoryFeature(x.Category)).ToArray();
                var valIns = thisValSet.Select(x => ImageExtractor.ExtractImageFeatures(x.ImageFile)).ToArray();

                for (int i = 0; i < net.Catetgories.Count; i++)
                {
                    outputs[i] = CategoryExtractor.ExtractCategoryFeature(net.Catetgories[i]);                    
                }
                
                int k = 0;


                double localError = teacher.ComputeError(inputs, outputs) / inputs.Length;
                double validationError = 1;

                DateTime start = DateTime.Now;
                ng.AddPoint(localError, .1, DateTime.Now-start);

                //validationError > .5 && 
                while (validationError > .05 && localError > .04)
                {
                    localError = teacher.RunEpoch(inputs, outputs)/inputs.Length;
                    if(valIns.Length > 0) validationError = teacher.ComputeError(valIns, valOuts) / valIns.Length;
                    ng.AddPoint(localError, validationError, DateTime.Now - start);
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
