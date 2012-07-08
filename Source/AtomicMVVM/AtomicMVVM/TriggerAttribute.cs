///-----------------------------------------------------------------------
/// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
/// License: MS-PL http://www.opensource.org/licenses/MS-PL
/// Notes:
///-----------------------------------------------------------------------

namespace AtomicMVVM
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Instructs the bootstrapper that if any of the listed properties change, the attributed method should be run.
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TriggerPropertyAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerPropertyAttribute" /> class.
        /// </summary>
        /// <param name="propertyNames">The property names used to trigger the attributed method.</param>
        public TriggerPropertyAttribute(params string[] propertyNames)
            : this(0, propertyNames)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerPropertyAttribute" /> class.
        /// </summary>
        /// <param name="priority">The order to run the attributed method compared to other trigger attributed methods. Higher first.</param>
        /// <param name="propertyNames">The property names used to trigger the attributed method.</param>
        public TriggerPropertyAttribute(int priority, params string[] propertyNames)
        {
            this.PropertyNames = propertyNames;
            this.Priority = priority;
        }

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <value>
        /// The property names.
        /// </value>
        public IEnumerable<string> PropertyNames { get; private set; }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        public int Priority { get; private set; }
    }
}
