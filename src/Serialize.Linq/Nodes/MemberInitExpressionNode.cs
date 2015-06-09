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
    [DataContract(Name = "MIE")]
#endif
#if !SILVERLIGHT
    [Serializable]
#endif
    #endregion
    public class MemberInitExpressionNode : ExpressionNode
    {
        public MemberInitExpressionNode()
            : base(NodeKind.MemberInitExpression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "B")]
#endif
        #endregion
        public MemberBindingNodeList Bindings { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "N")]
#endif
        #endregion
        public NewExpressionNode NewExpression { get; set; }

        public override Expression ToExpression(ExpressionContext context)
        {
            return Expression.MemberInit((NewExpression)this.NewExpression.ToExpression(context), this.Bindings.GetMemberBindings(context));
        }
    }
}
