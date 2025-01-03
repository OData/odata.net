namespace GeneratorV3
{
    using System.Text;

    public interface ITranscriber<T>
    {
        void Transcribe(T value, StringBuilder builder);
    }
}
