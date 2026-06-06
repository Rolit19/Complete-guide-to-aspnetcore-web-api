namespace my_books.Data.Model
{
    public class Publisher
    {
        public int Id { get; set; }

        public string Name { get; set; }

        //Navigation property
        public List<Book> Books { get; set; }

    }
}
