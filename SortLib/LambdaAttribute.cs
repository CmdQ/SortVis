using System;

namespace SortLib
{
    /// <summary>
    /// Tells which category a sorter falls into.
    /// </summary>
    public class LambdaAttribute : Attribute
    {
        private readonly BigO _lambda;

        /// <summary>
        /// Initializes a new instance of the <see cref="LambdaAttribute"/> class.
        /// </summary>
        /// <param name="lambda">The lambda Big-O category.</param>
        public LambdaAttribute(BigO lambda)
        {
            _lambda = lambda;
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public BigO Lambda
        {
            get
            {
                return _lambda;
            }
        }
    }
}
