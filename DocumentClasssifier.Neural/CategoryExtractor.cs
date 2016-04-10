using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentClasssifier.Neural
{
    public class CategoryExtractor
    {
        public static double[] ExtractCategoryFeature(Category category)
        {
            var parent = category.Parent;
            if (parent != null)
            {
                var output = new double[parent.Children.Count];
                output[parent.Children.IndexOf(category)] = 1;
                return output;
            }

            return null;
        }
    }
}
