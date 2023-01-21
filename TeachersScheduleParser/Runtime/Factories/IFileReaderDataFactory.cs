namespace ExelParser.Runtime.Factories
{
    public interface IFileReaderDataFactory<out T, in D>
    {
        T Create(D data);
    }
}