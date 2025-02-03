namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri> Instance { get; } = (_annotationInUriParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri>(_primitivePropertyInUriParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri>(_complexPropertyInUriParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri>(_collectionPropertyInUriParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri>(_navigationPropertyInUriParser.Instance);
        
        public static class _annotationInUriParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._annotationInUri> Instance { get; } = from _annotationInUri_1 in __GeneratedOdataV2.Parsers.Rules._annotationInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._annotationInUri(_annotationInUri_1);
        }
        
        public static class _primitivePropertyInUriParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._primitivePropertyInUri> Instance { get; } = from _primitivePropertyInUri_1 in __GeneratedOdataV2.Parsers.Rules._primitivePropertyInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._primitivePropertyInUri(_primitivePropertyInUri_1);
        }
        
        public static class _complexPropertyInUriParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._complexPropertyInUri> Instance { get; } = from _complexPropertyInUri_1 in __GeneratedOdataV2.Parsers.Rules._complexPropertyInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._complexPropertyInUri(_complexPropertyInUri_1);
        }
        
        public static class _collectionPropertyInUriParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._collectionPropertyInUri> Instance { get; } = from _collectionPropertyInUri_1 in __GeneratedOdataV2.Parsers.Rules._collectionPropertyInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._collectionPropertyInUri(_collectionPropertyInUri_1);
        }
        
        public static class _navigationPropertyInUriParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._navigationPropertyInUri> Instance { get; } = from _navigationPropertyInUri_1 in __GeneratedOdataV2.Parsers.Rules._navigationPropertyInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._navigationPropertyInUri(_navigationPropertyInUri_1);
        }
    }
    
}
