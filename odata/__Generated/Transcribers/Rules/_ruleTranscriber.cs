namespace __Generated.Trancsribers.Rules
{
    public sealed class _ruleTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._rule>
    {
        private _ruleTranscriber()
        {
        }
        
        public static _ruleTranscriber Instance { get; } = new _ruleTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._rule value, System.Text.StringBuilder builder)
        {
            __Generated.Trancsribers.Rules._rulenameTranscriber.Instance.Transcribe(value._rulename_1, builder);
__Generated.Trancsribers.Rules._definedⲻasTranscriber.Instance.Transcribe(value._definedⲻas_1, builder);
__Generated.Trancsribers.Rules._elementsTranscriber.Instance.Transcribe(value._elements_1, builder);
__Generated.Trancsribers.Rules._cⲻnlTranscriber.Instance.Transcribe(value._cⲻnl_1, builder);

        }
    }
    
}
