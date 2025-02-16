namespace __GeneratedOdataV3.Parsers.Rules
{
    using __GeneratedOdataV3.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _odataRelativeUriParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri> Instance { get; } = (_ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri>(_ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptionsParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri>(_ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptionsParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri>(_ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri>(_resourcePath_꘡ʺx3Fʺ_queryOptions꘡Parser.Instance);
        
        public static class _ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡>, IParser2<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡> Parse(IInput<char>? input)
                {
                    var _ʺx24x62x61x74x63x68ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x62x61x74x63x68ʺParser.Instance.Parse(input);
if (!_ʺx24x62x61x74x63x68ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡)!, input);
}

var _ʺx3Fʺ_batchOptions_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Fʺ_batchOptionsParser.Instance.Optional().Parse(_ʺx24x62x61x74x63x68ʺ_1.Remainder);
if (!_ʺx3Fʺ_batchOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡(_ʺx24x62x61x74x63x68ʺ_1.Parsed, _ʺx3Fʺ_batchOptions_1.Parsed.GetOrElse(null)), _ʺx3Fʺ_batchOptions_1.Remainder);
                }

                public Output2<char, _odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡> Parse2(IInput<char>? input)
                {
                    IParser2<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x62x61x74x63x68ʺ> _ʺx24x62x61x74x63x68ʺParser = default!;
                    IParser2<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_batchOptions> _ʺx3Fʺ_batchOptionsParser = default!;

                    var futureInput = new Future<Nothing, IInput<char>?>(input);
                    var _ʺx24x62x61x74x63x68ʺ_1 = futureInput.ContinueWith(output => _ʺx24x62x61x74x63x68ʺParser.Parse2(input));
                    var _ʺx3Fʺ_batchOptions_1 = _ʺx24x62x61x74x63x68ʺ_1.ContinueWith(
                        output => 
                            output.Success ?
                            _ʺx3Fʺ_batchOptionsParser.Parse2(output.Remainder) : 
                            new Output2<char, __GeneratedOdataV3.CstNodes.Inners._ʺx3Fʺ_batchOptions>(false, default, input)); //// TODO is there a way to keep passing the original input through?

                    ////return new Output2<char, _odataRelativeUri._ʺx24x62x61x74x63x68ʺ_꘡ʺx3Fʺ_batchOptions꘡>()

                    throw new System.NotImplementedException();
                }
            }
        }
        
        public static class _ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptionsParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions> Parse(IInput<char>? input)
                {
                    var _ʺx24x65x6Ex74x69x74x79ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x65x6Ex74x69x74x79ʺParser.Instance.Parse(input);
if (!_ʺx24x65x6Ex74x69x74x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions)!, input);
}

var _ʺx3Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3FʺParser.Instance.Parse(_ʺx24x65x6Ex74x69x74x79ʺ_1.Remainder);
if (!_ʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions)!, input);
}

var _entityOptions_1 = __GeneratedOdataV3.Parsers.Rules._entityOptionsParser.Instance.Parse(_ʺx3Fʺ_1.Remainder);
if (!_entityOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx3Fʺ_entityOptions(_ʺx24x65x6Ex74x69x74x79ʺ_1.Parsed, _ʺx3Fʺ_1.Parsed, _entityOptions_1.Parsed), _entityOptions_1.Remainder);
                }
            }
        }
        
        public static class _ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptionsParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions> Parse(IInput<char>? input)
                {
                    var _ʺx24x65x6Ex74x69x74x79ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x65x6Ex74x69x74x79ʺParser.Instance.Parse(input);
if (!_ʺx24x65x6Ex74x69x74x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions)!, input);
}

var _ʺx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2FʺParser.Instance.Parse(_ʺx24x65x6Ex74x69x74x79ʺ_1.Remainder);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions)!, input);
}

var _qualifiedEntityTypeName_1 = __GeneratedOdataV3.Parsers.Rules._qualifiedEntityTypeNameParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_qualifiedEntityTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions)!, input);
}

var _ʺx3Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3FʺParser.Instance.Parse(_qualifiedEntityTypeName_1.Remainder);
if (!_ʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions)!, input);
}

var _entityCastOptions_1 = __GeneratedOdataV3.Parsers.Rules._entityCastOptionsParser.Instance.Parse(_ʺx3Fʺ_1.Remainder);
if (!_entityCastOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x65x6Ex74x69x74x79ʺ_ʺx2Fʺ_qualifiedEntityTypeName_ʺx3Fʺ_entityCastOptions(_ʺx24x65x6Ex74x69x74x79ʺ_1.Parsed, _ʺx2Fʺ_1.Parsed, _qualifiedEntityTypeName_1.Parsed, _ʺx3Fʺ_1.Parsed, _entityCastOptions_1.Parsed), _entityCastOptions_1.Remainder);
                }
            }
        }
        
        public static class _ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡> Parse(IInput<char>? input)
                {
                    var _ʺx24x6Dx65x74x61x64x61x74x61ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx24x6Dx65x74x61x64x61x74x61ʺParser.Instance.Parse(input);
if (!_ʺx24x6Dx65x74x61x64x61x74x61ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡)!, input);
}

var _ʺx3Fʺ_metadataOptions_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Fʺ_metadataOptionsParser.Instance.Optional().Parse(_ʺx24x6Dx65x74x61x64x61x74x61ʺ_1.Remainder);
if (!_ʺx3Fʺ_metadataOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡)!, input);
}

var _context_1 = __GeneratedOdataV3.Parsers.Rules._contextParser.Instance.Optional().Parse(_ʺx3Fʺ_metadataOptions_1.Remainder);
if (!_context_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._ʺx24x6Dx65x74x61x64x61x74x61ʺ_꘡ʺx3Fʺ_metadataOptions꘡_꘡context꘡(_ʺx24x6Dx65x74x61x64x61x74x61ʺ_1.Parsed, _ʺx3Fʺ_metadataOptions_1.Parsed.GetOrElse(null), _context_1.Parsed.GetOrElse(null)), _context_1.Remainder);
                }
            }
        }
        
        public static class _resourcePath_꘡ʺx3Fʺ_queryOptions꘡Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡> Parse(IInput<char>? input)
                {
                    var _resourcePath_1 = __GeneratedOdataV3.Parsers.Rules._resourcePathParser.Instance.Parse(input);
if (!_resourcePath_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡)!, input);
}

var _ʺx3Fʺ_queryOptions_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3Fʺ_queryOptionsParser.Instance.Optional().Parse(_resourcePath_1.Remainder);
if (!_ʺx3Fʺ_queryOptions_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._odataRelativeUri._resourcePath_꘡ʺx3Fʺ_queryOptions꘡(_resourcePath_1.Parsed, _ʺx3Fʺ_queryOptions_1.Parsed.GetOrElse(null)), _ʺx3Fʺ_queryOptions_1.Remainder);
                }
            }
        }
    }
    
}
