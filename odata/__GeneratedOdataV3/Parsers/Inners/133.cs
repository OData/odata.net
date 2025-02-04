namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃ> Parse(IInput<char>? input)
            {
                var _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ_1 = __GeneratedOdataV3.Parsers.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺParser.Instance.Parse(input);
if (!_oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤoneToNine_ЖDIGITⳆʺx6Dx61x78ʺↃ(_oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ_1.Parsed), _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ_1.Remainder);
            }
        }
    }
    
}
