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
    [DataContract(Name = "MB")]
#endif
#if !SILVERLIGHT
    [Serializable]
#endif
    #endregion
    public abstract class MemberBindingNode : Node
    {
        protected MemberBindingNode(NodeKind nodeKind)
            : base (nodeKind) { }

        /*

        protected MemberBindingNode(INodeFactory factory)
            : base(factory) { }

        protected MemberBindingNode(INodeFactory factory, MemberBindingType bindingType, MemberInfo memberInfo)
            : base(factory)
        {
            this.BindingType = bindingType;
            this.Member = new MemberInfoNode(this.Factory, memberInfo);
        }
        */
        
        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "BT")]
#endif
        #endregion
        public MemberBindingType BindingType { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "M")]
#endif
        #endregion
        public MemberNode Member { get; set; }

        internal abstract MemberBinding ToMemberBinding(ExpressionContext context);
    }
}
