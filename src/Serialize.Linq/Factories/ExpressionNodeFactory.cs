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

            var stack = new ExpressionStack();
            stack.Push(rootExpression, retval);

            Expression expression;
            ExpressionNode expressionNode;

            while (stack.TryPop(out expression, out expressionNode))
                this.Process(expression, expressionNode, stack);

            return retval;
        }

        private void Process(Expression expression, ExpressionNode expressionNode, ExpressionStack stack)
        {
            expressionNode.NodeType = expression.NodeType;
            expressionNode.Type = _typeNodeFactory.CreateTypeNode(expression.Type);

            switch (expressionNode.ExpressionNodeType)
            {
                case ExpressionNodeType.Binary:
                    this.ProcessBinary((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.Conditional:
                    this.ProcessBinary((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.Constant:
                    this.ProcessBinary((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.Invocation:
                    this.ProcessBinary((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.Lambda:
                    this.ProcessBinary((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.ListInit:
                    this.ProcessBinary((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.Member:
                    this.ProcessBinary((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.MemberInit:
                    this.ProcessBinary((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.MethodCall:
                    this.ProcessBinary((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.New:
                    this.ProcessBinary((BinaryExpression)expression, (BinaryExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.NewArray:
                    this.ProcessNewArray((NewArrayExpression)expression, (NewArrayExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.Parameter:
                    this.ProcessParameter((ParameterExpression)expression, (ParameterExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.TypeBinary:
                    this.ProcessTypeBinary((TypeBinaryExpression)expression, (TypeBinaryExpressionNode)expressionNode, stack);
                    break;

                case ExpressionNodeType.Unary:
                    this.ProcessUnary((UnaryExpression)expression, (UnaryExpressionNode)expressionNode, stack);
                    break;
            }
        }

        private void ProcessBinary(BinaryExpression expression, BinaryExpressionNode expressionNode, ExpressionStack stack)
        {
            
        }

        private void ProcessConditional(ConditionalExpression expression, ConditionalExpressionNode expressionNode, ExpressionStack stack)
        {

        }

        private void ProcessConstant(ConstantExpression expression, ConstantExpressionNode expressionNode, ExpressionStack stack)
        {

        }

        private void ProcessInvocation(InvocationExpression expression, InvocationExpressionNode expressionNode, ExpressionStack stack)
        {

        }
        private void ProcessLambda(LambdaExpression expression, LambdaExpressionNode expressionNode, ExpressionStack stack)
        {

        }
        private void ProcessListInit(ListInitExpression expression, ListInitExpressionNode expressionNode, ExpressionStack stack)
        {

        }
        private void ProcessMember(MemberExpression expression, MemberExpressionNode expressionNode, ExpressionStack stack)
        {

        }
        private void ProcessMemberInit(MemberInitExpression expression, MemberInitExpressionNode expressionNode, ExpressionStack stack)
        {

        }
        private void ProcessMethodCall(MethodCallExpression expression, MethodCallExpressionNode expressionNode, ExpressionStack stack)
        {

        }
        private void ProcessNewArray(NewArrayExpression expression, NewArrayExpressionNode expressionNode, ExpressionStack stack)
        {

        }
        private void ProcessNew(NewExpression expression, NewExpressionNode expressionNode, ExpressionStack stack)
        {

        }
        private void ProcessParameter(ParameterExpression expression, ParameterExpressionNode expressionNode, ExpressionStack stack)
        {

        }
        private void ProcessTypeBinary(TypeBinaryExpression expression, TypeBinaryExpressionNode expressionNode, ExpressionStack stack)
        {

        }
        private void ProcessUnary(UnaryExpression expression, UnaryExpressionNode expressionNode, ExpressionStack stack)
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
