//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
