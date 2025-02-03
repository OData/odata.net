namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _actionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._actionImportCall> Instance { get; } = from _actionImport_1 in __GeneratedOdataV3.Parsers.Rules._actionImportParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._actionImportCall(_actionImport_1);
    }
    
}
