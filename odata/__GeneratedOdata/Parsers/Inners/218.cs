namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_parameterNameParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_parameterName> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _parameterName_1 in __GeneratedOdata.Parsers.Rules._parameterNameParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_parameterName(_COMMA_1, _parameterName_1);
    }
    
}