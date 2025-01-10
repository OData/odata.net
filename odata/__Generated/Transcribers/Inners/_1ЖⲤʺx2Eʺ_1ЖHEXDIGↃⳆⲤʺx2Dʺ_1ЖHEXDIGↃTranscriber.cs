namespace __Generated.Trancsribers.Inners
{
    public sealed class _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ>
    {
        private _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃTranscriber()
        {
        }
        
        public static _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃTranscriber Instance { get; } = new _1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃ node, System.Text.StringBuilder context)
            {
                foreach (var _Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1 in node._Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1)
{
__Generated.Trancsribers.Inners._Ⲥʺx2Eʺ_1ЖHEXDIGↃTranscriber.Instance.Transcribe(_Ⲥʺx2Eʺ_1ЖHEXDIGↃ_1, context);
}

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._1ЖⲤʺx2Eʺ_1ЖHEXDIGↃⳆⲤʺx2Dʺ_1ЖHEXDIGↃ._Ⲥʺx2Dʺ_1ЖHEXDIGↃ node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Inners._Ⲥʺx2Dʺ_1ЖHEXDIGↃTranscriber.Instance.Transcribe(node._Ⲥʺx2Dʺ_1ЖHEXDIGↃ_1, context);

return default;
            }
        }
    }
    
}
