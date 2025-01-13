namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _EQ_customValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._EQ_customValue> Instance { get; } = from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _customValue_1 in __GeneratedOdata.Parsers.Rules._customValueParser.Instance
select new __GeneratedOdata.CstNodes.Inners._EQ_customValue(_EQ_1, _customValue_1);
    }
    
}
