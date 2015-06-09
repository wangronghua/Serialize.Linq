#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using Serialize.Linq.Factories;

namespace Serialize.Linq.Interfaces
{
    public interface INodeFactory : IExpressionNodeFactory, ITypeNodeFactory
    {
        /// <summary>
        /// Returns the factory settings for this instance
        /// </summary>
        FactorySettings Settings { get; }
    }    
}