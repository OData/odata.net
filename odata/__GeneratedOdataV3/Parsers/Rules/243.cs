namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _abstractSpatialTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName> Instance { get; } = (_ʺx47x65x6Fx67x72x61x70x68x79ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName>(_ʺx47x65x6Fx6Dx65x74x72x79ʺParser.Instance);
        
        public static class _ʺx47x65x6Fx67x72x61x70x68x79ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx67x72x61x70x68x79ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx67x72x61x70x68x79ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx67x72x61x70x68x79ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx47x65x6Fx67x72x61x70x68x79ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx47x65x6Fx67x72x61x70x68x79ʺParser.Instance.Parse(input);
if (!_ʺx47x65x6Fx67x72x61x70x68x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx67x72x61x70x68x79ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx67x72x61x70x68x79ʺ(_ʺx47x65x6Fx67x72x61x70x68x79ʺ_1.Parsed), _ʺx47x65x6Fx67x72x61x70x68x79ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx47x65x6Fx6Dx65x74x72x79ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx6Dx65x74x72x79ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx6Dx65x74x72x79ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx6Dx65x74x72x79ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx47x65x6Fx6Dx65x74x72x79ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx47x65x6Fx6Dx65x74x72x79ʺParser.Instance.Parse(input);
if (!_ʺx47x65x6Fx6Dx65x74x72x79ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx6Dx65x74x72x79ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._abstractSpatialTypeName._ʺx47x65x6Fx6Dx65x74x72x79ʺ(_ʺx47x65x6Fx6Dx65x74x72x79ʺ_1.Parsed), _ʺx47x65x6Fx6Dx65x74x72x79ʺ_1.Remainder);
                }
            }
        }
    }
    
}
