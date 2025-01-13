namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _expandRefOptionParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._expandRefOption> Instance { get; } = (_expandCountOptionParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandRefOption>(_orderbyParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandRefOption>(_skipParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandRefOption>(_topParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._expandRefOption>(_inlinecountParser.Instance);
        
        public static class _expandCountOptionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandRefOption._expandCountOption> Instance { get; } = from _expandCountOption_1 in __GeneratedOdata.Parsers.Rules._expandCountOptionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandRefOption._expandCountOption(_expandCountOption_1);
        }
        
        public static class _orderbyParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandRefOption._orderby> Instance { get; } = from _orderby_1 in __GeneratedOdata.Parsers.Rules._orderbyParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandRefOption._orderby(_orderby_1);
        }
        
        public static class _skipParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandRefOption._skip> Instance { get; } = from _skip_1 in __GeneratedOdata.Parsers.Rules._skipParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandRefOption._skip(_skip_1);
        }
        
        public static class _topParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandRefOption._top> Instance { get; } = from _top_1 in __GeneratedOdata.Parsers.Rules._topParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandRefOption._top(_top_1);
        }
        
        public static class _inlinecountParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._expandRefOption._inlinecount> Instance { get; } = from _inlinecount_1 in __GeneratedOdata.Parsers.Rules._inlinecountParser.Instance
select new __GeneratedOdata.CstNodes.Rules._expandRefOption._inlinecount(_inlinecount_1);
        }
    }
    
}
