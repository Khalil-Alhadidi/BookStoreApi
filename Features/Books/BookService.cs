using Microsoft.EntityFrameworkCore;
using BookStoreApi.Infrastructure.Data;

namespace BookStoreApi.Features.Books;

public class BookService : IBookService
{
    private readonly BookStoreDbContext _context;
    private readonly IBookRepository _bookRepository;

    public BookService(BookStoreDbContext context, IBookRepository bookRepository)
    {
        _context = context;
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetAllAsync();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _bookRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Book>> FindBooksByTitleAsync(string title)
    {
        return await _bookRepository.FindByTitleAsync(title);
    }

    public async Task<Book> CreateBookAsync(Book book)
    {
        return await _bookRepository.AddAsync(book);
    }

    public async Task<Book?> UpdateBookAsync(Book book)
    {
        return await _bookRepository.UpdateAsync(book);
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        return await _bookRepository.DeleteAsync(id);
    }
} 