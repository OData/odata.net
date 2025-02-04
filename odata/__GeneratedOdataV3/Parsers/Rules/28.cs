namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _actionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._actionImportCall> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._actionImportCall>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._actionImportCall> Parse(IInput<char>? input)
            {
                var _actionImport_1 = __GeneratedOdataV3.Parsers.Rules._actionImportParser.Instance.Parse(input);
if (!_actionImport_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._actionImportCall)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._actionImportCall(_actionImport_1.Parsed), _actionImport_1.Remainder);
            }
        }
    }
    
}
