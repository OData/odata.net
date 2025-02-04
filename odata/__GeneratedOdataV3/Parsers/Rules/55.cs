namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _entityOptionsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._entityOptions> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._entityOptions>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._entityOptions> Parse(IInput<char>? input)
            {
                var _ⲤentityIdOption_ʺx26ʺↃ_1 = Inners._ⲤentityIdOption_ʺx26ʺↃParser.Instance.Many().Parse(input);
if (!_ⲤentityIdOption_ʺx26ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._entityOptions)!, input);
}

var _id_1 = __GeneratedOdataV3.Parsers.Rules._idParser.Instance.Parse(_ⲤentityIdOption_ʺx26ʺↃ_1.Remainder);
if (!_id_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._entityOptions)!, input);
}

var _Ⲥʺx26ʺ_entityIdOptionↃ_1 = Inners._Ⲥʺx26ʺ_entityIdOptionↃParser.Instance.Many().Parse(_id_1.Remainder);
if (!_Ⲥʺx26ʺ_entityIdOptionↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._entityOptions)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._entityOptions(_ⲤentityIdOption_ʺx26ʺↃ_1.Parsed, _id_1.Parsed, _Ⲥʺx26ʺ_entityIdOptionↃ_1.Parsed), _Ⲥʺx26ʺ_entityIdOptionↃ_1.Remainder);
            }
        }
    }
    
}
