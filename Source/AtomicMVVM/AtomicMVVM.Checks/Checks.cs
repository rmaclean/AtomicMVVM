using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AtomicMVVM
{
    public class Checks
    {
        [Fact]
        public void BootStrapperConstructor()
        {
            var bootStrapper = new Bootstrapper();
            Assert.IsType<Bootstrapper>(bootStrapper);
        }
    }
}
