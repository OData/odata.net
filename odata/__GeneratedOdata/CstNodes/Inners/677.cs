namespace __GeneratedOdata.CstNodes.Inners
{
    public sealed class HelperRangedAtMost5<T> : System.Collections.Generic.IEnumerable<T>
    {
        public HelperRangedAtMost5(System.Collections.Generic.IEnumerable<T> source)
        {
            this.Source = source;
        }
        
        private System.Collections.Generic.IEnumerable<T> Source { get; }
        
        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return this.Source.GetEnumerator();
        }
        
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)this.Source).GetEnumerator();
        }
    }
    
}
