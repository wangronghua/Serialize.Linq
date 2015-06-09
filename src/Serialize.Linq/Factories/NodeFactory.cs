#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Linq.Expressions;
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
            {
                
            }

            return rootNode;
        }

        /// <summary>
        /// Creates an expression node from an expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Unknown expression of type  + expression.GetType()</exception>
        public virtual ExpressionNode CreateExpressionNode(Expression expression)
        {
            var expressionNodeFactory = new ExpressionNodeFactory(this, this.Settings);
            return expressionNodeFactory.CreateExpressionNode(expression);
        }

        /// <summary>
        /// Creates an type node from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public TypeNode CreateTypeNode(Type type)
        {
            var typeNodeFactory = new TypeNodeFactory(this.Settings);
            return typeNodeFactory.CreateTypeNode(type);
        }

        private void Process(Expression expression, ExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.NodeType = expression.NodeType;
            expressionNode.Type = _typeNodeFactory.CreateTypeNode(expression.Type);

            switch (expressionNode.NodeKind)
            {
                case NodeKind.BinaryExpression:
                    this.ProcessBinary((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.ConditionalExpression:
                    this.ProcessConditional((ConditionalExpression)expression, (ConditionalExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.ConstantExpression:
                    this.ProcessConstant((ConstantExpression)expression, (ConstantExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.InvocationExpression:
                    this.ProcessInvocation((InvocationExpression)expression, (InvocationExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.LambdaExpression:
                    this.ProcessLambda((LambdaExpression)expression, (LambdaExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.ListInitExpression:
                    this.ProcessListInit((ListInitExpression)expression, (ListInitExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.MemberExpression:
                    this.ProcessMember((MemberExpression)expression, (MemberExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.MemberInitExpression:
                    this.ProcessMemberInit((MemberInitExpression)expression, (MemberInitExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.MethodCallExpression:
                    this.ProcessMethodCall((MethodCallExpression)expression, (MethodCallExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.NewExpression:
                    this.ProcessNew((NewExpression)expression, (NewExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.NewArrayExpression:
                    this.ProcessNewArray((NewArrayExpression)expression, (NewArrayExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.ParameterExpression:
                    this.ProcessParameter((ParameterExpression)expression, (ParameterExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.TypeBinaryExpression:
                    this.ProcessTypeBinary((TypeBinaryExpression)expression, (TypeBinaryExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.UnaryExpression:
                    this.ProcessUnary((UnaryExpression)expression, (UnaryExpressionNode)expressionNode, stack);
                    break;
            }
        }

        private void ProcessBinary(BinaryExpression expression, BinaryExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.Method = new MethodInfoNode(this.Factory, expression.Method);
            expressionNode.IsLiftedToNull = expression.IsLiftedToNull;
        }

        private void ProcessConditional(ConditionalExpression expression, ConditionalExpressionNode expressionNode, NodeStack stack)
        {

        }

        private void ProcessConstant(ConstantExpression expression, ConstantExpressionNode expressionNode, NodeStack stack)
        {

        }

        private void ProcessInvocation(InvocationExpression expression, InvocationExpressionNode expressionNode, NodeStack stack)
        {

        }
        private void ProcessLambda(LambdaExpression expression, LambdaExpressionNode expressionNode, NodeStack stack)
        {

        }
        private void ProcessListInit(ListInitExpression expression, ListInitExpressionNode expressionNode, NodeStack stack)
        {

        }
        private void ProcessMember(MemberExpression expression, MemberExpressionNode expressionNode, NodeStack stack)
        {

        }
        private void ProcessMemberInit(MemberInitExpression expression, MemberInitExpressionNode expressionNode, NodeStack stack)
        {

        }
        private void ProcessMethodCall(MethodCallExpression expression, MethodCallExpressionNode expressionNode, NodeStack stack)
        {

        }
        private void ProcessNewArray(NewArrayExpression expression, NewArrayExpressionNode expressionNode, NodeStack stack)
        {

        }
        private void ProcessNew(NewExpression expression, NewExpressionNode expressionNode, NodeStack stack)
        {

        }
        private void ProcessParameter(ParameterExpression expression, ParameterExpressionNode expressionNode, NodeStack stack)
        {

        }
        private void ProcessTypeBinary(TypeBinaryExpression expression, TypeBinaryExpressionNode expressionNode, NodeStack stack)
        {

        }
        private void ProcessUnary(UnaryExpression expression, UnaryExpressionNode expressionNode, NodeStack stack)
        {

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



            throw new ArgumentException("Cannot create node from object of type " + obj.GetType());
        }
    }
}