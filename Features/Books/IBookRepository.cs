namespace BookStoreApi.Features.Books;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(int id);
    Task<IEnumerable<Book>> FindByTitleAsync(string title);
    Task<Book> AddAsync(Book book);
    Task<Book?> UpdateAsync(Book book);
    Task<bool> DeleteAsync(int id);
} 