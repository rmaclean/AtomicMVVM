//-----------------------------------------------------------------------
// Project: AtomicPhoneMVVM https://bitbucket.org/rmaclean/atomicmvvm
// License: MS-PL http://www.opensource.org/licenses/MS-PL
// Notes:
//-----------------------------------------------------------------------

namespace AtomicPhoneMVVM
{
    using System;

    /// <summary>
    /// Used to attribute "can" methods whose return value is impaced by the property name.
    /// </summary>
    /// <remarks>The propertys must match case, be public and raise INotifyPropertyChanged for this to work.</remarks>
    [AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ReevaluatePropertyAttribute : System.Attribute
    {
        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="propertyNames">he property names that will be associated.</param>
        public ReevaluatePropertyAttribute(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }

        /// <summary>
        /// The property names that will be associated.
        /// </summary>
        public string[] PropertyNames { get; private set; }
    }
}
