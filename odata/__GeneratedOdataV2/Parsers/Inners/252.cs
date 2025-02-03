namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ> Instance { get; } = from _qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty_1 in __GeneratedOdataV2.Parsers.Inners._qualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ⲤqualifiedActionNameⳆqualifiedFunctionNameⳆselectListPropertyↃ(_qualifiedActionNameⳆqualifiedFunctionNameⳆselectListProperty_1);
    }
    
}
