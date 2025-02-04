namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _beginⲻarrayParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._beginⲻarray> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._beginⲻarray>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._beginⲻarray> Parse(IInput<char>? input)
            {
                var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(input);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._beginⲻarray)!, input);
}

var _Ⲥʺx5BʺⳆʺx25x35x42ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx5BʺⳆʺx25x35x42ʺↃParser.Instance.Parse(_BWS_1.Remainder);
if (!_Ⲥʺx5BʺⳆʺx25x35x42ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._beginⲻarray)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_Ⲥʺx5BʺⳆʺx25x35x42ʺↃ_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._beginⲻarray)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._beginⲻarray(_BWS_1.Parsed, _Ⲥʺx5BʺⳆʺx25x35x42ʺↃ_1.Parsed, _BWS_2.Parsed), _BWS_2.Remainder);
            }
        }
    }
    
}
