namespace __GeneratedOdataV3.Trancsribers.Inners
{
    public sealed class _navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Transcriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡>
    {
        private _navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Transcriber()
        {
        }
        
        public static _navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Transcriber Instance { get; } = new _navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡Transcriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Inners._navigation_ЖⲤcontainmentNavigationↃ_꘡ʺx2Fʺ_qualifiedEntityTypeName꘡ value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._navigationTranscriber.Instance.Transcribe(value._navigation_1, builder);
foreach (var _ⲤcontainmentNavigationↃ_1 in value._ⲤcontainmentNavigationↃ_1)
{
Inners._ⲤcontainmentNavigationↃTranscriber.Instance.Transcribe(_ⲤcontainmentNavigationↃ_1, builder);
}
if (value._ʺx2Fʺ_qualifiedEntityTypeName_1 != null)
{
__GeneratedOdataV3.Trancsribers.Inners._ʺx2Fʺ_qualifiedEntityTypeNameTranscriber.Instance.Transcribe(value._ʺx2Fʺ_qualifiedEntityTypeName_1, builder);
}

        }
    }
    
}
