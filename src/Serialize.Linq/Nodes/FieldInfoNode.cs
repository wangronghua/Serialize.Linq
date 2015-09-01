#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "FI")]
#endif
#if !SILVERLIGHT
    [Serializable]
#endif
    #endregion
    public class FieldInfoNode : MemberNode<FieldInfo>
    {
        public FieldInfoNode()
            : base(NodeKind.FieldInfo) { }

        protected override IEnumerable<FieldInfo> GetMemberInfosForType(ExpressionContext context, Type type)
        {
            return type.GetFields();
        }
    }
}