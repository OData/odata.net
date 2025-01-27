namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _complexPropertyⳆcomplexColPropertyParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._complexPropertyⳆcomplexColProperty> Instance { get; } = (_complexPropertyParser.Instance).Or<char, __GeneratedOdata.CstNodes.Inners._complexPropertyⳆcomplexColProperty>(_complexColPropertyParser.Instance);
        
        public static class _complexPropertyParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexProperty> Instance { get; } = from _complexProperty_1 in __GeneratedOdata.Parsers.Rules._complexPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexProperty(_complexProperty_1);
        }
        
        public static class _complexColPropertyParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexColProperty> Instance { get; } = from _complexColProperty_1 in __GeneratedOdata.Parsers.Rules._complexColPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexColProperty(_complexColProperty_1);
        }
    }
    
}
