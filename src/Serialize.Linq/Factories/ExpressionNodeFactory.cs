using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    internal class ExpressionNodeFactory : IExpressionNodeFactory
    {
        private readonly ITypeNodeFactory _typeNodeFactory;
        private readonly FactorySettings _factorySettings;

        public ExpressionNodeFactory(ITypeNodeFactory typeNodeFactory, FactorySettings factorySettings)
        {
            if (typeNodeFactory == null)
                throw new ArgumentNullException("typeNodeFactory");
            if (factorySettings == null)
                throw new ArgumentNullException("factorySettings");
            _typeNodeFactory = typeNodeFactory;
            _factorySettings = factorySettings;
        }

        public ExpressionNode CreateExpressionNode(Expression rootExpression)
        {
            if (rootExpression == null)
                return null;

            var retval = New(rootExpression);

            var stack = new NodeStack();
            stack.Push(rootExpression, retval);

            Expression expression;
            ExpressionNode expressionNode;

            while (stack.TryPop(out expression, out expressionNode))
                this.Process(expression, expressionNode, stack);

            return retval;
        }

        private void Process(Expression expression, ExpressionNode expressionNode, NodeStack stack)
        {
            expressionNode.NodeType = expression.NodeType;
            expressionNode.Type = _typeNodeFactory.CreateTypeNode(expression.Type);

            switch (expressionNode.ExpressionNodeType)
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

                case NodeKind.Lambda:
                    this.ProcessLambda((LambdaExpression)expression, (LambdaExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.ListInit:
                    this.ProcessListInit((ListInitExpression)expression, (ListInitExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.Member:
                    this.ProcessMember((MemberExpression)expression, (MemberExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.MemberInit:
                    this.ProcessMemberInit((MemberInitExpression)expression, (MemberInitExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.MethodCall:
                    this.ProcessMethodCall((MethodCallExpression)expression, (MethodCallExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.New:
                    this.ProcessNew((NewExpression)expression, (NewExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.NewArray:
                    this.ProcessNewArray((NewArrayExpression)expression, (NewArrayExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.Parameter:
                    this.ProcessParameter((ParameterExpression)expression, (ParameterExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.TypeBinary:
                    this.ProcessTypeBinary((TypeBinaryExpression)expression, (TypeBinaryExpressionNode)expressionNode, stack);
                    break;

                case NodeKind.Unary:
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

        private static ExpressionNode New(Expression expression)
        {
            if (expression is BinaryExpression) return new BinaryExpressionNode();
            if (expression is ConditionalExpression) return new ConditionalExpressionNode();
            if (expression is ConstantExpression) return new ConstantExpressionNode();
            if (expression is InvocationExpression) return new InvocationExpressionNode();
            if (expression is LambdaExpression) return new LambdaExpressionNode();
            if (expression is ListInitExpression) return new ListInitExpressionNode();
            if (expression is MemberExpression) return new MemberExpressionNode();
            if (expression is MemberInitExpression) return new MemberInitExpressionNode();
            if (expression is MethodCallExpression) return new MethodCallExpressionNode();
            if (expression is NewArrayExpression) return new NewArrayExpressionNode();
            if (expression is NewExpression) return new NewExpressionNode();
            if (expression is ParameterExpression) return new ParameterExpressionNode();
            if (expression is TypeBinaryExpression) return new TypeBinaryExpressionNode();
            if (expression is UnaryExpression) return new UnaryExpressionNode();

            throw new ArgumentException("Unknown expression of type " + expression.GetType());
        }
    }
}
