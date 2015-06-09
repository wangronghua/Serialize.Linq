using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    internal class TypeNodeFactory : ITypeNodeFactory
    {
        private readonly FactorySettings _factorySettings;

        public TypeNodeFactory(FactorySettings factorySettings)
        {
            if (factorySettings == null)
                throw new ArgumentNullException("factorySettings");
            _factorySettings = factorySettings;
        }

        public TypeNode CreateTypeNode(Type rootType)
        {
            if (rootType == null)
                return null;

            var retval = new TypeNode();

            var stack = new TypeStack();
            stack.Push(rootType, retval);

            Type type;
            TypeNode typeNode;
            while (stack.TryPop(out type, out typeNode))
            {
                var isAnonymousType = Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                    && type.IsGenericType && type.Name.Contains("AnonymousType")
                    && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                    && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;

                if (type.IsGenericType)
                {
                    var argTypes = type.GetGenericArguments();

                    typeNode.GenericArguments = new TypeNode[argTypes.Length];
                    for (var i = 0; i < argTypes.Length; ++i)
                    {
                        var argTypeNode = new TypeNode();
                        stack.Push(argTypes[i], argTypeNode);
                        typeNode.GenericArguments[i] = argTypeNode;
                    }
                    type = type.GetGenericTypeDefinition();
                }

                if (isAnonymousType || !_factorySettings.UseRelaxedTypeNames)
                    typeNode.Name = type.AssemblyQualifiedName;
                else
                    typeNode.Name = type.FullName;
            }

            return retval;
        }
    }
}
