using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStoreApi.Features.Books;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(int id);
    Task<IEnumerable<Book>> FindBooksByTitleAsync(string title);
    Task<Book> CreateBookAsync(Book book);
    Task<Book?> UpdateBookAsync(Book book);
    Task<bool> DeleteBookAsync(int id);
} 