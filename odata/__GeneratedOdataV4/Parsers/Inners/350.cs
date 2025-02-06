namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUriParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri> Instance { get; } = (_annotationInUriParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri>(_primitivePropertyInUriParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri>(_complexPropertyInUriParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri>(_collectionPropertyInUriParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri>(_navigationPropertyInUriParser.Instance);
        
        public static class _annotationInUriParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._annotationInUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._annotationInUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._annotationInUri> Parse(IInput<char>? input)
                {
                    var _annotationInUri_1 = __GeneratedOdataV4.Parsers.Rules._annotationInUriParser.Instance.Parse(input);
if (!_annotationInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._annotationInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._annotationInUri(_annotationInUri_1.Parsed), _annotationInUri_1.Remainder);
                }
            }
        }
        
        public static class _primitivePropertyInUriParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._primitivePropertyInUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._primitivePropertyInUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._primitivePropertyInUri> Parse(IInput<char>? input)
                {
                    var _primitivePropertyInUri_1 = __GeneratedOdataV4.Parsers.Rules._primitivePropertyInUriParser.Instance.Parse(input);
if (!_primitivePropertyInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._primitivePropertyInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._primitivePropertyInUri(_primitivePropertyInUri_1.Parsed), _primitivePropertyInUri_1.Remainder);
                }
            }
        }
        
        public static class _complexPropertyInUriParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._complexPropertyInUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._complexPropertyInUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._complexPropertyInUri> Parse(IInput<char>? input)
                {
                    var _complexPropertyInUri_1 = __GeneratedOdataV4.Parsers.Rules._complexPropertyInUriParser.Instance.Parse(input);
if (!_complexPropertyInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._complexPropertyInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._complexPropertyInUri(_complexPropertyInUri_1.Parsed), _complexPropertyInUri_1.Remainder);
                }
            }
        }
        
        public static class _collectionPropertyInUriParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._collectionPropertyInUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._collectionPropertyInUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._collectionPropertyInUri> Parse(IInput<char>? input)
                {
                    var _collectionPropertyInUri_1 = __GeneratedOdataV4.Parsers.Rules._collectionPropertyInUriParser.Instance.Parse(input);
if (!_collectionPropertyInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._collectionPropertyInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._collectionPropertyInUri(_collectionPropertyInUri_1.Parsed), _collectionPropertyInUri_1.Remainder);
                }
            }
        }
        
        public static class _navigationPropertyInUriParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._navigationPropertyInUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._navigationPropertyInUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._navigationPropertyInUri> Parse(IInput<char>? input)
                {
                    var _navigationPropertyInUri_1 = __GeneratedOdataV4.Parsers.Rules._navigationPropertyInUriParser.Instance.Parse(input);
if (!_navigationPropertyInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._navigationPropertyInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._annotationInUriⳆprimitivePropertyInUriⳆcomplexPropertyInUriⳆcollectionPropertyInUriⳆnavigationPropertyInUri._navigationPropertyInUri(_navigationPropertyInUri_1.Parsed), _navigationPropertyInUri_1.Remainder);
                }
            }
        }
    }
    
}
