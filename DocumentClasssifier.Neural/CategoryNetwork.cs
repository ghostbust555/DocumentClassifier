using Accord.Neuro;
using AForge.Neuro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentClasssifier.Neural
{
    public class CategoryNetwork
    {
        public ActivationNetwork Network;
        public Category Category;

        /// <summary>
        /// Training data
        /// </summary>
        public List<Category> Catetgories = new List<Category>();
        public List<double[]> ImageData = new List<double[]>();

        const int HIDDEN_LAYER_SIZE = (ImageExtractor.IMAGE_SIZE * ImageExtractor.IMAGE_SIZE)/20;

        public CategoryNetwork(Category category)
        {
            Category = category;

            Network = new ActivationNetwork(new SigmoidFunction(), 
                ImageExtractor.IMAGE_SIZE * ImageExtractor.IMAGE_SIZE, 
                HIDDEN_LAYER_SIZE,
                HIDDEN_LAYER_SIZE/10,
                category.Children.Count);

            new NguyenWidrow(Network).Randomize();
        }

        public void ClearData()
        {
            Catetgories.Clear();
            ImageData.Clear();
        }

        public void AddData(Category category, ref double[] imageData)
        {
            Catetgories.Add(category);
            ImageData.Add(imageData);
        }        
    }
}
