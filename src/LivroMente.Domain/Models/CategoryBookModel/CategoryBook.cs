using LivroMente.Domain.Models.BookModel;

namespace LivroMente.Domain.Models.CategoryBookModel
{
    public class CategoryBook
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public List<Book> Books { get; set; }
        public bool IsActive { get; set; }
    }
}