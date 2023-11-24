using BookStoreApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Controllers
{
	public class AuthorController : Controller
	{
		private BookstoreContext context { get; set; }

		public AuthorController(BookstoreContext ctx) => context = ctx;

		public IActionResult Index(int page = 1, int pageSize = 5, string searchTerm = "")
		{
			int skip = (page - 1) * pageSize;

			var authorsQuery = context.Authors.AsQueryable();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				// Apply search filter
				authorsQuery = authorsQuery
					.Where(a => EF.Functions.Like(a.FirstName, $"%{searchTerm}%") || EF.Functions.Like(a.LastName, $"%{searchTerm}%"));
			}

			authorsQuery = authorsQuery.OrderBy(m => m.AuthorId);

			var totalCount = authorsQuery.Count();
			var authors = authorsQuery.Skip(skip).Take(pageSize).ToList();

			var pagedList = new PagedList<Author>
			{
				Items = authors,
				PageNumber = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};

			var viewModel = new AuthorViewModel
			{
				PageResult = pagedList,
			};

			return View(viewModel);
		}


		[HttpGet]
		public IActionResult Edit(int id)
		{
			var author = context.Authors.Find(id);
			return View(author);
		}
		[HttpPost]
		public IActionResult Edit(Author author)
		{
			if (ModelState.IsValid)
			{
				if (author.AuthorId == 0)
					context.Authors.Add(author);
				else
					context.Authors.Update(author);
				context.SaveChanges();
				return RedirectToAction("Index", "Author");
			}
			else
			{
				return View(author);
			}
		}

		[HttpGet]
		public IActionResult Delete(int id)
		{
			var author = context.Authors.Find(id);
			return View(author);
		}

		[HttpPost]
		public IActionResult Delete(Author author)
		{

			if (author == null)
			{
				return NotFound();
			}

			var booksQuery = context.Books
				.Where(b => b.AuthorId == author.AuthorId)
				.ToList();

			context.Books.RemoveRange(booksQuery);

			context.Authors.Remove(author);

			context.SaveChanges();

			return RedirectToAction("Index", "Author");
		}

	}
}
