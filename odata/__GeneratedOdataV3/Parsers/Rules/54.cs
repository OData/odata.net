namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _metadataOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._metadataOption> Instance { get; } = (_formatParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._metadataOption>(_customQueryOptionParser.Instance);
        
        public static class _formatParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._metadataOption._format> Instance { get; } = from _format_1 in __GeneratedOdataV3.Parsers.Rules._formatParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._metadataOption._format(_format_1);
        }
        
        public static class _customQueryOptionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._metadataOption._customQueryOption> Instance { get; } = from _customQueryOption_1 in __GeneratedOdataV3.Parsers.Rules._customQueryOptionParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._metadataOption._customQueryOption(_customQueryOption_1);
        }
    }
    
}
