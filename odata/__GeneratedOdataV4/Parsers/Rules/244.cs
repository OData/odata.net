namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _concreteSpatialTypeNameParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName> Instance { get; } = (_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6EʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName>(_ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName>(_ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName>(_ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName>(_ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6EʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName>(_ʺx50x6Fx69x6Ex74ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName>(_ʺx50x6Fx6Cx79x67x6Fx6EʺParser.Instance);
        
        public static class _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6EʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ> Parse(IInput<char>? input)
                {
                    var _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6EʺParser.Instance.Parse(input);
if (!_ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ.Instance, _ʺx43x6Fx6Cx6Cx65x63x74x69x6Fx6Eʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺParser.Instance.Parse(input);
if (!_ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ.Instance, _ʺx4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺParser.Instance.Parse(input);
if (!_ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ.Instance, _ʺx4Dx75x6Cx74x69x4Cx69x6Ex65x53x74x72x69x6Ex67ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74ʺParser.Instance.Parse(input);
if (!_ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74ʺ.Instance, _ʺx4Dx75x6Cx74x69x50x6Fx69x6Ex74ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6EʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Eʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Eʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Eʺ> Parse(IInput<char>? input)
                {
                    var _ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6EʺParser.Instance.Parse(input);
if (!_ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Eʺ.Instance, _ʺx4Dx75x6Cx74x69x50x6Fx6Cx79x67x6Fx6Eʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx50x6Fx69x6Ex74ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx50x6Fx69x6Ex74ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx50x6Fx69x6Ex74ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx50x6Fx69x6Ex74ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx50x6Fx69x6Ex74ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx50x6Fx69x6Ex74ʺParser.Instance.Parse(input);
if (!_ʺx50x6Fx69x6Ex74ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx50x6Fx69x6Ex74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx50x6Fx69x6Ex74ʺ.Instance, _ʺx50x6Fx69x6Ex74ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx50x6Fx6Cx79x67x6Fx6EʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx50x6Fx6Cx79x67x6Fx6Eʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx50x6Fx6Cx79x67x6Fx6Eʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx50x6Fx6Cx79x67x6Fx6Eʺ> Parse(IInput<char>? input)
                {
                    var _ʺx50x6Fx6Cx79x67x6Fx6Eʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx50x6Fx6Cx79x67x6Fx6EʺParser.Instance.Parse(input);
if (!_ʺx50x6Fx6Cx79x67x6Fx6Eʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx50x6Fx6Cx79x67x6Fx6Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._concreteSpatialTypeName._ʺx50x6Fx6Cx79x67x6Fx6Eʺ.Instance, _ʺx50x6Fx6Cx79x67x6Fx6Eʺ_1.Remainder);
                }
            }
        }
    }
    
}
