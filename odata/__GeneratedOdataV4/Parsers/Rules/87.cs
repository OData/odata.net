namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._select> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._select>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._select> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃ_1 = __GeneratedOdataV4.Parsers.Inners._Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._select)!, input);
}

var _EQ_1 = __GeneratedOdataV4.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._select)!, input);
}

var _selectItem_1 = __GeneratedOdataV4.Parsers.Rules._selectItemParser.Instance.Parse(_EQ_1.Remainder);
if (!_selectItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._select)!, input);
}

var _ⲤCOMMA_selectItemↃ_1 = Inners._ⲤCOMMA_selectItemↃParser.Instance.Many().Parse(_selectItem_1.Remainder);
if (!_ⲤCOMMA_selectItemↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._select)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._select(_Ⲥʺx24x73x65x6Cx65x63x74ʺⳆʺx73x65x6Cx65x63x74ʺↃ_1.Parsed, _EQ_1.Parsed, _selectItem_1.Parsed, _ⲤCOMMA_selectItemↃ_1.Parsed), _ⲤCOMMA_selectItemↃ_1.Remainder);
            }
        }
    }
    
}
