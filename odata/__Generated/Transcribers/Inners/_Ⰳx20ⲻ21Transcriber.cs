namespace __Generated.Trancsribers.Inners
{
    public sealed class _Ⰳx20ⲻ21Transcriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Inners._Ⰳx20ⲻ21>
    {
        private _Ⰳx20ⲻ21Transcriber()
        {
        }
        
        public static _Ⰳx20ⲻ21Transcriber Instance { get; } = new _Ⰳx20ⲻ21Transcriber();
        
        public void Transcribe(__Generated.CstNodes.Inners._Ⰳx20ⲻ21 value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Inners._Ⰳx20ⲻ21.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._Ⰳx20ⲻ21._20 node, System.Text.StringBuilder context)
            {
                context.Append((char)0x20);
return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Inners._Ⰳx20ⲻ21._21 node, System.Text.StringBuilder context)
            {
                context.Append((char)0x21);
return default;
            }
        }
    }
    
}
