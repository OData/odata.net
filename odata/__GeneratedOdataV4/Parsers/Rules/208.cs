namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _endⲻobjectParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._endⲻobject> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._endⲻobject>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._endⲻobject> Parse(IInput<char>? input)
            {
                var _BWS_1 = __GeneratedOdataV4.Parsers.Rules._BWSParser.Instance.Parse(input);
if (!_BWS_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._endⲻobject)!, input);
}

var _Ⲥʺx7DʺⳆʺx25x37x44ʺↃ_1 = __GeneratedOdataV4.Parsers.Inners._Ⲥʺx7DʺⳆʺx25x37x44ʺↃParser.Instance.Parse(_BWS_1.Remainder);
if (!_Ⲥʺx7DʺⳆʺx25x37x44ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._endⲻobject)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._endⲻobject(_BWS_1.Parsed, _Ⲥʺx7DʺⳆʺx25x37x44ʺↃ_1.Parsed), _Ⲥʺx7DʺⳆʺx25x37x44ʺↃ_1.Remainder);
            }
        }
    }
    
}
