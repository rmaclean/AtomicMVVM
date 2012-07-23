//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicPhoneMVVM
{
    using System;

    [AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TriggerPropertyAttribute : System.Attribute
    {
        public TriggerPropertyAttribute(params string[] propertyNames)
            : this(0, propertyNames)
        {
        }

        public TriggerPropertyAttribute(int order, params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
            this.Order = order;
        }

        public string[] PropertyNames { get; private set; }
        public int Order { get; private set; }
    }
}
