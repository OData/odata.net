namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _schemaversionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._schemaversion> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._schemaversion>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._schemaversion> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺⳆʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx24x73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺⳆʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺⳆʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._schemaversion)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺⳆʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._schemaversion)!, input);
}

var _ⲤSTARⳆ1ЖunreservedↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤSTARⳆ1ЖunreservedↃParser.Instance.Parse(_EQ_1.Remainder);
if (!_ⲤSTARⳆ1ЖunreservedↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._schemaversion)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._schemaversion(_Ⲥʺx24x73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺⳆʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺↃ_1.Parsed, _EQ_1.Parsed, _ⲤSTARⳆ1ЖunreservedↃ_1.Parsed), _ⲤSTARⳆ1ЖunreservedↃ_1.Remainder);
            }
        }
    }
    
}
