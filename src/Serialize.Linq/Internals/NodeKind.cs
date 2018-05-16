namespace Serialize.Linq.Internals
{
    internal enum NodeKind
    {
        BinaryExpression, ConditionalExpression, ConstantExpression, InvocationExpression, LambdaExpression,
        ListInitExpression, MemberExpression, MemberInitExpression, MethodCallExpression, NewArrayExpression,
        NewExpression, ParameterExpression, TypeBinaryExpression, UnaryExpression, ExpressionList,

        ConstructorInfo, FieldInfo, MemberInfo, MethodInfo, PropertyInfo, MemberInfoList,
        ElementInit, ElementInitList, Type,

        MemberMemberBinding, MemberListBinding, MemberAssignment, MemberBindingList
    }
}
