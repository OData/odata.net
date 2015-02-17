//---------------------------------------------------------------------
// <copyright file="PseudoLocalizationFixup.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Modifies names of all types, members, containers and sets in the model by translating them using 
    /// a specified instance of <see cref="IPseudoLocalizer"/>.
    /// </summary>
    [ImplementationName(typeof(IGlobalizationFixup), "PLOC", HelpText = "Performs simple pseudo-localzation of all names in the model.")]
    public class PseudoLocalizationFixup : IEntityModelFixup, IGlobalizationFixup
    {
        private Dictionary<string, string> translations;

        /// <summary>
        /// Gets or sets the PseudoLocalizer
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IPseudoLocalizer PseudoLocalizer { get; set; }

        /// <summary>
        /// Modifies names of all types, members, containers and sets in the model by translating them using 
        /// a specified instance of <see cref="IPseudoLocalizer"/>.
        /// </summary>
        /// <param name="model">Model to perform fixup on</param>
        public virtual void Fixup(EntityModelSchema model)
        {
            this.translations = new Dictionary<string, string>();

            foreach (var type in model.EntityTypes)
            {
                type.Name = this.PseudoLocalize(type.Name);
                type.NamespaceName = this.PseudoLocalize(type.NamespaceName);
                this.PseudoLocalizeProperties(type.Properties);
                this.PseudoLocalizeNavigationProperties(type.NavigationProperties);
            }

            foreach (var type in model.ComplexTypes)
            {
                type.Name = this.PseudoLocalize(type.Name, true);
                type.NamespaceName = this.PseudoLocalize(type.NamespaceName);
                this.PseudoLocalizeProperties(type.Properties);
            }

            foreach (var association in model.Associations)
            {
                association.Name = this.PseudoLocalize(association.Name);
                association.NamespaceName = this.PseudoLocalize(association.NamespaceName);
                this.PseudoLocalizeEnds(association.Ends);
            }

            foreach (var container in model.EntityContainers)
            {
                container.Name = this.PseudoLocalize(container.Name);
                foreach (var set in container.EntitySets)
                {
                    set.Name = this.PseudoLocalize(set.Name);
                }

                foreach (var set in container.AssociationSets)
                {
                    set.Name = this.PseudoLocalize(set.Name);
                }
            }
        }

        /// <summary>
        /// Pseudo-localizes a given text.
        /// </summary>
        /// <param name="text">Text to pseudo-localize.</param>
        /// <returns>Pseudo-localized text.</returns>
        protected string PseudoLocalize(string text)
        {
            return this.PseudoLocalize(text, false);
        }

        /// <summary>
        /// Pseudo-localizes a given text.
        /// </summary>
        /// <param name="text">Text to pseudo-localize.</param>
        /// <param name="isPropertyName">Indicates if the item is a property name</param>
        /// <returns>Pseudo-localized text.</returns>
        protected string PseudoLocalize(string text, bool isPropertyName)
        {
            if (text == null)
            {
                return null;
            }

            string translatedText;

            if (!this.translations.TryGetValue(text, out translatedText))
            {
                translatedText = this.PseudoLocalizer.PseudoLocalize(text, isPropertyName);
                this.translations.Add(text, translatedText);
            }

            return translatedText;
        }

        private void PseudoLocalizeProperties(IEnumerable<MemberProperty> properties)
        {
            foreach (var prop in properties)
            {
                prop.Name = this.PseudoLocalize(prop.Name, true);
            }
        }

        private void PseudoLocalizeNavigationProperties(IEnumerable<NavigationProperty> navigationProperties)
        {
            foreach (var navprop in navigationProperties)
            {
                navprop.Name = this.PseudoLocalize(navprop.Name, true);
            }
        }

        private void PseudoLocalizeEnds(IEnumerable<AssociationEnd> ends)
        {
            foreach (var end in ends)
            {
                end.RoleName = this.PseudoLocalize(end.RoleName);
            }
        }
    }
}
