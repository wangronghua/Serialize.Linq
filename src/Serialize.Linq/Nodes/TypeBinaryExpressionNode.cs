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
    [DataContract(Name = "TB")]   
#endif
#if !SILVERLIGHT
    [Serializable]
#endif

    #endregion
    public class TypeBinaryExpressionNode : ExpressionNode
    {
        public TypeBinaryExpressionNode()
            : base(NodeKind.TypeBinaryExpression) { }

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
        [DataMember(EmitDefaultValue = false, Name = "O")]
#endif
        #endregion
        public TypeNode TypeOperand { get; set; }

        public override Expression ToExpression(ExpressionContext context)
        {
            return System.Linq.Expressions.Expression.TypeIs(this.Expression.ToExpression(context), this.TypeOperand.ToType(context));
        }
    }
}
