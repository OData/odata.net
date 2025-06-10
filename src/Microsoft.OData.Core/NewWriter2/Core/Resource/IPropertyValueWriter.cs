using System.Threading.Tasks;

namespace Microsoft.OData.Core.NewWriter2;

/// <summary>
/// Handles serializing the value of a property of a resource.
/// </summary>
/// <typeparam name="TContext">Represents session-wide context data.</typeparam>
/// <typeparam name="TState">Represents state that changes from one scope to another.</typeparam>
/// <typeparam name="TResource">The type of resource we're reading the property from.</typeparam>
/// <typeparam name="TProperty">The type representing the property information.</typeparam>
internal interface IPropertyValueWriter<TContext, TState, TResource, TProperty>
{
    ValueTask WritePropertyValue(TResource resource, TProperty property, TState state, TContext context);
}
