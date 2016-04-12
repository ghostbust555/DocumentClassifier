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

        public override bool Equals(object obj)
        {
            return Name == ((Category)(obj)).Name;
        }

        public static bool operator ==(Category a, Category b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Name == b.Name;
        }

        public static bool operator !=(Category a, Category b)
        {
            return !(a == b);
        }

    }
}
