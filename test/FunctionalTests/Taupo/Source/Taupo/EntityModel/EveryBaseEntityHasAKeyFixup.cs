//---------------------------------------------------------------------
// <copyright file="EveryBaseEntityHasAKeyFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// NumberOfKeysPerEntityGoal class
    /// </summary>
    public class EveryBaseEntityHasAKeyFixup : IEntityModelFixupWithValidate
    {
        /// <summary>
        /// Checks whether goal has been met
        /// </summary>
        /// <param name="model">Model to enrich</param>
        /// <returns>Returns whether goal has been met</returns>
        public bool IsModelValid(EntityModelSchema model)
        {
            return model.EntityTypes.Where(p => p.BaseType == null).All(p => p.AllKeyProperties.Count() > 0);
        }

        /// <summary>
        /// Improve the model if goal not yet met
        /// </summary>
        /// <param name="model">Model to improve</param>        
        public void Fixup(EntityModelSchema model)
        {
            foreach (EntityType et in model.EntityTypes.Where(p => p.BaseType == null && p.AllKeyProperties.Count() == 0).ToList())
            {
                if (et.Properties.Count() > 0)
                {
                    et.Properties.First().IsPrimaryKey = true;
                }
                else
                {
                    MemberProperty mp = new MemberProperty();
                    mp.IsPrimaryKey = true;
                    et.Properties.Add(mp);
                }
            }
        }
    }
}
