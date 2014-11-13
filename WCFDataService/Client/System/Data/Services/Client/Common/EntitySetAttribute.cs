//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Common
{
    /// <summary>Indicates the entity set to which a client data service class belongs.</summary>
        /// <remarks>
        /// This attribute is generated only when there is one entity set associated with the type.
        /// When there are more than one entity set associated with the type, then the entity set
        /// name can be passed in through the EntitySetNameResolver event.
        /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class EntitySetAttribute : System.Attribute
    {
        /// <summary>
        /// The entity set name.
        /// </summary>
        private readonly string entitySet;

        /// <summary>Creates a new instance of the <see cref="T:System.Data.Services.Common.EntitySetAttribute" />.</summary>
        /// <param name="entitySet">The entity set to which the class belongs.</param>
        public EntitySetAttribute(string entitySet)
        {
            this.entitySet = entitySet;
        }

        /// <summary>Gets the entity set to which the class belongs.</summary>
        /// <returns>The entity set as string value. </returns>
        public string EntitySet
        {
            get
            {
                return this.entitySet;
            }
        }
    }
}
