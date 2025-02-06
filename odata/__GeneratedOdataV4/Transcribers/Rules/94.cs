namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _qualifiedActionNameTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._qualifiedActionName>
    {
        private _qualifiedActionNameTranscriber()
        {
        }
        
        public static _qualifiedActionNameTranscriber Instance { get; } = new _qualifiedActionNameTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._qualifiedActionName value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._namespaceTranscriber.Instance.Transcribe(value._namespace_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ʺx2EʺTranscriber.Instance.Transcribe(value._ʺx2Eʺ_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._actionTranscriber.Instance.Transcribe(value._action_1, builder);

        }
    }
    
}
