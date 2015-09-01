#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Serialize.Linq.Internals;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public class TypeResolverNodeFactory : NodeFactory
    {
        private readonly Type[] _expectedTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeResolverNodeFactory"/> class.
        /// </summary>
        /// <param name="expectedTypes">The expected types.</param>
        /// <param name="factorySettings">The factory settings to use.</param>
        /// <exception cref="System.ArgumentNullException">expectedTypes</exception>
        public TypeResolverNodeFactory(IEnumerable<Type> expectedTypes, FactorySettings factorySettings = null)
            : base(factorySettings)
        {
            if (expectedTypes == null)
                throw new ArgumentNullException("expectedTypes");
            _expectedTypes = expectedTypes.ToArray();
        }

        /// <summary>
        /// Determines whether the specified type is expected.
        /// </summary>
        /// <param name="declaredType">Type of the declared.</param>
        /// <returns>
        ///   <c>true</c> if type is expected; otherwise, <c>false</c>.
        /// </returns>
        private bool IsExpectedType(Type declaredType)
        {
            foreach (var expectedType in _expectedTypes)
            {
                if (declaredType == expectedType || declaredType.IsSubclassOf(expectedType))
                    return true;
                if (expectedType.IsInterface)
                {
                    var resultTypes = declaredType.GetInterfaces();
                    if (resultTypes.Contains(expectedType))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries the get constant value from member expression.
        /// </summary>
        /// <param name="memberExpression">The member expression.</param>
        /// <param name="constantValue">The constant value.</param>
        /// <param name="constantValueType">Type of the constant value.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">MemberType ' + memberExpression.Member.MemberType + ' not yet supported.</exception>
        private bool TryGetConstantValueFromMemberExpression(MemberExpression memberExpression, out object constantValue, out Type constantValueType)
        {
            constantValue = null;
            constantValueType = null;

            var run = memberExpression;
            while (run != null)
            {
                if (this.IsExpectedType(run.Member.DeclaringType))
                    return false;

                run = run.Expression as MemberExpression;
            }

            switch (memberExpression.Member.MemberType)
            {
                case MemberTypes.Field:
                    if (memberExpression.Expression != null)
                    {
                        if (memberExpression.Expression.NodeType == ExpressionType.Constant)
                        {
                            var constantExpression = (ConstantExpression)memberExpression.Expression;
                            var flags = this.GetBindingFlags();
                            var fields = flags == null
                                ? constantExpression.Type.GetFields()
                                : constantExpression.Type.GetFields(flags.Value);
                            var memberField = fields.Single(n => memberExpression.Member.Name.Equals(n.Name));
                            constantValueType = memberField.FieldType;
                            constantValue = memberField.GetValue(constantExpression.Value);
                            return true;
                        }
                        var subExpression = memberExpression.Expression as MemberExpression;
                        if (subExpression != null)
                            return this.TryGetConstantValueFromMemberExpression(subExpression, out constantValue, out constantValueType);
                    }
                    var field = (FieldInfo)memberExpression.Member;
                    if (field.IsPrivate || field.IsFamilyAndAssembly)
                    {
                        constantValue = field.GetValue(null);
                        return true;
                    }
                    break;

                case MemberTypes.Property:
                    try
                    {
                        constantValue = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                        return true;
                    }
                    catch (InvalidOperationException)
                    {
                        constantValue = null;
                        return false;
                    }

                default:
                    throw new NotSupportedException("MemberType '" + memberExpression.Member.MemberType + "' not yet supported.");
            }

            return false;
        }

        /// <summary>
        /// Tries to inline an expression.
        /// </summary>
        private bool TryToInlineExpression(MemberExpression memberExpression, out Expression inlineExpression)
        {
            inlineExpression = null;

            if (memberExpression.Member.MemberType != MemberTypes.Field)
                return false;

            if (memberExpression.Expression == null || memberExpression.Expression.NodeType != ExpressionType.Constant)
                return false;

            var constantExpression = (ConstantExpression)memberExpression.Expression;
            var flags = this.GetBindingFlags();
            var fields = flags == null
                ? constantExpression.Type.GetFields()
                : constantExpression.Type.GetFields(flags.Value);
            var memberField = fields.Single(n => memberExpression.Member.Name.Equals(n.Name));
            var constantValue = memberField.GetValue(constantExpression.Value);

            inlineExpression = constantValue as Expression;
            return inlineExpression != null;
        }

        /// <summary>
        /// Resolves the member expression.
        /// </summary>
        private Node ResolveMemberExpression(MemberExpression memberExpression, NodeStack stack)
        {
            Expression inlineExpression;
            if (this.TryToInlineExpression(memberExpression, out inlineExpression))
                return this.CreateNode(inlineExpression, stack);

            object constantValue;
            Type constantValueType;

            return this.TryGetConstantValueFromMemberExpression(memberExpression, out constantValue, out constantValueType)
                ? this.CreateConstantExpressionNode(constantValue, constantValueType, stack)
                : base.CreateNode(memberExpression, stack);
        }

        /// <summary>
        /// Resolves the method call expression.
        /// </summary>
        /// <param name="methodCallExpression">The method call expression.</param>
        /// <returns></returns>
        private Node ResolveMethodCallExpression(MethodCallExpression methodCallExpression, NodeStack stack)
        {
            var memberExpression = methodCallExpression.Object as MemberExpression;
            if (memberExpression != null)
            {
                object constantValue;
                Type constantValueType;
                if (this.TryGetConstantValueFromMemberExpression(memberExpression, out constantValue, out constantValueType))
                {
                    if (methodCallExpression.Arguments.Count == 0)
                    {
                        constantValue = Expression.Lambda(methodCallExpression).Compile().DynamicInvoke();
                        return this.CreateConstantExpressionNode(constantValue, stack);
                    }
                }
            }
            else if (methodCallExpression.Method.Name == "ToString" && methodCallExpression.Method.ReturnType == typeof(string))
            {
                var constantValue = Expression.Lambda(methodCallExpression).Compile().DynamicInvoke();
                return this.CreateConstantExpressionNode(constantValue, stack);
            }
            return base.CreateNode(methodCallExpression, stack);
        }

        private ConstantExpressionNode CreateConstantExpressionNode(object value, NodeStack stack)
        {
            return this.CreateConstantExpressionNode(value, value != null ? value.GetType() : null, stack);
        }

        private ConstantExpressionNode CreateConstantExpressionNode(object value, Type type, NodeStack stack)
        {
            var node = new ConstantExpressionNode
            {
                Value = value, 
                Type = NewAndStack<TypeNode>(type, stack)
            };
            return node;
        }

        internal override Node CreateNode(object root, NodeStack stack)
        {
            if (root is MemberExpression)
                return this.ResolveMemberExpression(root as MemberExpression, stack);
            if (root is MethodCallExpression)
                return this.ResolveMethodCallExpression(root as MethodCallExpression, stack);
            
            return base.CreateNode(root, stack);
        }
    }
}