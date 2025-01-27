namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _actionImportCallParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._actionImportCall> Instance { get; } = from _actionImport_1 in __GeneratedOdata.Parsers.Rules._actionImportParser.Instance
select new __GeneratedOdata.CstNodes.Rules._actionImportCall(_actionImport_1);
    }
    
}
