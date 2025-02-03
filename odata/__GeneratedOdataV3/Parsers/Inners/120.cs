namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _complexPropertyⳆcomplexColPropertyParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexPropertyⳆcomplexColProperty> Instance { get; } = (_complexPropertyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._complexPropertyⳆcomplexColProperty>(_complexColPropertyParser.Instance);
        
        public static class _complexPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexProperty> Instance { get; } = from _complexProperty_1 in __GeneratedOdataV3.Parsers.Rules._complexPropertyParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexProperty(_complexProperty_1);
        }
        
        public static class _complexColPropertyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexColProperty> Instance { get; } = from _complexColProperty_1 in __GeneratedOdataV3.Parsers.Rules._complexColPropertyParser.Instance
select new __GeneratedOdataV3.CstNodes.Inners._complexPropertyⳆcomplexColProperty._complexColProperty(_complexColProperty_1);
        }
    }
    
}
