namespace __Generated.Trancsribers.Inners
{
    public sealed class _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ>
    {
        private _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃTranscriber()
        {
        }
        
        public static _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃTranscriber Instance { get; } = new _1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._1ЖⲤʺx2Eʺ_1ЖDIGITↃ node, System.Text.StringBuilder context)
            {
                foreach (var _Ⲥʺx2Eʺ_1ЖDIGITↃ_1 in node._Ⲥʺx2Eʺ_1ЖDIGITↃ_1)
{
Inners._Ⲥʺx2Eʺ_1ЖDIGITↃTranscriber.Instance.Transcribe(_Ⲥʺx2Eʺ_1ЖDIGITↃ_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖDIGITↃⳆⲤʺx2Dʺ_1ЖDIGITↃ._Ⲥʺx2Dʺ_1ЖDIGITↃ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._Ⲥʺx2Dʺ_1ЖDIGITↃTranscriber.Instance.Transcribe(node._Ⲥʺx2Dʺ_1ЖDIGITↃ_1, context);

return default;
            }
        }
    }
    
}
