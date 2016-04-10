using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentClasssifier.Neural
{
    public class Category
    {
        public string Name;
        public List<Category> Children = new List<Category>();
        public Category Parent = null;

        public Category(string name)
        {
            Name = name;
        }

        public Category AddSubcategory(string name)
        {
            var cat = new Category(name);
            cat.Parent = this;
            Children.Add(cat);

            return cat;
        }
    }
}
