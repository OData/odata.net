// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Design", "CA1005:Avoid excessive parameters on generic types",
    Justification = "The design of the library depends on having multiple generic types to provide flexibility while minimizing allocations.",
    Scope = "namespace",
    Target = "~N:Microsoft.OData.Serializer")]
