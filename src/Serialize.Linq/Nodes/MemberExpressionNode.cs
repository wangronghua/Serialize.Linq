#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "M")]
#endif
#if !SILVERLIGHT
    [Serializable]
#endif
    #endregion
    public class MemberExpressionNode : ExpressionNode
    {
        public MemberExpressionNode()
            : base(NodeKind.MemberExpression) { }

        #region DataMember
        #if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "E")]
#endif
        #endregion
        public ExpressionNode Expression { get; set; }

        #region DataMember
        #if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "M")]
#endif
        #endregion
        public MemberNode Member { get; set; }

        public override Expression ToExpression(ExpressionContext context)
        {
            var member = this.Member.ToMemberInfo(context);
            return System.Linq.Expressions.Expression.MakeMemberAccess(this.Expression != null ? this.Expression.ToExpression(context) : null, member);
        }
    }
}
