using ScoresDb.Models;

namespace ScoresDb.Repositories
{
    public interface IRepository<T>
    {
        public string TableName { get; }
        public List<T> Items { get; set; }
        public string ConnectionString { get; set; }
        Task<string> CreateTable();
        public Task<IEnumerable<T>> GetItems();
        public Task<T> GetById(Guid id);
        public Task<T> GetByName(string name);
        public Task<bool> ReadAll();
        public Task<T> Create(T item);
        public Task<T> Update(T item);
        public Task<bool> Delete(T item);

      

    }
}