//---------------------------------------------------------------------
// <copyright file="ModelGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Scalable ModelGenerator class
    /// </summary>
    public class ModelGenerator
    {
        /// <summary>
        /// Initializes a new instance of the ModelGenerator class
        /// </summary>
        public ModelGenerator()
        {
            this.Goals = new List<EntityModelGoal>();
            this.ValidationRules = new List<IEntityModelFixupWithValidate>();
        }

        /// <summary>
        /// Gets or sets Max number of iterations before the model stops enrichments and concentrate on correctness
        /// </summary>
        public int MaxRounds { get; set; }

        /// <summary>
        /// Gets Model enrichment goals
        /// </summary>
        public IList<EntityModelGoal> Goals { get; private set; }

        /// <summary>
        /// Gets the list of validation goals
        /// </summary>
        public IList<IEntityModelFixupWithValidate> ValidationRules { get; private set; }

        /// <summary>
        /// Sets the goal for model generation.
        /// </summary>
        /// <param name="newGoal">The new goal.</param>
        public void SetGoal(EntityModelGoal newGoal)
        {
            foreach (var goal in this.Goals.ToList())
            {
                if (goal.GetType() == newGoal.GetType())
                {
                    this.Goals.Remove(goal);
                }
            }

            this.Goals.Add(newGoal);
        }

        /// <summary>
        /// Generates the model.
        /// </summary>
        /// <returns>Generated model.</returns>
        public EntityModelSchema GenerateModel()
        {
            EntityModelSchema model = new EntityModelSchema();

            for (int i = 0; i < this.MaxRounds; ++i)
            {
                var goalsNotMet = this.Goals.Where(c => !c.HasBeenMet(model)).ToList();
                
                if (goalsNotMet.Count == 0)
                {
                    break;
                }

                foreach (var goal in goalsNotMet)
                {
                    goal.Improve(model);
                }
            }

            this.FixInvalidModel(model);
            return model;
        }
        
        private void FixInvalidModel(EntityModelSchema model)
        {
            var validationGoalsNotMet = this.ValidationRules.Where(c => !c.IsModelValid(model)).ToList();
            while (validationGoalsNotMet.Count() > 0)
            {
                foreach (var rule in validationGoalsNotMet)
                {
                    rule.Fixup(model);
                }

                validationGoalsNotMet = this.ValidationRules.Where(c => !c.IsModelValid(model)).ToList();
            }
        }
    }
}
