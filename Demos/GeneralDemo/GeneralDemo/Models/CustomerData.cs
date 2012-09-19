//-----------------------------------------------------------------------
// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------


using System;
using GeneralDemo.ViewModels;

namespace GeneralDemo.Models
{
    using Customer = HeaderedArray<Tuple<string, DateTime, double>>;
    using Order = Tuple<string, DateTime, double>;

    public class CustomerData
    {
        // this should be in a model
        private Customer[] data = new[]{
            new Customer("Robert MacLean",new Order[]
            {
                 new Order("Halo Reach",new DateTime(2010,12,3), 600),
                 new Order("Forza 4",new DateTime(2010,12,25), 0),
                 new Order("Portal 2",new DateTime(2010,11,3), 550),
                 new Order("Halo",new DateTime(2011,1,3), 220)
            }),
            new Customer("Rudi Grobler",new Order[]
            {
                 new Order("Halo Reach",new DateTime(2010,12,3), 600),
                 new Order("Spiderman",new DateTime(2010,10,1), 300),
                 new Order("Ben Ten",new DateTime(2010,8,17), 300)
            }),
            new Customer("William Brander",new Order[]
            {
                 new Order("Halo ODST",new DateTime(2010,8,3), 0),
                 new Order("Portal",new DateTime(2010,8,3), 0),
                 new Order("Forza 3",new DateTime(2010,8,3), 0),
                 new Order("PGR 3",new DateTime(2010,8,3), 0),
            })
        };

        public Customer[] Data
        {
            get
            {
                return data;
            }
        }
    }
}
