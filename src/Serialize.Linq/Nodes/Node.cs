#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Runtime.Serialization;

namespace Serialize.Linq.Nodes
{
    public enum NodeKind
    {
        BinaryExpression, ConditionalExpression, ConstantExpression, InvocationExpression, LambdaExpression,
        ListInitExpression, MemberExpression, MemberInitExpression, MethodCallExpression, NewArrayExpression,
        NewExpression, ParameterExpression, TypeBinaryExpression, UnaryExpression, ExpressionList,

        ConstructorInfo, FieldInfo, MemberInfo, MethodInfo, PropertyInfo, MemberInfoList,
        ElementInit, ElementInitList, Type,

        MemberMemberBinding, MemberListBinding, MemberAssignment, MemberBindingList
    }

#region DataContract
    [DataContract]
#if !SILVERLIGHT
    [Serializable]
#endif
    #region KnownTypes
    [KnownType(typeof(BinaryExpressionNode))]
    [KnownType(typeof(ConditionalExpressionNode))]
    [KnownType(typeof(ConstantExpressionNode))]
    [KnownType(typeof(ConstructorInfoNode))]
    [KnownType(typeof(ElementInitNode))]
    [KnownType(typeof(ElementInitNodeList))]
    [KnownType(typeof(ExpressionNode))]
    [KnownType(typeof(ExpressionNodeList))]
    [KnownType(typeof(FieldInfoNode))]
    [KnownType(typeof(InvocationExpressionNode))]
    [KnownType(typeof(LambdaExpressionNode))]
    [KnownType(typeof(ListInitExpressionNode))]
    [KnownType(typeof(MemberAssignmentNode))]
    [KnownType(typeof(MemberBindingNode))]
    [KnownType(typeof(MemberBindingNodeList))]
    [KnownType(typeof(MemberExpressionNode))]
    [KnownType(typeof(MemberNode))]
    [KnownType(typeof(MemberInfoNode))]
    [KnownType(typeof(MemberInfoNodeList))]    
    [KnownType(typeof(MemberInitExpressionNode))]
    [KnownType(typeof(MemberListBindingNode))]
    [KnownType(typeof(MemberMemberBindingNode))]
    [KnownType(typeof(MethodCallExpressionNode))]
    [KnownType(typeof(NewArrayExpressionNode))]
    [KnownType(typeof(NewExpressionNode))]
    [KnownType(typeof(ParameterExpressionNode))]
    [KnownType(typeof(PropertyInfoNode))]    
    [KnownType(typeof(TypeBinaryExpressionNode))]
    [KnownType(typeof(TypeNode))]
    [KnownType(typeof(UnaryExpressionNode))]
    #endregion
    #endregion
    public abstract class Node
    {
        protected Node(NodeKind nodeKind)
        {
            this.NodeKind = nodeKind;
        }

        [IgnoreDataMember]
        internal NodeKind NodeKind { get; private set; }
    }
}
