public class ModelAnalyzer
{
    private readonly IEdmModel Model;

    public ModelAnalyzer(IEdmModel model)
    {
        Model = model;
    }

    public Node CreateTree()
    {
        return Unfold(Model.EntityContainer, ImmutableList<(string, IEdmType)>.Empty);
    }

    private Node Unfold(IEdmEntityContainer entityContainer, ImmutableList<(string, IEdmType)> visited)
    {
        var node = new Node("", EdmUntypedStructuredType.Instance);

        // instead of iterating twice over container elements via EntitySets and Singletons extension methods
        // iterate once and do the type-check/cast inline in a switch
        foreach (var element in entityContainer.Elements)
        {
            switch (element)
            {
                case IEdmEntitySet entitySet:
                    node.Add(UnfoldEntitySet(entitySet, visited));
                    break;
                case IEdmSingleton singleton:
                    node.Add(UnfoldSingleton(visited, singleton));
                    break;
                default:
                    // intentionally left blank. other container elements are not supported yet.
                    Console.Error.WriteLine("ignoring {0} {1}", element.ContainerElementKind, element.Name);
                    break;
            }
        }
        return node;
    }

    private Node UnfoldSingleton(ImmutableList<(string, IEdmType)> visited, IEdmSingleton singleton)
    {
        if (singleton.Type is IEdmEntityType singletonType)
        {
            return UnfoldStructuredType(singleton.Name, singletonType, visited);
        }
        throw new NotSupportedException("singleton type not a entity type");
    }

    private Node UnfoldEntitySet(IEdmEntitySet entitySet, ImmutableList<(string, IEdmType)> visited)
    {
        if (entitySet.Type is IEdmCollectionType collectionType)
        {
            return UnfoldCollectionType(entitySet.Name, collectionType, visited);
        }
        throw new NotSupportedException("EntitySet type not a collection of entity types");
    }


    private Node UnfoldStructuredType(string segment, IEdmStructuredType structuredType, ImmutableList<(string, IEdmType)> visited)
    {
        var node = new Node(segment, structuredType);
        visited = visited.Add((segment, structuredType));

        if (BreakLoop(visited, out var loop))
        {
            node.Add(loop);
            return node;
        }

        foreach (var subtype in Model.FindAllDerivedTypes(structuredType))
        {
            node.Add(UnfoldStructuredType(subtype.FullTypeName(), subtype, visited));
        }

        // Get all properties, not just navigation properties 
        // This will generate navigation paths that navigate through a structural property (e.g.  /Suppliers/{ID}/Address/Country: in example 89)
        // If the property type is neither complex nor entity, nothing will be (yield) returned in the switch statement
        // TODO: deal with type definitions
        foreach (var property in structuredType.Properties())
        {
            // var node = new Node(property.Name, property.Type.Definition);
            switch (property.Type.Definition)
            {
                case IEdmStructuredType propertyStructuredType:
                    node.Add(UnfoldStructuredType(property.Name, propertyStructuredType, visited));
                    break;

                case IEdmCollectionType collectionType:
                    node.Add(UnfoldCollectionType(property.Name, collectionType, visited));
                    break;
            }
        }
        return node;
    }

    private bool BreakLoop(ImmutableList<(string, IEdmType)> visited, [MaybeNullWhen(false)] out Node node)
    {
        // if we visited the type, return one last path and stop recursion
        // this can only happen if there is al least two elements
        if (visited.Count >= 2)
        {
            var type = visited.Last().Item2;
            var ix = visited.FindLastIndex(visited.Count - 2, p => p.Item2 == type);
            if (ix >= 0)
            {
                var tail = visited.Skip(ix + 1).Select(p => p.Item1).ToList();
                node = new Node($"{{ ({string.Join("/", tail)})+ }}", type);
                return true;
            }
        }
        node = default;
        return false;
    }

    private Node UnfoldCollectionType(string segment, IEdmCollectionType collectionType, ImmutableList<(string, IEdmType)> visited)
    {
        var node = new Node(segment, collectionType);
        visited = visited.Add((segment, collectionType));

        if (!(collectionType.ElementType.Definition is IEdmEntityType elementType))
        {
            throw new NotSupportedException("IEdmCollectionType's element type is not a entity type");
        }

        var keys = elementType.Key();
        if (!keys.TryGetSingle(out var key))
        {
            throw new NotSupportedException("multipart keys are not supported");
        }

        node.Add(UnfoldStructuredType($"{{{key.Name}}}", elementType, visited));

        return node;
    }
}

