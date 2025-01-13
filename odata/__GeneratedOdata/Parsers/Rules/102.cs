namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _customQueryOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._customQueryOption> Instance { get; } = from _customName_1 in __GeneratedOdata.Parsers.Rules._customNameParser.Instance
from _EQ_customValue_1 in __GeneratedOdata.Parsers.Inners._EQ_customValueParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._customQueryOption(_customName_1, _EQ_customValue_1.GetOrElse(null));
    }
    
}
