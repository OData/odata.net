//---------------------------------------------------------------------
// <copyright file="IUpdatable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System.Linq;

    /// <summary>
    /// This interface declares the methods required for supporting
    /// update of resources
    /// </summary>
    public interface IUpdatable
    {
        /// <summary>Creates the resource of the specified type and that belongs to the specified container.</summary>
        /// <returns>The object representing a resource of specified type and belonging to the specified container.</returns>
        /// <param name="containerName">The name of the entity set to which the resource belongs.</param>
        /// <param name="fullTypeName">The full namespace-qualified type name of the resource.</param>
        object CreateResource(string containerName, string fullTypeName);

        /// <summary>Gets the resource of the specified type identified by a query and type name. </summary>
        /// <returns>An opaque object representing a resource of the specified type, referenced by the specified query.</returns>
        /// <param name="query">Language integrated query (LINQ) pointing to a particular resource.</param>
        /// <param name="fullTypeName">The fully qualified type name of resource.</param>
        object GetResource(IQueryable query, string fullTypeName);

        /// <summary>Resets the resource identified by the parameter <paramref name="resource " />to its default value.</summary>
        /// <returns>The resource with its value reset to the default value.</returns>
        /// <param name="resource">The resource to be updated.</param>
        object ResetResource(object resource);

        /// <summary>Sets the value of the property with the specified name on the target resource to the specified property value.</summary>
        /// <param name="targetResource">The target object that defines the property.</param>
        /// <param name="propertyName">The name of the property whose value needs to be updated.</param>
        /// <param name="propertyValue">The property value for update.</param>
        void SetValue(object targetResource, string propertyName, object propertyValue);

        /// <summary>Gets the value of the specified property on the target object.</summary>
        /// <returns>The value of the object.</returns>
        /// <param name="targetResource">An opaque object that represents a resource.</param>
        /// <param name="propertyName">The name of the property whose value needs to be retrieved.</param>
        object GetValue(object targetResource, string propertyName);

        /// <summary>Sets the value of the specified reference property on the target object.</summary>
        /// <param name="targetResource">The target object that defines the property.</param>
        /// <param name="propertyName">The name of the property whose value needs to be updated.</param>
        /// <param name="propertyValue">The property value to be updated.</param>
        void SetReference(object targetResource, string propertyName, object propertyValue);

        /// <summary>Adds the specified value to the collection.</summary>
        /// <param name="targetResource">Target object that defines the property.</param>
        /// <param name="propertyName">The name of the collection property to which the resource should be added..</param>
        /// <param name="resourceToBeAdded">The opaque object representing the resource to be added.</param>
        void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded);

        /// <summary>Removes the specified value from the collection.</summary>
        /// <param name="targetResource">The target object that defines the property.</param>
        /// <param name="propertyName">The name of the property whose value needs to be updated.</param>
        /// <param name="resourceToBeRemoved">The property value that needs to be removed.</param>
        void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved);

        /// <summary>Deletes the specified resource.</summary>
        /// <param name="targetResource">The resource to be deleted.</param>
        void DeleteResource(object targetResource);

        /// <summary>Saves all the changes that have been made by using the <see cref="T:Microsoft.OData.Service.IUpdatable" /> APIs.</summary>
        void SaveChanges();

        /// <summary>Returns the instance of the resource represented by the specified resource object.</summary>
        /// <returns>Returns the instance of the resource represented by the specified resource object.</returns>
        /// <param name="resource">The object representing the resource whose instance needs to be retrieved.</param>
        object ResolveResource(object resource);

        /// <summary>Cancels a change to the data.</summary>
        void ClearChanges();
    }
}
