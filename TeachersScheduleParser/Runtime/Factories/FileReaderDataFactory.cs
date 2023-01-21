using System;
using System.Data;

namespace TeachersScheduleParser.Runtime.Factories
{
    public class FileReaderDataFactory<T, D> : IFileReaderDataFactory<T, D>
    {
        private readonly Func<D, T> _factory;
        
        public FileReaderDataFactory(Func<D, T> factory)
        {
            _factory = factory;
        }

        public T Create(D data)
        {
            var value = data;
            
            return _factory(value);
        }
    }
}