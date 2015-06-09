using System;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Interfaces
{
    public interface ITypeNodeFactory
    {
        /// <summary>
        /// Creates the specified type node from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        TypeNode CreateTypeNode(Type type);
    }
}
