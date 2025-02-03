namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _formatParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._format> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._format>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._format> Parse(IInput<char>? input)
            {
                var _Ⲥʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺↃParser.Instance.Parse(input);
if (!_Ⲥʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._format)!, input);
}

var _EQ_1 = __GeneratedOdataV3.Parsers.Rules._EQParser.Instance.Parse(_Ⲥʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺↃ_1.Remainder);
if (!_EQ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._format)!, input);
}

var _Ⲥʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1ЖpcharↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1ЖpcharↃParser.Instance.Parse(_EQ_1.Remainder);
if (!_Ⲥʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1ЖpcharↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._format)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._format(_Ⲥʺx24x66x6Fx72x6Dx61x74ʺⳆʺx66x6Fx72x6Dx61x74ʺↃ_1.Parsed, _EQ_1.Parsed,  _Ⲥʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1ЖpcharↃ_1.Parsed), _Ⲥʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1ЖpcharↃ_1.Remainder);
            }
        }
    }
    
}
