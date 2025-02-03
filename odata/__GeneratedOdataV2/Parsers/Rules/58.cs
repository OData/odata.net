namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityCastOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entityCastOption> Instance { get; } = (_entityIdOptionParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._entityCastOption>(_expandParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._entityCastOption>(_selectParser.Instance);
        
        public static class _entityIdOptionParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entityCastOption._entityIdOption> Instance { get; } = from _entityIdOption_1 in __GeneratedOdataV2.Parsers.Rules._entityIdOptionParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._entityCastOption._entityIdOption(_entityIdOption_1);
        }
        
        public static class _expandParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entityCastOption._expand> Instance { get; } = from _expand_1 in __GeneratedOdataV2.Parsers.Rules._expandParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._entityCastOption._expand(_expand_1);
        }
        
        public static class _selectParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._entityCastOption._select> Instance { get; } = from _select_1 in __GeneratedOdataV2.Parsers.Rules._selectParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._entityCastOption._select(_select_1);
        }
    }
    
}
