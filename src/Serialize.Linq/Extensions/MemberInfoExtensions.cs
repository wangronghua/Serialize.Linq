#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Reflection;

namespace Serialize.Linq.Extensions
{
    /// <summary>
    /// MemberInfo extensions methods.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// Gets the return type of an member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">Unable to get return type of member of type  + member.MemberType</exception>
        public static Type GetReturnType(this MemberInfo member)
        {
            var property = member as PropertyInfo;
            if (property != null)
                return property.PropertyType;

            var method = member as MethodInfo;
            if (method != null)
                return method.ReturnType;

            var field = member as FieldInfo;
            if (field != null)
                return field.FieldType;

            throw new NotSupportedException("Unable to get return type of member of type " + member.GetType().Name);
        }
    }
}