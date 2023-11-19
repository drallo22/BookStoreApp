namespace BookStoreApp.Models
{
	public class AuthorViewModel
	{
		public Author Author { get; set; }
		public PagedList<Author> PageResult { get; set; }
	}
}
