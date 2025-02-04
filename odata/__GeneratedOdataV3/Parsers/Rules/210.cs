namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _endⲻarrayParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._endⲻarray> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._endⲻarray>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._endⲻarray> Parse(IInput<char>? input)
            {
                var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(input);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._endⲻarray)!, input);
}

var _Ⲥʺx5DʺⳆʺx25x35x44ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx5DʺⳆʺx25x35x44ʺↃParser.Instance.Parse(_BWS_1.Remainder);
if (!_Ⲥʺx5DʺⳆʺx25x35x44ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._endⲻarray)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._endⲻarray(_BWS_1.Parsed, _Ⲥʺx5DʺⳆʺx25x35x44ʺↃ_1.Parsed), _Ⲥʺx5DʺⳆʺx25x35x44ʺↃ_1.Remainder);
            }
        }
    }
    
}
