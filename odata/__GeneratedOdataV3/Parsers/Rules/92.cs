namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption> Instance { get; } = (_selectOptionPCParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOption>(_computeParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOption>(_selectParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOption>(_expandParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._selectOption>(_aliasAndValueParser.Instance);
        
        public static class _selectOptionPCParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._selectOptionPC> Instance { get; } = from _selectOptionPC_1 in __GeneratedOdataV3.Parsers.Rules._selectOptionPCParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectOption._selectOptionPC(_selectOptionPC_1);
        }
        
        public static class _computeParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._compute> Instance { get; } = from _compute_1 in __GeneratedOdataV3.Parsers.Rules._computeParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectOption._compute(_compute_1);
        }
        
        public static class _selectParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._select> Instance { get; } = from _select_1 in __GeneratedOdataV3.Parsers.Rules._selectParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectOption._select(_select_1);
        }
        
        public static class _expandParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._expand> Instance { get; } = from _expand_1 in __GeneratedOdataV3.Parsers.Rules._expandParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectOption._expand(_expand_1);
        }
        
        public static class _aliasAndValueParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._selectOption._aliasAndValue> Instance { get; } = from _aliasAndValue_1 in __GeneratedOdataV3.Parsers.Rules._aliasAndValueParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._selectOption._aliasAndValue(_aliasAndValue_1);
        }
    }
    
}
