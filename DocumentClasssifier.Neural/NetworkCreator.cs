using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentClasssifier.Neural
{
    //creates a network from a root category
    public class NetworkCreator
    {
        public List<CategoryNetwork> Networks = new List<CategoryNetwork>();

        //creates a network from a root category
        public List<CategoryNetwork> CreateNetworks(Category root)
        {
            if (root.Children.Count == 0)
            {
                return null;
            }
            else
            {
                var cn = new CategoryNetwork(root);
                Networks.Add(cn);

                foreach (var child in root.Children)
                {
                    CreateNetworks(child);
                }
            }

            return Networks;
        }
    }
}
