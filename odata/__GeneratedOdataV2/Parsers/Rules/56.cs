namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityIdOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entityIdOption> Instance { get; } = (_formatParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._entityIdOption>(_customQueryOptionParser.Instance);
        
        public static class _formatParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entityIdOption._format> Instance { get; } = from _format_1 in __GeneratedOdataV2.Parsers.Rules._formatParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._entityIdOption._format(_format_1);
        }
        
        public static class _customQueryOptionParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entityIdOption._customQueryOption> Instance { get; } = from _customQueryOption_1 in __GeneratedOdataV2.Parsers.Rules._customQueryOptionParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._entityIdOption._customQueryOption(_customQueryOption_1);
        }
    }
    
}
