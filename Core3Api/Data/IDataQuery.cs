public interface IDataQuery
{
    Task<IEnumerable<T>> DataListReturn<T>(string procName);
    Task<T> DataValueReturn<T>(string procName);
    void DataNotReturn(string procName);
}