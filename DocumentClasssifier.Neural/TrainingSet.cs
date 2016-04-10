using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentClasssifier.Neural
{
    public class TrainingSet
    {
        public string ImageFile;
        public Category Category;

        public TrainingSet(string imageFile, Category category)
        {
            ImageFile = imageFile;
            Category = category;
        }
    }
}
