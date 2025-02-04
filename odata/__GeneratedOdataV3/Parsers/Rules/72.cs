namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _orderbyParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._orderby> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._orderby>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._orderby> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._orderby)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._orderby)!, input);
}

var _orderbyItem_1 = __GeneratedOdataV3.Parsers.Rules._orderbyItemParser.Instance.Parse(_EQ_1.Remainder);
if (!_orderbyItem_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._orderby)!, input);
}

var _ⲤCOMMA_orderbyItemↃ_1 = Inners._ⲤCOMMA_orderbyItemↃParser.Instance.Many().Parse(_orderbyItem_1.Remainder);
if (!_ⲤCOMMA_orderbyItemↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._orderby)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._orderby(_Ⲥʺx24x6Fx72x64x65x72x62x79ʺⳆʺx6Fx72x64x65x72x62x79ʺↃ_1.Parsed, _EQ_1.Parsed, _orderbyItem_1.Parsed, _ⲤCOMMA_orderbyItemↃ_1.Parsed), _ⲤCOMMA_orderbyItemↃ_1.Remainder);
            }
        }
    }
    
}
