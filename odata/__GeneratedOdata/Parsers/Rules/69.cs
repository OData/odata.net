namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _expandOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._expandOption> Instance { get; } = (_expandRefOptionParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandOption>(_selectParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandOption>(_expandParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandOption>(_computeParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandOption>(_levelsParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandOption>(_aliasAndValueParser.Instance);
        
        public static class _expandRefOptionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandOption._expandRefOption> Instance { get; } = from _expandRefOption_1 in __GeneratedOdata.Parsers.Rules._expandRefOptionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandOption._expandRefOption(_expandRefOption_1);
        }
        
        public static class _selectParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandOption._select> Instance { get; } = from _select_1 in __GeneratedOdata.Parsers.Rules._selectParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandOption._select(_select_1);
        }
        
        public static class _expandParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandOption._expand> Instance { get; } = from _expand_1 in __GeneratedOdata.Parsers.Rules._expandParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandOption._expand(_expand_1);
        }
        
        public static class _computeParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandOption._compute> Instance { get; } = from _compute_1 in __GeneratedOdata.Parsers.Rules._computeParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandOption._compute(_compute_1);
        }
        
        public static class _levelsParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandOption._levels> Instance { get; } = from _levels_1 in __GeneratedOdata.Parsers.Rules._levelsParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandOption._levels(_levels_1);
        }
        
        public static class _aliasAndValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandOption._aliasAndValue> Instance { get; } = from _aliasAndValue_1 in __GeneratedOdata.Parsers.Rules._aliasAndValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandOption._aliasAndValue(_aliasAndValue_1);
        }
    }
    
}