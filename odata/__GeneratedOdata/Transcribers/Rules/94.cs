namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _qualifiedActionNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._qualifiedActionName>
    {
        private _qualifiedActionNameTranscriber()
        {
        }
        
        public static _qualifiedActionNameTranscriber Instance { get; } = new _qualifiedActionNameTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._qualifiedActionName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdata.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdata.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdata.Trancsribers.Rules._actionTranscriber.Instance.Transcribe(value._action_1, builder);

        }
    }
    
}
