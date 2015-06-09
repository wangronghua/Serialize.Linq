using System.Linq.Expressions;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Internals
{
    internal class ExpressionStack : PairStack<Expression, ExpressionNode> { }
}
