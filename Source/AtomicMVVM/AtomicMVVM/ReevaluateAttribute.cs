
namespace AtomicMVVM
{
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// Indicates that the attribute method is affected by the listed properties.
    /// </summary>
    /// <remarks>
    /// Used to cause an automatic firing of the "can" methods.
    /// </remarks>
    [AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ReevaluatePropertyAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReevaluatePropertyAttribute" /> class.
        /// </summary>
        /// <param name="propertyNames">The property names that can affect the output of the attributed "can" method.</param>
        public ReevaluatePropertyAttribute(params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <value>
        /// The property names.
        /// </value>
        public IEnumerable<string> PropertyNames { get; private set; }
    }
}
