//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicPhoneMVVM
{
    using System;

    /// <summary>
    /// Allows any method to be fired off by a change of a property.
    /// </summary>
    /// <remarks>The propertys must match case, be public and raise INotifyPropertyChanged for this to work.</remarks>
    [AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TriggerPropertyAttribute : System.Attribute
    {
        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="propertyNames">The property names that will be associated.</param>
        public TriggerPropertyAttribute(params string[] propertyNames)
            : this(0, propertyNames)
        {
        }

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="propertyNames">The property names that will be associated.</param>
        /// <param name="order">Specifies the order in which to run multiple associated methods.</param>
        public TriggerPropertyAttribute(int order, params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
            this.Order = order;
        }

        /// <summary>
        ///     The property names that will be associated.
        /// </summary>
        public string[] PropertyNames { get; private set; }

        /// <summary>
        /// Specifies the order in which to run multiple associated methods.
        /// </summary>
        public int Order { get; private set; }
    }
}
