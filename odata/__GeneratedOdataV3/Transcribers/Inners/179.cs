namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _STARⳆ1ЖunreservedTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved>
    {
        private _STARⳆ1ЖunreservedTranscriber()
        {
        }
        
        public static _STARⳆ1ЖunreservedTranscriber Instance { get; } = new _STARⳆ1ЖunreservedTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._STAR node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._STARTranscriber.Instance.Transcribe(node._STAR_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._1Жunreserved node, System.Text.StringBuilder context)
            {
                foreach (var _unreserved_1 in node._unreserved_1)
{
__GeneratedOdataV3.Trancsribers.Rules._unreservedTranscriber.Instance.Transcribe(_unreserved_1, context);
}

return default;
            }
        }
    }
    
}
