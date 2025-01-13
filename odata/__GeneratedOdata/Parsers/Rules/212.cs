namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _nameⲻseparatorParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._nameⲻseparator> Instance { get; } = from _BWS_1 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
from _COLON_1 in __GeneratedOdata.Parsers.Rules._COLONParser.Instance
from _BWS_2 in __GeneratedOdata.Parsers.Rules._BWSParser.Instance
select new __GeneratedOdata.CstNodes.Rules._nameⲻseparator(_BWS_1, _COLON_1, _BWS_2);
    }
    
}
