using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentClasssifier.Neural
{
    public class Result
    {
        public double Confidence;
        public Category Category;

        public Result(Category category, double conf)
        {
            Category = category;
            Confidence = conf;
        }
    }
}
