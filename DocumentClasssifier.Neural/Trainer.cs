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
        //train the classifier networks
        public double Train(List<CategoryNetwork> networks, List<TrainingSet> trainingSetList, List<TrainingSet> validationSetList = null)
        {
            var ng = new NetworkGraph();

            //open a new training process display in a new thread
            new Thread(() =>
            {
                ng.Load += (sender, e) => (sender as NetworkGraph).Visible = true;
                ng.ShowDialog();
            }).Start();

            //start fresh, reset all networks
            foreach (var net in networks)
            {
                net.ClearData();
            }

            //create a list of categoryNetworks and add the images to them to generate epochs
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

            //for each network begin training with paralell rprop
            foreach(var net in networks)
            {
                var teacher = new ParallelResilientBackpropagationLearning(net.Network);

                //display the category being trained
                ng.AddTitle(net.Category.Name);
                ng.ResetData();

             //   teacher.UpdateUpperBound = 500;

                var inputs = net.ImageData.ToArray();
                var outputs = new double[net.Catetgories.Count][];


                //dtermine which documents belong to the calidation set for the network currently being trained
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

                //vectorize these images and their respective categorical classification
                var valOuts = thisValSet.Select(x => CategoryExtractor.ExtractCategoryFeature(x.Category)).ToArray();
                var valIns = thisValSet.Select(x => ImageExtractor.ExtractImageFeatures(x.ImageFile)).ToArray();

                for (int i = 0; i < net.Catetgories.Count; i++)
                {
                    outputs[i] = CategoryExtractor.ExtractCategoryFeature(net.Catetgories[i]);                    
                }
                
                int k = 0;

                //find the current batch error during training
                double localError = teacher.ComputeError(inputs, outputs) / inputs.Length;
                double validationError = 1;

                DateTime start = DateTime.Now;
                ng.AddPoint(localError, localError, DateTime.Now-start);
                
                //find the validation error by checking the validation set on the network
                while (validationError > .05 && localError > .04)
                {
                    localError = teacher.RunEpoch(inputs, outputs)/inputs.Length;
                    if(valIns.Length > 0) validationError = teacher.ComputeError(valIns, valOuts) / valIns.Length;
                    ng.AddPoint(localError, validationError, DateTime.Now - start);
                    k++;
                }
                
            }
                        
            return 0;
        }

        //Find the category and confidence chain of an image
        public List<Result> Run(List<CategoryNetwork> networks, Category root, string imageFile)
        {
            var output = new List<Result>();

            var currentCategory = root;

            //keep deeping until you hit a terminal node
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

        //Find the categories and confidence chain values for a set of images
        public List<List<Result>> Run(List<CategoryNetwork> networks, Category root, List<string> imageFiles)
        {
            var output = new List<List<Result>>();

            foreach(var imageFile in imageFiles)
            {
                output.Add(Run(networks,root,imageFile));                
            }

            return output;
        }

        //Get the network belonging to a specific category from a list of networks
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
