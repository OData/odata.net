namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _COMMA_lineStringDataParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._COMMA_lineStringData> Instance { get; } = from _COMMA_1 in __GeneratedOdata.Parsers.Rules._COMMAParser.Instance
from _lineStringData_1 in __GeneratedOdata.Parsers.Rules._lineStringDataParser.Instance
select new __GeneratedOdata.CstNodes.Inners._COMMA_lineStringData(_COMMA_1, _lineStringData_1);
    }
    
}
