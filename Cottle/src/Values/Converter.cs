#if CORECLR
namespace System
{
  public delegate TOutput Converter<TInput, TOutput> (TInput input);
}
#endif
