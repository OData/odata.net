namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _hierⲻpartParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart> Instance { get; } = (_ʺx2Fx2Fʺ_authority_pathⲻabemptyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart>(_pathⲻabsoluteParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart>(_pathⲻrootlessParser.Instance);
        
        public static class _ʺx2Fx2Fʺ_authority_pathⲻabemptyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._ʺx2Fx2Fʺ_authority_pathⲻabempty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._ʺx2Fx2Fʺ_authority_pathⲻabempty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._ʺx2Fx2Fʺ_authority_pathⲻabempty> Parse(IInput<char>? input)
                {
                    var _ʺx2Fx2Fʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx2Fx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hierⲻpart._ʺx2Fx2Fʺ_authority_pathⲻabempty)!, input);
}

var _authority_1 = __GeneratedOdataV3.Parsers.Rules._authorityParser.Instance.Parse(_ʺx2Fx2Fʺ_1.Remainder);
if (!_authority_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hierⲻpart._ʺx2Fx2Fʺ_authority_pathⲻabempty)!, input);
}

var _pathⲻabempty_1 = __GeneratedOdataV3.Parsers.Rules._pathⲻabemptyParser.Instance.Parse(_authority_1.Remainder);
if (!_pathⲻabempty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hierⲻpart._ʺx2Fx2Fʺ_authority_pathⲻabempty)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._ʺx2Fx2Fʺ_authority_pathⲻabempty(_ʺx2Fx2Fʺ_1.Parsed, _authority_1.Parsed, _pathⲻabempty_1.Parsed), _pathⲻabempty_1.Remainder);
                }
            }
        }
        
        public static class _pathⲻabsoluteParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻabsolute> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻabsolute>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻabsolute> Parse(IInput<char>? input)
                {
                    var _pathⲻabsolute_1 = __GeneratedOdataV3.Parsers.Rules._pathⲻabsoluteParser.Instance.Parse(input);
if (!_pathⲻabsolute_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻabsolute)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻabsolute(_pathⲻabsolute_1.Parsed), _pathⲻabsolute_1.Remainder);
                }
            }
        }
        
        public static class _pathⲻrootlessParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻrootless> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻrootless>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻrootless> Parse(IInput<char>? input)
                {
                    var _pathⲻrootless_1 = __GeneratedOdataV3.Parsers.Rules._pathⲻrootlessParser.Instance.Parse(input);
if (!_pathⲻrootless_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻrootless)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._hierⲻpart._pathⲻrootless(_pathⲻrootless_1.Parsed), _pathⲻrootless_1.Remainder);
                }
            }
        }
    }
    
}
