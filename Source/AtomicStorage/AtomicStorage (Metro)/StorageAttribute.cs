///-----------------------------------------------------------------------
/// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
/// License: MS-PL http://www.opensource.org/licenses/MS-PL
/// Notes:
///-----------------------------------------------------------------------

namespace AtomicStorage
{
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class StorageAttribute : Attribute
    {
        public string Name { get; private set; }        

        public StorageAttribute(string name)
        {
            this.Name = name;
        }        
    }
}
