namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1ЖpcharParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar> Instance { get; } = (_ʺx61x74x6Fx6DʺParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar>(_ʺx6Ax73x6Fx6EʺParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar>(_ʺx78x6Dx6CʺParser.Instance).Or<__GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar>(_1Жpchar_ʺx2Fʺ_1ЖpcharParser.Instance);
        
        public static class _ʺx61x74x6Fx6DʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._ʺx61x74x6Fx6Dʺ> Instance { get; } = from _ʺx61x74x6Fx6Dʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx61x74x6Fx6DʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._ʺx61x74x6Fx6Dʺ(_ʺx61x74x6Fx6Dʺ_1);
        }
        
        public static class _ʺx6Ax73x6Fx6EʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._ʺx6Ax73x6Fx6Eʺ> Instance { get; } = from _ʺx6Ax73x6Fx6Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx6Ax73x6Fx6EʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._ʺx6Ax73x6Fx6Eʺ(_ʺx6Ax73x6Fx6Eʺ_1);
        }
        
        public static class _ʺx78x6Dx6CʺParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._ʺx78x6Dx6Cʺ> Instance { get; } = from _ʺx78x6Dx6Cʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx78x6Dx6CʺParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._ʺx78x6Dx6Cʺ(_ʺx78x6Dx6Cʺ_1);
        }
        
        public static class _1Жpchar_ʺx2Fʺ_1ЖpcharParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._1Жpchar_ʺx2Fʺ_1Жpchar> Instance { get; } = from _pchar_1 in __GeneratedOdata.Parsers.Rules._pcharParser.Instance.Many()
from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _pchar_2 in __GeneratedOdata.Parsers.Rules._pcharParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._1Жpchar_ʺx2Fʺ_1Жpchar(_pchar_1, _ʺx2Fʺ_1, _pchar_2);
        }
    }
    
}