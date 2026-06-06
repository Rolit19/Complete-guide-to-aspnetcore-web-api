namespace my_books.Data.Model
{
    public class Author
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        //Navigation property for the related books

        public List<Book_Author> Book_Authors { get; set; }
    }
}
