namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qualifiedTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName> Instance { get; } = (_singleQualifiedTypeNameParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName>(_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSEParser.Instance);
        
        public static class _singleQualifiedTypeNameParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._singleQualifiedTypeName> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._singleQualifiedTypeName>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._singleQualifiedTypeName> Parse(IInput<char>? input)
                {
                    var _singleQualifiedTypeName_1 = __GeneratedOdataV4.Parsers.Rules._singleQualifiedTypeNameParser.Instance.Parse(input);
if (!_singleQualifiedTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._singleQualifiedTypeName)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._singleQualifiedTypeName(_singleQualifiedTypeName_1.Parsed), _singleQualifiedTypeName_1.Remainder);
                }
            }
        }
        
        public static class _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSEParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE> Parse(IInput<char>? input)
                {
                    var _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6EʺParser.Instance.Parse(input);
if (!_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE)!, input);
}

var _OPEN_1 = __GeneratedOdataV4.Parsers.Rules._OPENParser.Instance.Parse(_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1.Remainder);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE)!, input);
}

var _singleQualifiedTypeName_1 = __GeneratedOdataV4.Parsers.Rules._singleQualifiedTypeNameParser.Instance.Parse(_OPEN_1.Remainder);
if (!_singleQualifiedTypeName_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE)!, input);
}

var _CLOSE_1 = __GeneratedOdataV4.Parsers.Rules._CLOSEParser.Instance.Parse(_singleQualifiedTypeName_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._qualifiedTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_OPEN_singleQualifiedTypeName_CLOSE(_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1.Parsed, _OPEN_1.Parsed, _singleQualifiedTypeName_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
                }
            }
        }
    }
    
}
