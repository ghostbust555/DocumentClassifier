using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentClasssifier.Neural
{
    public class NetworkCreator
    {
        public List<CategoryNetwork> Networks = new List<CategoryNetwork>();
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
