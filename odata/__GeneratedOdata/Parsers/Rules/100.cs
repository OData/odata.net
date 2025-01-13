namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _nameAndValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._nameAndValue> Instance { get; } = from _parameterName_1 in __GeneratedOdata.Parsers.Rules._parameterNameParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _parameterValue_1 in __GeneratedOdata.Parsers.Rules._parameterValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._nameAndValue(_parameterName_1, _EQ_1, _parameterValue_1);
    }
    
}
