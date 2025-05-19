using Microsoft.EntityFrameworkCore;
using BookStoreApi.Features.Books;
using BookStoreApi.Infrastructure.Data;

namespace BookStoreApi.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly BookStoreDbContext _context;

    public BookRepository(BookStoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books.ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books.FindAsync(id);
    }

    public async Task<IEnumerable<Book>> FindByTitleAsync(string title)
    {
        return await _context.Books
            .Where(b => b.Title.Contains(title))
            .ToListAsync();
    }

    public async Task<Book> AddAsync(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<Book?> UpdateAsync(Book book)
    {
        var existingBook = await _context.Books.FindAsync(book.Id);
        if (existingBook == null)
            return null;

        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.Price = book.Price;
        existingBook.ISBN = book.ISBN;

        await _context.SaveChangesAsync();
        return existingBook;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
} 