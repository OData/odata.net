namespace V2.Fx.Runtime.CompilerServices
{
    using OldUnsafe = System.Runtime.CompilerServices.Unsafe;
    
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// The purpose of this to is to be equivalent to <see cref="System.Runtime.CompilerServices.Unsafe"/> while extending it's
    /// capabilities to be more accurate.
    /// </remarks>
    public static class Unsafe
    {
        public static unsafe void* AsPointer<T>(in T value) where T : allows ref struct
        {
            ref T reference = ref OldUnsafe.AsRef(in value);

            return OldUnsafe.AsPointer(ref reference);
        }
    }
}
