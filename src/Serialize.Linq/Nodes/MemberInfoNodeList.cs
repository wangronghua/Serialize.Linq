#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [CollectionDataContract]
#else
    [CollectionDataContract(Name = "MIL")]
#endif
#if !SILVERLIGHT
    [Serializable]
#endif
    #endregion
    public class MemberInfoNodeList : NodeList<MemberNode>
    {
        public MemberInfoNodeList()
            : base(NodeKind.MemberInfoList) { }

        public IEnumerable<MemberInfo> GetMembers(ExpressionContext context)
        {
            return this.Select(m => m.ToMemberInfo(context));
        }
    }
}
