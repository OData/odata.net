namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_polygonDataParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_polygonData> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _polygonData_1 in __GeneratedOdata.Parsers.Rules._polygonDataParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_polygonData(_COMMA_1, _polygonData_1);
    }
    
}
