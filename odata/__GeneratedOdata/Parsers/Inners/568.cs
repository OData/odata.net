namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_preferenceParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_preference> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _preference_1 in __GeneratedOdata.Parsers.Rules._preferenceParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_preference(_COMMA_1, _preference_1);
    }
    
}