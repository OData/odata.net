namespace __Generated.Trancsribers.Inners
{
    public sealed class _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ>
    {
        private _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃTranscriber()
        {
        }
        
        public static _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃTranscriber Instance { get; } = new _1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._1ЖⲤʺx2Eʺ_1ЖBITↃ node, System.Text.StringBuilder context)
            {
                foreach (var _Ⲥʺx2Eʺ_1ЖBITↃ_1 in node._Ⲥʺx2Eʺ_1ЖBITↃ_1)
{
Inners._Ⲥʺx2Eʺ_1ЖBITↃTranscriber.Instance.Transcribe(_Ⲥʺx2Eʺ_1ЖBITↃ_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖBITↃⳆⲤʺx2Dʺ_1ЖBITↃ._Ⲥʺx2Dʺ_1ЖBITↃ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._Ⲥʺx2Dʺ_1ЖBITↃTranscriber.Instance.Transcribe(node._Ⲥʺx2Dʺ_1ЖBITↃ_1, context);

return default;
            }
        }
    }
    
}
