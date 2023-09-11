

public record class Node(string Name, IEdmType Type)
{

    public List<Node> Nodes { get; } = new List<Node>();


    public void Add(Node node)
    {
        Nodes.Add(node);
    }

    public void AddRange(IEnumerable<Node> node)
    {
        Nodes.AddRange(node);
    }

    public IEnumerable<(IEnumerable<string> Segments, IEdmType ResponseType)> Paths()
    {
        return
            from node in Nodes
            from path in node.Paths(ImmutableList<string>.Empty)
            select path;
    }

    public IEnumerable<(IEnumerable<string>, IEdmType)> Paths(ImmutableList<string> path)
    {
        path = path.Add(Name);
        yield return (path, Type);
        foreach (var node in Nodes)
        {
            foreach (var child in node.Paths(path))
            {
                yield return child;
            }
        }
    }
}
