namespace LanguageServer2
{
    public interface IRequestContext<T>
    {
        IClient Connection { get; }

        object Params { get; }
    }
}
