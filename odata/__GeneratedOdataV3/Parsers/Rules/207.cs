namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _beginⲻobjectParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._beginⲻobject> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._beginⲻobject>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._beginⲻobject> Parse(IInput<char>? input)
            {
                var _BWS_1 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(input);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._beginⲻobject)!, input);
}

var _Ⲥʺx7BʺⳆʺx25x37x42ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx7BʺⳆʺx25x37x42ʺↃParser.Instance.Parse(_BWS_1.Remainder);
if (!_Ⲥʺx7BʺⳆʺx25x37x42ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._beginⲻobject)!, input);
}

var _BWS_2 = __GeneratedOdataV3.Parsers.Rules._BWSParser.Instance.Parse(_Ⲥʺx7BʺⳆʺx25x37x42ʺↃ_1.Remainder);
if (!_BWS_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._beginⲻobject)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._beginⲻobject(_BWS_1.Parsed, _Ⲥʺx7BʺⳆʺx25x37x42ʺↃ_1.Parsed, _BWS_2.Parsed), _BWS_2.Remainder);
            }
        }
    }
    
}
