namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _binaryParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._binary> Instance { get; } = from _ʺx62x69x6Ex61x72x79ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx62x69x6Ex61x72x79ʺParser.Instance
from _SQUOTE_1 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
from _binaryValue_1 in __GeneratedOdataV2.Parsers.Rules._binaryValueParser.Instance
from _SQUOTE_2 in __GeneratedOdataV2.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._binary(_ʺx62x69x6Ex61x72x79ʺ_1, _SQUOTE_1, _binaryValue_1, _SQUOTE_2);
    }
    
}
