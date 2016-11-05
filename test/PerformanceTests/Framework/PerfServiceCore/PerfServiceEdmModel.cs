//---------------------------------------------------------------------
// <copyright file="PerfServiceEdmModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.PerfService
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;

    public static class PerfServiceEdmModel
    {
        public static IEdmModel CreateServiceEdmModel(string ns)
        {
            EdmModel model = new EdmModel();
            var defaultContainer = new EdmEntityContainer(ns, "PerfInMemoryContainer");
            model.AddElement(defaultContainer);

            var personType = new EdmEntityType(ns, "Person");
            var personIdProperty = new EdmStructuralProperty(personType, "PersonID", EdmCoreModel.Instance.GetInt32(false));
            personType.AddProperty(personIdProperty);
            personType.AddKeys(new IEdmStructuralProperty[] { personIdProperty });
            personType.AddProperty(new EdmStructuralProperty(personType, "FirstName", EdmCoreModel.Instance.GetString(false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "LastName", EdmCoreModel.Instance.GetString(false)));
            personType.AddProperty(new EdmStructuralProperty(personType, "MiddleName", EdmCoreModel.Instance.GetString(true)));
            personType.AddProperty(new EdmStructuralProperty(personType, "Age", EdmCoreModel.Instance.GetInt32(false)));
            model.AddElement(personType);

            var simplePersonSet = new EdmEntitySet(defaultContainer, "SimplePeopleSet", personType);
            defaultContainer.AddElement(simplePersonSet);

            var largetPersonSet = new EdmEntitySet(defaultContainer, "LargePeopleSet", personType);
            defaultContainer.AddElement(largetPersonSet);

            var addressType = new EdmComplexType(ns, "Address");
            addressType.AddProperty(new EdmStructuralProperty(addressType, "Street", EdmCoreModel.Instance.GetString(false)));
            addressType.AddProperty(new EdmStructuralProperty(addressType, "City", EdmCoreModel.Instance.GetString(false)));
            addressType.AddProperty(new EdmStructuralProperty(addressType, "PostalCode", EdmCoreModel.Instance.GetString(false)));
            model.AddElement(addressType);

            var companyType = new EdmEntityType(ns, "Company");
            var companyId = new EdmStructuralProperty(companyType, "CompanyID", EdmCoreModel.Instance.GetInt32(false));
            companyType.AddProperty(companyId);
            companyType.AddKeys(companyId);
            companyType.AddProperty(new EdmStructuralProperty(companyType, "Name", EdmCoreModel.Instance.GetString(true)));
            companyType.AddProperty(new EdmStructuralProperty(companyType, "Address", new EdmComplexTypeReference(addressType, true)));
            companyType.AddProperty(new EdmStructuralProperty(companyType, "Revenue", EdmCoreModel.Instance.GetInt32(false)));

            model.AddElement(companyType);

            var companySet = new EdmEntitySet(defaultContainer, "CompanySet", companyType);
            defaultContainer.AddElement(companySet);

            var companyEmployeeNavigation = companyType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo()
            {
                Name = "Employees",
                Target = personType,
                TargetMultiplicity = EdmMultiplicity.Many
            });
            companySet.AddNavigationTarget(companyEmployeeNavigation, largetPersonSet);

            // ResetDataSource
            var resetDataSourceAction = new EdmAction(ns, "ResetDataSource", null, false, null);
            model.AddElement(resetDataSourceAction);
            defaultContainer.AddActionImport(resetDataSourceAction);

            return model;
        }
    }
}
