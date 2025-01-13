namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _entityIdOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._entityIdOption> Instance { get; } = (_formatParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._entityIdOption>(_customQueryOptionParser.Instance);
        
        public static class _formatParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._entityIdOption._format> Instance { get; } = from _format_1 in __GeneratedOdata.Parsers.Rules._formatParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entityIdOption._format(_format_1);
        }
        
        public static class _customQueryOptionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._entityIdOption._customQueryOption> Instance { get; } = from _customQueryOption_1 in __GeneratedOdata.Parsers.Rules._customQueryOptionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entityIdOption._customQueryOption(_customQueryOption_1);
        }
    }
    
}
