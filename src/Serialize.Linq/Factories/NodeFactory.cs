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
using System.Runtime.CompilerServices;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public class NodeFactory : INodeFactory
    {
        private readonly FactorySettings _factorySettings;

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
            _factorySettings = factorySettings ?? new FactorySettings();
        }

        public FactorySettings Settings
        {
            get { return _factorySettings; }
        }

        public Node CreateNode(object root)
        {
            if (root == null)
                return null;

            var rootNode = New(root);
            var nodeStack = new NodeStack();
            nodeStack.Push(root, rootNode);

            object current;
            Node currentNode;
            while (nodeStack.TryPop(out current, out currentNode))
                this.Process(current, currentNode, nodeStack);

            return rootNode;
        }
        
        private void Process(object obj, Node node, NodeStack stack)
        {
            var expression = obj as Expression;
            if (expression != null)
            {
                this.ProcessExpression(expression, (ExpressionNode)node, stack);
                return;
            }

            switch (node.NodeKind)
            {
                case NodeKind.Type:
                    this.ProcessType((Type)obj, (TypeNode)node, stack);
                    break;
            }
        }

        protected virtual void ProcessExpression(Expression expression, ExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.NodeType = expression.NodeType;
            expressionNode.Type = NewAndStack<TypeNode>(expression.Type, stack);

            switch (expressionNode.NodeKind)
            {
                case NodeKind.BinaryExpression:
                    this.ProcessBinaryExpression((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.ConditionalExpression:
                    this.ProcessConditionalExpression((ConditionalExpression)expression, (ConditionalExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.ConstantExpression:
                    this.ProcessConstantExpression((ConstantExpression)expression, (ConstantExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.InvocationExpression:
                    this.ProcessInvocationExpression((InvocationExpression)expression, (InvocationExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.LambdaExpression:
                    this.ProcessLambdaExpression((LambdaExpression)expression, (LambdaExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.ListInitExpression:
                    this.ProcessListInitExpression((ListInitExpression)expression, (ListInitExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.MemberExpression:
                    this.ProcessMemberExpression((MemberExpression)expression, (MemberExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.MemberInitExpression:
                    this.ProcessMemberInitExpression((MemberInitExpression)expression, (MemberInitExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.MethodCallExpression:
                    this.ProcessMethodCallExpression((MethodCallExpression)expression, (MethodCallExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.NewExpression:
                    this.ProcessNewExpression((NewExpression)expression, (NewExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.NewArrayExpression:
                    this.ProcessNewArrayExpression((NewArrayExpression)expression, (NewArrayExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.ParameterExpression:
                    this.ProcessParameterExpression((ParameterExpression)expression, (ParameterExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.TypeBinaryExpression:
                    this.ProcessTypeBinaryExpression((TypeBinaryExpression)expression, (TypeBinaryExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.UnaryExpression:
                    this.ProcessUnaryExpression((UnaryExpression)expression, (UnaryExpressionNode)expressionNode, stack);
                    break;
            }
        }

        private void ProcessBinaryExpression(BinaryExpression expression, BinaryExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.Left = NewAndStack<ExpressionNode>(expression.Left, stack);
            expressionNode.Right = NewAndStack<ExpressionNode>(expression.Right, stack);
            expressionNode.Conversion = NewAndStack<ExpressionNode>(expression.Conversion, stack);
            expressionNode.Method = NewAndStack<MethodInfoNode>(expression.Method, stack);
            expressionNode.IsLiftedToNull = expression.IsLiftedToNull;
        }

        private void ProcessConditionalExpression(ConditionalExpression expression, ConditionalExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.IfFalse = NewAndStack<ExpressionNode>(expression.IfFalse, stack);
            expressionNode.IfTrue = NewAndStack<ExpressionNode>(expression.IfTrue, stack);
            expressionNode.Test = NewAndStack<ExpressionNode>(expression.Test, stack);
        }

        private void ProcessConstantExpression(ConstantExpression expression, ConstantExpressionNode expressionNode, NodeStack stack)
        {

        }

        private void ProcessInvocationExpression(InvocationExpression expression, InvocationExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.Arguments = NewAndStack<ExpressionNodeList>(expression.Arguments, stack);
            expressionNode.Expression = NewAndStack<ExpressionNode>(expression.Expression, stack);
        }

        private void ProcessLambdaExpression(LambdaExpression expression, LambdaExpressionNode expressionNode, NodeStack stack)
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
        private void ProcessListInitExpression(ListInitExpression expression, ListInitExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.Initializers = NewAndStack<ElementInitNodeList>(expression.Initializers, stack);
            expressionNode.NewExpression = NewAndStack<NewExpressionNode>(expression.NewExpression, stack);
        }
        private void ProcessMemberExpression(MemberExpression expression, MemberExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.Expression = NewAndStack<ExpressionNode>(expression.Expression, stack);
            expressionNode.Member = NewAndStack<MemberInfoNode>(expression.Member, stack);
        }
        private void ProcessMemberInitExpression(MemberInitExpression expression, MemberInitExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.Bindings = NewAndStack<MemberBindingNodeList>(expression.Bindings, stack);
            expressionNode.NewExpression = NewAndStack<NewExpressionNode>(expression.NewExpression, stack);
        }
        private void ProcessMethodCallExpression(MethodCallExpression expression, MethodCallExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.Arguments = NewAndStack<ExpressionNodeList>(expression.Arguments, stack);
            expressionNode.Method = NewAndStack<MethodInfoNode>(expression.Method, stack);
            expressionNode.Object = NewAndStack<ExpressionNode>(expression.Object, stack);
        }
        private void ProcessNewArrayExpression(NewArrayExpression expression, NewArrayExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.Expressions = NewAndStack<ExpressionNodeList>(expression.Expressions, stack);
        }
        private void ProcessNewExpression(NewExpression expression, NewExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.Arguments = NewAndStack<ExpressionNodeList>(expression.Arguments, stack);
            expressionNode.Constructor = NewAndStack<ConstructorInfoNode>(expression.Constructor, stack);
            expressionNode.Members = NewAndStack<MemberInfoNodeList>(expression.Members, stack);
        }

        private void ProcessParameterExpression(ParameterExpression expression, ParameterExpressionNode expressionNode, NodeStack stack)
        {
#if !WINDOWS_PHONE7
            expressionNode.IsByRef = expression.IsByRef;
#else
            expressionNode.IsByRef = false;
#endif
            expressionNode.Name = expression.Name;
        }

        private void ProcessType(Type type, TypeNode typeNode, NodeStack stack)
        {
            object current = null;
            Node currentNode = null;
            do
            {
                if (current != null)
                {
                    if (!(current is Type))
                        break;
                    stack.TryPop(out current, out currentNode);
                    type = (Type) current;
                    typeNode = (TypeNode) currentNode;
                }

                var isAnonymousType = Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                    && type.IsGenericType && type.Name.Contains("AnonymousType")
                    && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                    && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;

                if (type.IsGenericType)
                {
                    var argTypes = type.GetGenericArguments();

                    typeNode.GenericArguments = new TypeNode[argTypes.Length];
                    for (var i = 0; i < argTypes.Length; ++i)
                        typeNode.GenericArguments[i] = NewAndStack<TypeNode>(argTypes[i], stack);
                    type = type.GetGenericTypeDefinition();
                }

                if (isAnonymousType || !_factorySettings.UseRelaxedTypeNames)
                    typeNode.Name = type.AssemblyQualifiedName;
                else
                    typeNode.Name = type.FullName;
                
            } 
            while (stack.TryPeek(out current, out currentNode));
        }

        private void ProcessTypeBinaryExpression(TypeBinaryExpression expression, TypeBinaryExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.Expression = NewAndStack<ExpressionNode>(expression.Expression, stack);
            expressionNode.TypeOperand = NewAndStack<TypeNode>(expression.TypeOperand, stack);
        }
        private void ProcessUnaryExpression(UnaryExpression expression, UnaryExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.Operand = NewAndStack<ExpressionNode>(expression.Operand, stack);
        }

        private static TNode NewAndStack<TNode>(object obj, NodeStack stack) where TNode : Node
        {
            var retval = (TNode) New(obj);
            stack.Push(obj, retval);
            return retval;
        }

        private static Node New(object obj)
        {
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
            if (obj is IEnumerable<ElementInit>) return new ElementInitNodeList();
            if (obj is FieldInfo) return new FieldInfoNode();
            if (obj is PropertyInfo) return new PropertyInfoNode();
            if (obj is MethodInfo) return new MethodInfoNode();
            if (obj is ConstructorInfo) return new ConstructorInfoNode();
            if (obj is MemberMemberBinding) return new MemberMemberBindingNode();
            if (obj is MemberListBinding) return new MemberListBindingNode();
            if (obj is MemberAssignment) return new MemberAssignmentNode();
            if (obj is IEnumerable<MemberBinding>) return new MemberBindingNodeList();
            if (obj is MemberInfo) return new MemberInfoNode();
            if (obj is IEnumerable<MemberInfo>) return new MemberInfoNodeList();
            
            throw new ArgumentException("Cannot create node from object of type " + obj.GetType());
        }
    }
}