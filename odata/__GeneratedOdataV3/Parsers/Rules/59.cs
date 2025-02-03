namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _idParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._id> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._id>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._id> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x69x64ʺⳆʺx69x64ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx24x69x64ʺⳆʺx69x64ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x69x64ʺⳆʺx69x64ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._id)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x69x64ʺⳆʺx69x64ʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._id)!, input);
}

var _IRIⲻinⲻquery_1 = __GeneratedOdataV3.Parsers.Rules._IRIⲻinⲻqueryParser.Instance.Parse(_EQ_1.Remainder);
if (!_IRIⲻinⲻquery_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._id)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._id(_Ⲥʺx24x69x64ʺⳆʺx69x64ʺↃ_1.Parsed, _EQ_1.Parsed,  _IRIⲻinⲻquery_1.Parsed), _IRIⲻinⲻquery_1.Remainder);
            }
        }
    }
    
}
