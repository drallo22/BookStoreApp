using BookStoreApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace BookStoreApp.Controllers
{
    public class BookController : Controller
    {

        private BookstoreContext context { get; set; }
        public BookController(BookstoreContext ctx) =>  context = ctx;
        public IActionResult Index(int page = 1, int pageSize = 5, int selectedGenreId = 0)
        {
			int skip = (page - 1) * pageSize;
			var genres = context.Genres.ToList();

			// Search the books based on the selected genre.
			var booksQuery = context.Books.Include(m => m.Genre).Include(m => m.authorObject).OrderBy(m => m.Title);
			if (selectedGenreId != 0)
			{
				booksQuery = (IOrderedQueryable<Book>)booksQuery.Where(b => b.GenreId == selectedGenreId);
			}
			var totalCount = booksQuery.Count();
			var books = booksQuery.Skip(skip).Take(pageSize).ToList();

			var pagedList = new PagedList<Book>
			{
				Items = books,
				PageNumber = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};

			var viewModel = new BookViewModel
			{
				PageResult = pagedList,
				Genres = new SelectList(genres, "GenreId", "Name"),
				SelectedGenreId = selectedGenreId
			};

			return View(viewModel);
        }
        [HttpPost]
        public IActionResult Add(BookViewModel model)
        {
            TempData["message"] = $"{model.Book.Title} has been added in the cart";
            return RedirectToAction("Index", "Book");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {

            //ViewBag.Action = "Edit";
            //ViewBag.Genres = context.Genres.OrderBy(g => g.Name).ToList();
            //var movie = context.Movies.Find(id);
            //return View(movie);

            ViewBag.Action = "Edit";
            ViewBag.Authors = context.Authors.OrderBy(a => a.AuthorId).ToList();
            ViewBag.Genres = context.Genres.OrderBy(g => g.GenreId).ToList();
            var book = context.Books.Find(id);
            //  var books = context.Books.OrderBy(b => b.BookId).ToList();


             /*Book book = context.Books.Where(b => b.ISBN == id)
                         .OrderBy(b => b.BookId)
                         .Include(a => a.authorObject)
                         .FirstOrDefault();*/

            return View(book);

        }

         [HttpPost]
         public IActionResult Edit(Book book)
         {
           // if (ModelState.IsValid)
           // {
             //   if (book.ISBN == "")
              //     context.Books.Add(book);
                //else
                    context.Books.Update(book);
                context.SaveChanges();
                return RedirectToAction("Index", "Book");
            //}
            //else
            //{
            //    ViewBag.Action =
            //        (book.ISBN == "") ? "Add" : "Edit";
            //    //ViewBag.Authors = context.Authors.OrderBy(a => a.AuthorId).ToList();
            //    ViewBag.Genres = context.Genres.OrderBy(g => g.GenreId).ToList();
            //    return View(book);

            //}
          /*  //  var authorToUpdate = context.Authors.FirstAsync(a => a.AuthorId == book.AuthorId);

            ViewBag.Authors = context.Authors.OrderBy(a => a.AuthorId).ToList();
            ViewBag.Genres = context.Genres.OrderBy(g => g.GenreId).ToList();
            var books = context.Books.OrderBy(b => b.BookId).ToList();

            var givenBook = context.Books.FirstOrDefault(b => b.ISBN == book.ISBN);
            if(givenBook != null)
            {
                givenBook.ISBN = book.ISBN;
                givenBook.Title = book.Title;
                givenBook.Price = book.Price;
                givenBook.AuthorId = book.AuthorId;
                givenBook.GenreId = book.GenreId;

                //context.SaveChanges();
                context.Entry(book).CurrentValues.SetValues(givenBook);

                return RedirectToAction("Index");

            }
            // context.SaveChanges();
           // context.Entry(givenBook).CurrentValues.SetValues(book);
            //context.Books.Update(book);
                context.SaveChanges();
                
                return RedirectToAction("Index", "Books");*/
         }
        }
       }
