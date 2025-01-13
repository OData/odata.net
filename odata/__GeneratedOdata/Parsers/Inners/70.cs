namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_entitySetNameParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_entitySetName> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _entitySetName_1 in __GeneratedOdata.Parsers.Rules._entitySetNameParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_entitySetName(_COMMA_1, _entitySetName_1);
    }
    
}
