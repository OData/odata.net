namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _expandRefOptionParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._expandRefOption> Instance { get; } = (_expandCountOptionParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._expandRefOption>(_orderbyParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._expandRefOption>(_skipParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._expandRefOption>(_topParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._expandRefOption>(_inlinecountParser.Instance);
        
        public static class _expandCountOptionParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._expandRefOption._expandCountOption> Instance { get; } = from _expandCountOption_1 in __GeneratedOdataV2.Parsers.Rules._expandCountOptionParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._expandRefOption._expandCountOption(_expandCountOption_1);
        }
        
        public static class _orderbyParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._expandRefOption._orderby> Instance { get; } = from _orderby_1 in __GeneratedOdataV2.Parsers.Rules._orderbyParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._expandRefOption._orderby(_orderby_1);
        }
        
        public static class _skipParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._expandRefOption._skip> Instance { get; } = from _skip_1 in __GeneratedOdataV2.Parsers.Rules._skipParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._expandRefOption._skip(_skip_1);
        }
        
        public static class _topParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._expandRefOption._top> Instance { get; } = from _top_1 in __GeneratedOdataV2.Parsers.Rules._topParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._expandRefOption._top(_top_1);
        }
        
        public static class _inlinecountParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._expandRefOption._inlinecount> Instance { get; } = from _inlinecount_1 in __GeneratedOdataV2.Parsers.Rules._inlinecountParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._expandRefOption._inlinecount(_inlinecount_1);
        }
    }
    
}
