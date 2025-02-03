namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1ЖpcharTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar>
    {
        private _ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1ЖpcharTranscriber()
        {
        }
        
        public static _ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1ЖpcharTranscriber Instance { get; } = new _ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1ЖpcharTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._ʺx61x74x6Fx6Dʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx61x74x6Fx6DʺTranscriber.Instance.Transcribe(node._ʺx61x74x6Fx6Dʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._ʺx6Ax73x6Fx6Eʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx6Ax73x6Fx6EʺTranscriber.Instance.Transcribe(node._ʺx6Ax73x6Fx6Eʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._ʺx78x6Dx6Cʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx78x6Dx6CʺTranscriber.Instance.Transcribe(node._ʺx78x6Dx6Cʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx61x74x6Fx6DʺⳆʺx6Ax73x6Fx6EʺⳆʺx78x6Dx6CʺⳆ1Жpchar_ʺx2Fʺ_1Жpchar._1Жpchar_ʺx2Fʺ_1Жpchar node, System.Text.StringBuilder context)
            {
                foreach (var _pchar_1 in node._pchar_1)
{
__GeneratedOdataV3.Trancsribers.Rules._pcharTranscriber.Instance.Transcribe(_pchar_1, context);
}
__GeneratedOdataV3.Trancsribers.Inners._ʺx2FʺTranscriber.Instance.Transcribe(node._ʺx2Fʺ_1, context);
foreach (var _pchar_2 in node._pchar_2)
{
__GeneratedOdataV3.Trancsribers.Rules._pcharTranscriber.Instance.Transcribe(_pchar_2, context);
}

return default;
            }
        }
    }
    
}
