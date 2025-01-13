namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _entityCastOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._entityCastOption> Instance { get; } = (_entityIdOptionParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._entityCastOption>(_expandParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._entityCastOption>(_selectParser.Instance);
        
        public static class _entityIdOptionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._entityCastOption._entityIdOption> Instance { get; } = from _entityIdOption_1 in __GeneratedOdata.Parsers.Rules._entityIdOptionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entityCastOption._entityIdOption(_entityIdOption_1);
        }
        
        public static class _expandParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._entityCastOption._expand> Instance { get; } = from _expand_1 in __GeneratedOdata.Parsers.Rules._expandParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entityCastOption._expand(_expand_1);
        }
        
        public static class _selectParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._entityCastOption._select> Instance { get; } = from _select_1 in __GeneratedOdata.Parsers.Rules._selectParser.Instance
select new __GeneratedOdata.CstNodes.Rules._entityCastOption._select(_select_1);
        }
    }
    
}
