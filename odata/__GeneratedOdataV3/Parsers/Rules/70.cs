namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _levelsParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._levels> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._levels>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._levels> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._levels)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._levels)!, input);
}

var _ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃParser.Instance.Parse(_EQ_1.Remainder);
if (!_ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._levels)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._levels(_Ⲥʺx24x6Cx65x76x65x6Cx73ʺⳆʺx6Cx65x76x65x6Cx73ʺↃ_1.Parsed, _EQ_1.Parsed,  _ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃ_1.Parsed), _ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃ_1.Remainder);
            }
        }
    }
    
}
