namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _selectPathParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._selectPath> Instance { get; } = from _ⲤcomplexPropertyⳆcomplexColPropertyↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃParser.Instance
from _ʺx2Fʺ_qualifiedComplexTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._selectPath(_ⲤcomplexPropertyⳆcomplexColPropertyↃ_1, _ʺx2Fʺ_qualifiedComplexTypeName_1.GetOrElse(null));
    }
    
}
