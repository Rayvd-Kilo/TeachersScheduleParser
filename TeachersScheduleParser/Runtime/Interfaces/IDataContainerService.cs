namespace TeachersScheduleParser.Runtime.Interfaces;

public interface IDataContainerService<T> : IDataSaver<T>, IDataGetter<T> { }