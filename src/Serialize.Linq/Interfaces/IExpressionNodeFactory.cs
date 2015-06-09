using System.Linq.Expressions;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Interfaces
{
    public interface IExpressionNodeFactory
    {
        /// <summary>
        /// Creates the specified expression node an expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        ExpressionNode CreateExpressionNode(Expression expression);
    }
}
