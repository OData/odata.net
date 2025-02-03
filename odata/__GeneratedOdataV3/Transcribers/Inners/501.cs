namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute>
    {
        private _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteTranscriber()
        {
        }
        
        public static _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteTranscriber Instance { get; } = new _ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minuteTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._ʺx5Aʺ node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Inners._ʺx5AʺTranscriber.Instance.Transcribe(node._ʺx5Aʺ_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV3.CstNodes.Inners._ʺx5AʺⳆSIGN_hour_ʺx3Aʺ_minute._SIGN_hour_ʺx3Aʺ_minute node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV3.Trancsribers.Rules._SIGNTranscriber.Instance.Transcribe(node._SIGN_1, context);
__GeneratedOdataV3.Trancsribers.Rules._hourTranscriber.Instance.Transcribe(node._hour_1, context);
__GeneratedOdataV3.Trancsribers.Inners._ʺx3AʺTranscriber.Instance.Transcribe(node._ʺx3Aʺ_1, context);
__GeneratedOdataV3.Trancsribers.Rules._minuteTranscriber.Instance.Transcribe(node._minute_1, context);

return default;
            }
        }
    }
    
}
