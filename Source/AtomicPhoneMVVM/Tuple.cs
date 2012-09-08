
namespace AtomicPhoneMVVM
{
    /// <summary>
    /// Provides a simple two value Tuple.
    /// </summary>
    /// <typeparam name="T">The type of data in item 1.</typeparam>
    /// <typeparam name="K">The type of data in item 2.</typeparam>
    public class Tuple<T,K>
    {
        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <param name="item1">The data for item 1.</param>
        /// <param name="item2">The data for item 2.</param>
        public Tuple(T item1, K item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        /// <summary>
        /// Item 1 - as provided in the constructor.
        /// </summary>
        public T Item1 { get; private set; }

        /// <summary>
        /// Item 2 - as provided in the constructor.
        /// </summary>
        public K Item2 { get; private set; }
    }
}
