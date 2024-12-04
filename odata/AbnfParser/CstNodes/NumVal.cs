namespace AbnfParser.CstNodes
{
    public abstract class NumVal
    {
        private NumVal()
        {
        }

        //// TODO do these

        public sealed class BinVal : NumVal
        {
        }

        public sealed class DecVal : NumVal
        {
        }

        public sealed class HexVal : NumVal
        {
        }
    }
}
