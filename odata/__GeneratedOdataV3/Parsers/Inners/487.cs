namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ> Parse(IInput<char>? input)
            {
                var _ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺParser.Instance.Parse(input);
if (!_ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._Ⲥʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺↃ(_ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ_1.Parsed), _ʺx41ʺⳆʺx51ʺⳆʺx67ʺⳆʺx77ʺ_1.Remainder);
            }
        }
    }
    
}
