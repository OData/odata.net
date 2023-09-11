using System.Xml;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;

class Program
{

    private static void Main()
    {
        var args = Args.Parse();



        using var reader = XmlReader.Create(args.InputFile);
        if (!CsdlReader.TryParse(reader, out var model, out var errors))
        {
            Console.WriteLine(string.Join(Environment.NewLine, errors));
            return;
        }

        var analyzer = new ModelAnalyzer(model);
        var tree = analyzer.CreateTree();

        // format tree
        if (!args.HideTree)
        {
            using var writer = new TreeWriter(Console.Out, true);
            writer.Display(tree);
        }

        // list of paths
        if (args.ShowPaths)
        {
            foreach (var path in tree.Paths())
            {
                // just path
                // Console.WriteLine("{0}", path.Segments.SeparatedBy("/"));

                //  path and response type 
                Console.WriteLine("{0} \x1b[36m{1}\x1b[m", path.Segments.SeparatedBy("/"), path.ResponseType.Format());

                // //  path, response type and a procedure like signature
                // Console.WriteLine("{0}\n\t\x1b[36m{1}\x1b[m",
                //     path.Segments.SeparatedBy("/"),
                //     Signature(path));
            }
        }
    }


    static string Signature((IEnumerable<string> Segments, IEdmType ResponseType) path)
    {
        var parameters = string.Join(", ", path.Segments.Where(s => s.StartsWith('{')).Select(w => w.Trim('{', '}')));
        var name = string.Join("Of", path.Segments.Where(s => !s.StartsWith('{')).Select(s => s.Capitalize()).Reverse());

        return $"{name}({parameters}) -> {path.ResponseType.Format()}";
    }
}

class Args
{


    const string DEFAULT_INPUT = "data/directory.csdl.xml";


    public string InputFile { get; private set; } = null!;
    public bool ShowPaths { get; private set; }
    public bool HideTree { get; private set; }

    public static Args Parse()
    {
        var result = new Args();
        var defaultArgProvided = false;
        var args = Environment.GetCommandLineArgs();
        for (int i = 1; i < args.Length; i++)
        {
            var arg = args[i];
            switch (arg)
            {
                case "--paths":
                case "-p":
                    result.ShowPaths = true;
                    break;
                case "--no-tree":
                case "-t":
                    result.HideTree = true;
                    break;
                case string s when s.StartsWith("--"):
                    throw new Exception($"unknown option {arg}");
                default:
                    if (defaultArgProvided)
                    {
                        throw new Exception($"two default arguments provided");
                    }
                    else
                    {
                        result.InputFile = arg;
                        defaultArgProvided = true;
                    }
                    break;
            }
        }
        if (result.InputFile == null)
        {
            result.InputFile = DEFAULT_INPUT;
        }
        return result;
    }
}