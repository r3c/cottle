#if NETSTANDARD1_5
namespace System
{
  public delegate TOutput Converter<TInput, TOutput> (TInput input);
}
#endif
