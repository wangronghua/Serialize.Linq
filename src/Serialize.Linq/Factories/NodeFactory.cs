#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public class NodeFactory : INodeFactory
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeFactory"/> class.
        /// </summary>
        public NodeFactory()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeFactory"/> class.
        /// </summary>
        /// <param name="factorySettings">The factory settings to use.</param>
        public NodeFactory(FactorySettings factorySettings)
        {
            Settings = factorySettings ?? new FactorySettings();
        }

        public FactorySettings Settings { get; private set; }

        public Node CreateNode(object obj)
        {
            var stack = new ObjectAndNodeStack();
            return this.CreateNode(obj, stack);
        }

        internal virtual Node CreateNode(object root, ObjectAndNodeStack stack)
        {
            if (root == null)
                return null;

            var rootNode = this.NewAndStack<Node>(root, stack);

            while (stack.TryPop(out object current, out Node currentNode))
                this.Process(current, currentNode, stack);

            return rootNode;
        }

        /// <summary>
        /// Creates an expression node from an expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Unknown expression of type  + expression.GetType()</exception>
        public virtual ExpressionNode Create(Expression expression)
        {
            if (expression == null)
                return null;

            if (expression is BinaryExpression)        return new BinaryExpressionNode(this, expression as BinaryExpression);
            if (expression is ConditionalExpression)   return new ConditionalExpressionNode(this, expression as ConditionalExpression);
            if (expression is ConstantExpression)      return new ConstantExpressionNode(this, expression as ConstantExpression);
            if (expression is InvocationExpression)    return new InvocationExpressionNode(this, expression as InvocationExpression);
            if (expression is LambdaExpression)        return new LambdaExpressionNode(this, expression as LambdaExpression);
            if (expression is ListInitExpression)      return new ListInitExpressionNode(this, expression as ListInitExpression);
            if (expression is MemberExpression)        return new MemberExpressionNode(this, expression as MemberExpression);
            if (expression is MemberInitExpression)    return new MemberInitExpressionNode(this, expression as MemberInitExpression);
            if (expression is MethodCallExpression)    return new MethodCallExpressionNode(this, expression as MethodCallExpression);
            if (expression is NewArrayExpression)      return new NewArrayExpressionNode(this, expression as NewArrayExpression);
            if (expression is NewExpression)           return new NewExpressionNode(this, expression as NewExpression);
            if (expression is ParameterExpression)     return new ParameterExpressionNode(this, expression as ParameterExpression);                        
            if (expression is TypeBinaryExpression)    return new TypeBinaryExpressionNode(this, expression as TypeBinaryExpression);
            if (expression is UnaryExpression)         return new UnaryExpressionNode(this, expression as UnaryExpression);                        

            throw new ArgumentException("Unknown expression of type " + expression.GetType());
        }

        internal virtual TNode NewAndStack<TNode>(object obj, ObjectAndNodeStack stack) where TNode : Node
        {
            var retval = (TNode)New(obj);
            if (retval != null)
                stack.Push(obj, retval);
            return retval;
        }

        /// <summary>
        /// Creates an type node from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public TypeNode Create(Type type)
        {
            return new TypeNode(this, type);
        }

        /// <summary>
        /// Gets binding flags to be used when accessing type members.
        /// </summary>
        public BindingFlags? GetBindingFlags()
        {
            if (!this.Settings.AllowPrivateFieldAccess)
                return null;

            return BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        }

        private void Process(object obj, Node node, ObjectAndNodeStack stack)
        {
            if (node == null)
                return;

            if (obj is Expression expression)
            {
                this.ProcessExpression(expression, (ExpressionNode)node, stack);
                return;
            }

            switch (node.NodeKind)
            {
                case NodeKind.Type:
                    this.ProcessType((Type)obj, (TypeNode)node, stack);
                    break;

                case NodeKind.PropertyInfo:
                    this.ProcessMemberInfo((MemberInfo)obj, (MemberNode)node, stack);
                    break;

                case NodeKind.ConstructorInfo:
                    this.ProcessMemberInfo((MemberInfo)obj, (MemberNode)node, stack);
                    break;

                case NodeKind.FieldInfo:
                    this.ProcessMemberInfo((MemberInfo)obj, (MemberNode)node, stack);
                    break;

                case NodeKind.MemberInfo:
                    this.ProcessMemberInfo((MemberInfo)obj, (MemberNode)node, stack);
                    break;

                case NodeKind.MethodInfo:
                    this.ProcessMethodInfo((MethodInfo)obj, (MethodInfoNode)node, stack);
                    break;

                case NodeKind.ElementInit:
                    this.ProcessElementInit((ElementInit)obj, (ElementInitNode)node, stack);
                    break;

                case NodeKind.MemberAssignment:
                    this.ProcessMemberAssignment((MemberAssignment)obj, (MemberAssignmentNode)node, stack);
                    break;

                case NodeKind.MemberListBinding:
                    this.ProcessMemberListBinding((MemberListBinding)obj, (MemberListBindingNode)node, stack);
                    break;

                case NodeKind.MemberMemberBinding:
                    this.ProcessMemberMemberBinding((MemberMemberBinding)obj, (MemberMemberBindingNode)node, stack);
                    break;

                case NodeKind.ElementInitList:
                    this.ProcessElementInitList((IEnumerable<ElementInit>)obj, (ElementInitNodeList)node, stack);
                    break;

                case NodeKind.MemberBindingList:
                    this.ProcessMemberBindingList((IEnumerable<MemberBinding>)obj, (MemberBindingNodeList)node, stack);
                    break;

                case NodeKind.MemberInfoList:
                    this.ProcesMemberInfoList((IEnumerable<MemberInfo>)obj, (MemberInfoNodeList)node, stack);
                    break;

                case NodeKind.ExpressionList:
                    this.ProcessExpressionNodeList((IEnumerable<Expression>)obj, (ExpressionNodeList)node, stack);
                    break;
            }
        }

        private void ProcesMemberInfoList(IEnumerable<MemberInfo> members, MemberInfoNodeList infoNodeList,
            ObjectAndNodeStack stack)
        {
            foreach (var member in members)
                infoNodeList.Add(NewAndStack<MemberNode>(member, stack));
        }

        private void ProcessMemberBindingList(IEnumerable<MemberBinding> bindings, MemberBindingNodeList bindingNodeList,
            ObjectAndNodeStack stack)
        {
            foreach (var binding in bindings)
                bindingNodeList.Add(NewAndStack<MemberBindingNode>(binding, stack));
        }

        private void ProcessElementInitList(IEnumerable<ElementInit> elements, ElementInitNodeList elementInitNodeList,
            ObjectAndNodeStack stack)
        {
            foreach (var element in elements)
                elementInitNodeList.Add(NewAndStack<ElementInitNode>(element, stack));
        }

        private void ProcessExpressionNodeList(IEnumerable<Expression> expressions,
            ExpressionNodeList expressionNodeList, ObjectAndNodeStack stack)
        {
            foreach (var expression in expressions)
                expressionNodeList.Add(NewAndStack<ExpressionNode>(expression, stack));
        }

        private void ProcessElementInit(ElementInit elementInit, ElementInitNode elementInitNode, ObjectAndNodeStack stack)
        {
            elementInitNode.AddMethod = NewAndStack<MethodInfoNode>(elementInit.AddMethod, stack);
            elementInitNode.Arguments = NewAndStack<ExpressionNodeList>(elementInit.Arguments, stack);
        }

        private void ProcessMemberInfo(MemberInfo member, MemberNode memberNode, ObjectAndNodeStack stack)
        {
            memberNode.DeclaringType = NewAndStack<TypeNode>(member.DeclaringType, stack);
            memberNode.Signature = member.ToString();
        }

        private void ProcessMethodInfo(MethodInfo method, MethodInfoNode methodNode, ObjectAndNodeStack stack)
        {
            methodNode.DeclaringType = NewAndStack<TypeNode>(method.DeclaringType, stack);
            if (method.IsGenericMethod)
            {
                methodNode.IsGenericMethod = true;
                methodNode.Signature = method.GetGenericMethodDefinition().ToString();

                var arguments = method.GetGenericArguments();
                methodNode.GenericArguments = new TypeNode[arguments.Length];
                for (var i = 0; i < arguments.Length; ++i)
                    methodNode.GenericArguments[i] = NewAndStack<TypeNode>(arguments[i], stack);
            }
            else
            {
                methodNode.Signature = method.ToString();
            }
        }

        private void ProcessMemberAssignment(MemberAssignment assignment, MemberAssignmentNode assignmentNode,
            ObjectAndNodeStack stack)
        {
            assignmentNode.Member = NewAndStack<MemberNode>(assignment.Member, stack);
            assignmentNode.Expression = NewAndStack<ExpressionNode>(assignment.Expression, stack);
        }

        private void ProcessMemberListBinding(MemberListBinding listBinding, MemberListBindingNode listBindingNode,
            ObjectAndNodeStack stack)
        {
            listBindingNode.Member = NewAndStack<MemberNode>(listBinding.Member, stack);
            listBindingNode.Initializers = NewAndStack<ElementInitNodeList>(listBinding.Initializers, stack);
        }

        private void ProcessMemberMemberBinding(MemberMemberBinding memberBinding,
            MemberMemberBindingNode memberBindingNode, ObjectAndNodeStack stack)
        {
            memberBindingNode.Member = NewAndStack<MemberNode>(memberBinding.Member, stack);
            memberBindingNode.Bindings = NewAndStack<MemberBindingNodeList>(memberBinding.Bindings, stack);
        }

        private void ProcessBinaryExpression(BinaryExpression expression, BinaryExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
            expressionNode.Left = NewAndStack<ExpressionNode>(expression.Left, stack);
            expressionNode.Right = NewAndStack<ExpressionNode>(expression.Right, stack);
            expressionNode.Conversion = NewAndStack<ExpressionNode>(expression.Conversion, stack);
            expressionNode.Method = NewAndStack<MethodInfoNode>(expression.Method, stack);
            expressionNode.IsLiftedToNull = expression.IsLiftedToNull;
        }

        private void ProcessConditionalExpression(ConditionalExpression expression, ConditionalExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
            expressionNode.IfFalse = NewAndStack<ExpressionNode>(expression.IfFalse, stack);
            expressionNode.IfTrue = NewAndStack<ExpressionNode>(expression.IfTrue, stack);
            expressionNode.Test = NewAndStack<ExpressionNode>(expression.Test, stack);
        }

        private void ProcessConstantExpression(ConstantExpression expression, ConstantExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
            expressionNode.Value = expression.Value;
        }

        private void ProcessInvocationExpression(InvocationExpression expression, InvocationExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
            expressionNode.Arguments = NewAndStack<ExpressionNodeList>(expression.Arguments, stack);
            expressionNode.Expression = NewAndStack<ExpressionNode>(expression.Expression, stack);
        }

        private void ProcessLambdaExpression(LambdaExpression expression, LambdaExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
            expressionNode.Body = NewAndStack<ExpressionNode>(expression.Body, stack);

            IEnumerable<Expression> parameters =
#if !WINDOWS_PHONE7
                expression.Parameters;
#else
                expression.Parameters.Select(p => (Expression)p);
#endif
            expressionNode.Parameters = NewAndStack<ExpressionNodeList>(parameters, stack);
        }
        private void ProcessListInitExpression(ListInitExpression expression, ListInitExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
            expressionNode.Initializers = NewAndStack<ElementInitNodeList>(expression.Initializers, stack);
            expressionNode.NewExpression = NewAndStack<NewExpressionNode>(expression.NewExpression, stack);
        }
        private void ProcessMemberExpression(MemberExpression expression, MemberExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
            expressionNode.Expression = NewAndStack<ExpressionNode>(expression.Expression, stack);
            expressionNode.Member = NewAndStack<MemberNode>(expression.Member, stack);
        }
        private void ProcessMemberInitExpression(MemberInitExpression expression, MemberInitExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
            expressionNode.Bindings = NewAndStack<MemberBindingNodeList>(expression.Bindings, stack);
            expressionNode.NewExpression = NewAndStack<NewExpressionNode>(expression.NewExpression, stack);
        }
        private void ProcessMethodCallExpression(MethodCallExpression expression, MethodCallExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
            expressionNode.Arguments = NewAndStack<ExpressionNodeList>(expression.Arguments, stack);
            expressionNode.Method = NewAndStack<MethodInfoNode>(expression.Method, stack);
            expressionNode.Object = NewAndStack<ExpressionNode>(expression.Object, stack);
        }
        private void ProcessNewArrayExpression(NewArrayExpression expression, NewArrayExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
            expressionNode.Expressions = NewAndStack<ExpressionNodeList>(expression.Expressions, stack);
        }
        private void ProcessNewExpression(NewExpression expression, NewExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
            expressionNode.Arguments = NewAndStack<ExpressionNodeList>(expression.Arguments, stack);
            expressionNode.Constructor = NewAndStack<ConstructorInfoNode>(expression.Constructor, stack);
            expressionNode.Members = NewAndStack<MemberInfoNodeList>(expression.Members, stack);
        }

        private void ProcessParameterExpression(ParameterExpression expression, ParameterExpressionNode expressionNode, ObjectAndNodeStack stack)
        {
#if !WINDOWS_PHONE7
            expressionNode.IsByRef = expression.IsByRef;
#else
            expressionNode.IsByRef = false;
#endif
            expressionNode.Name = expression.Name;
        }

        private static Node New(object obj)
        {
            if (obj == null)
                return null;

            if (obj is BinaryExpression) return new BinaryExpressionNode();
            if (obj is ConditionalExpression) return new ConditionalExpressionNode();
            if (obj is ConstantExpression) return new ConstantExpressionNode();
            if (obj is InvocationExpression) return new InvocationExpressionNode();
            if (obj is LambdaExpression) return new LambdaExpressionNode();
            if (obj is ListInitExpression) return new ListInitExpressionNode();
            if (obj is MemberExpression) return new MemberExpressionNode();
            if (obj is MemberInitExpression) return new MemberInitExpressionNode();
            if (obj is MethodCallExpression) return new MethodCallExpressionNode();
            if (obj is NewArrayExpression) return new NewArrayExpressionNode();
            if (obj is NewExpression) return new NewExpressionNode();
            if (obj is ParameterExpression) return new ParameterExpressionNode();
            if (obj is TypeBinaryExpression) return new TypeBinaryExpressionNode();
            if (obj is UnaryExpression) return new UnaryExpressionNode();

            if (obj is Type) return new TypeNode();
            if (obj is ElementInit) return new ElementInitNode();
            if (obj is FieldInfo) return new FieldInfoNode();
            if (obj is PropertyInfo) return new PropertyInfoNode();
            if (obj is ConstructorInfo) return new ConstructorInfoNode();
            if (obj is MethodInfo) return new MethodInfoNode();
            if (obj is MemberMemberBinding) return new MemberMemberBindingNode();
            if (obj is MemberListBinding) return new MemberListBindingNode();
            if (obj is MemberAssignment) return new MemberAssignmentNode();
            if (obj is MemberInfo) return new MemberInfoNode();


            if (obj is IEnumerable<ElementInit>) return new ElementInitNodeList();
            if (obj is IEnumerable<MemberBinding>) return new MemberBindingNodeList();
            if (obj is IEnumerable<MemberInfo>) return new MemberInfoNodeList();
            if (obj is IEnumerable<Expression>) return new ExpressionNodeList();

            throw new ArgumentException("Cannot create node from object of type " + obj.GetType());
        }
    }
}