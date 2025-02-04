namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityCastOptionsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._entityCastOptions> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._entityCastOptions>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._entityCastOptions> Parse(IInput<char>? input)
            {
                var _ⲤentityCastOption_ʺx26ʺↃ_1 = Inners._ⲤentityCastOption_ʺx26ʺↃParser.Instance.Many().Parse(input);
if (!_ⲤentityCastOption_ʺx26ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._entityCastOptions)!, input);
}

var _id_1 = __GeneratedOdataV3.Parsers.Rules._idParser.Instance.Parse(_ⲤentityCastOption_ʺx26ʺↃ_1.Remainder);
if (!_id_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._entityCastOptions)!, input);
}

var _Ⲥʺx26ʺ_entityCastOptionↃ_1 = Inners._Ⲥʺx26ʺ_entityCastOptionↃParser.Instance.Many().Parse(_id_1.Remainder);
if (!_Ⲥʺx26ʺ_entityCastOptionↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._entityCastOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._entityCastOptions(_ⲤentityCastOption_ʺx26ʺↃ_1.Parsed, _id_1.Parsed, _Ⲥʺx26ʺ_entityCastOptionↃ_1.Parsed), _Ⲥʺx26ʺ_entityCastOptionↃ_1.Remainder);
            }
        }
    }
    
}
