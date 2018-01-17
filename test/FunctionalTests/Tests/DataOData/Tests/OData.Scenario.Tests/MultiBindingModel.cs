using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;

namespace Microsoft.Test.Taupo.OData.Scenario.Tests
{
    internal class MultiBindingModel
    {
        public static IEdmModel GetModel()
        {
            var model = new EdmModel();

            var entityType = new EdmEntityType("NS", "EntityType");
            var id = entityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            entityType.AddKeys(id);

            var derivedEntityType = new EdmEntityType("NS", "DerivedEntityType", entityType);

            var containedEntityType = new EdmEntityType("NS", "ContainedEntityType");
            var containedId = containedEntityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            containedEntityType.AddKeys(containedId);

            var containedNav1 = entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "ContainedNav1",
                Target = containedEntityType,
                TargetMultiplicity = EdmMultiplicity.One,
                ContainsTarget = true
            });

            var containedNav2 = entityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "ContainedNav2",
                Target = containedEntityType,
                TargetMultiplicity = EdmMultiplicity.One,
                ContainsTarget = true
            });

            var navEntityType = new EdmEntityType("NS", "NavEntityType");
            var navEntityId = navEntityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetString(false));
            navEntityType.AddKeys(navEntityId);

            var nestedNavEntityType = new EdmEntityType("NS", "NestedNavEntityType");
            var nestedId = nestedNavEntityType.AddStructuralProperty("NestedId", EdmCoreModel.Instance.GetString(false));
            nestedNavEntityType.AddKeys(nestedId);

            var nestNav = navEntityType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "NavNested",
                Target = nestedNavEntityType,
                TargetMultiplicity = EdmMultiplicity.One
            });

            var complex = new EdmComplexType("NS", "ComplexType");
            complex.AddStructuralProperty("Prop1", EdmCoreModel.Instance.GetString(false));

            var derivedComplex = new EdmComplexType("NS", "DerivedComplexType", complex);
            derivedComplex.AddStructuralProperty("DerivedProp", EdmCoreModel.Instance.GetString(false));

            var derivedNav = derivedEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "NavOnDerived",
                    Target = navEntityType,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            var complxNavP = complex.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "CollectionOfNavOnComplex",
                    Target = navEntityType,
                    TargetMultiplicity = EdmMultiplicity.Many,
                });

            entityType.AddStructuralProperty("complexProp1", new EdmComplexTypeReference(complex, false));
            entityType.AddStructuralProperty("complexProp2", new EdmComplexTypeReference(complex, false));

            var navOnContained = containedEntityType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo()
                {
                    Name = "NavOnContained",
                    Target = navEntityType,
                    TargetMultiplicity = EdmMultiplicity.One,
                });

            model.AddElement(entityType);
            model.AddElement(derivedEntityType);
            model.AddElement(containedEntityType);
            model.AddElement(navEntityType);
            model.AddElement(nestedNavEntityType);
            model.AddElement(complex);
            model.AddElement(derivedComplex);

            var entityContainer = new EdmEntityContainer("NS", "Container");
            model.AddElement(entityContainer);
            var entitySet = new EdmEntitySet(entityContainer, "EntitySet", entityType);
            var navEntitySet1 = new EdmEntitySet(entityContainer, "NavEntitySet1", navEntityType);
            var navEntitySet2 = new EdmEntitySet(entityContainer, "NavEntitySet2", navEntityType);
            var nestNavEntitySet = new EdmEntitySet(entityContainer, "NestedEntitySet", nestedNavEntityType);
            navEntitySet1.AddNavigationTarget(nestNav, nestNavEntitySet);
            navEntitySet2.AddNavigationTarget(nestNav, nestNavEntitySet);
            entitySet.AddNavigationTarget(derivedNav, navEntitySet1, new EdmPathExpression("NS.DerivedEntityType/NavOnDerived"));
            entitySet.AddNavigationTarget(complxNavP, navEntitySet1, new EdmPathExpression("complexProp1/CollectionOfNavOnComplex"));
            entitySet.AddNavigationTarget(complxNavP, navEntitySet2, new EdmPathExpression("complexProp2/CollectionOfNavOnComplex"));
            entitySet.AddNavigationTarget(navOnContained, navEntitySet1, new EdmPathExpression("ContainedNav1/NavOnContained"));
            entitySet.AddNavigationTarget(navOnContained, navEntitySet2, new EdmPathExpression("ContainedNav2/NavOnContained"));
            entityContainer.AddElement(entitySet);
            entityContainer.AddElement(navEntitySet1);
            entityContainer.AddElement(navEntitySet2);
            entityContainer.AddElement(nestNavEntitySet);

            return model;
        }
    }
}
