namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _customQueryOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._customQueryOption> Instance { get; } = from _customName_1 in __GeneratedOdataV2.Parsers.Rules._customNameParser.Instance
from _EQ_customValue_1 in __GeneratedOdataV2.Parsers.Inners._EQ_customValueParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._customQueryOption(_customName_1, _EQ_customValue_1.GetOrElse(null));
    }
    
}
