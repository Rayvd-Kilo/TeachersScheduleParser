namespace TeachersScheduleParser.Runtime.Interfaces
{
    public interface IFileReaderDataFactory<out T, in D>
    {
        T Create(D data);
    }
}