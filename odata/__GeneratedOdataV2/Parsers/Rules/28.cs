namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _actionImportCallParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._actionImportCall> Instance { get; } = from _actionImport_1 in __GeneratedOdataV2.Parsers.Rules._actionImportParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._actionImportCall(_actionImport_1);
    }
    
}
