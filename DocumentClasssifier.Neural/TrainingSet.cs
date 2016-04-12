using System;
using System.Collections.Generic;
using System.IO;
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

        public static Tuple<List<TrainingSet>, Category> FromDirectory(string directory)
        {
            var name = Path.GetFileName(directory);
            var root = new Category(name);
            
            List<TrainingSet> td = new List<TrainingSet>();
            getCategories(ref root, directory, ref td);
            var rootOut = root.Children.First();
            rootOut.Parent = null;
            return Tuple.Create(td, rootOut);
        }

        private static void getCategories(ref Category root, string directory, ref List<TrainingSet> trainingData)
        {
            var dirs = Directory.GetDirectories(directory);

            foreach (var dir in dirs)
            {
                var sc = root.AddSubcategory(Path.GetFileName(dir));

                var files = Directory.GetFiles(dir);

                foreach (var file in files)
                {
                    var ts = new TrainingSet(file, sc);
                    trainingData.Add(ts);
                }

                getCategories(ref sc, dir, ref trainingData);
            }
        }
    }
}
