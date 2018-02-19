using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    public class TodoItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public TodoItemsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: TodoItems
        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var userId = _userManager.GetUserId(User);
                ApplicationUser user = _context.Users.First(u => u.Id.Equals(userId));
                return View(await _context.TodoItems.Where(i => i.User.Equals(user)).ToListAsync());
            }
            return RedirectToAction("Login", "Account");
        }

        // GET: TodoItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!_signInManager.IsSignedIn(User))
                return RedirectToAction("Login", "Account");
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems
                .Include(t => t.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // GET: TodoItems/Create
        public IActionResult Create()
        {
            if(_signInManager.IsSignedIn(User))
                return View();
            return RedirectToAction("Login", "Account");
        }

        // POST: TodoItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Deadline")] TodoItem todoItem)
        {
            if (ModelState.IsValid && _signInManager.IsSignedIn(User))
            {
                todoItem.User = _context.Users.First(u => u.Id.Equals(_userManager.GetUserId(User)));
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todoItem);
        }

        // GET: TodoItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!_signInManager.IsSignedIn(User))
                return RedirectToAction("Login", "Account");

            if (id == null)
            {
                return NotFound();
            }


            var todoItem = await _context.TodoItems.SingleOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }
            return View(todoItem);
        }

        // POST: TodoItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Deadline,IsDone")] TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", todoItem.User);
            return View(todoItem);
        }

        // GET: TodoItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!_signInManager.IsSignedIn(User))
                return RedirectToAction("Login", "Account");

            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems
                .Include(t => t.User)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // POST: TodoItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoItem = await _context.TodoItems.SingleOrDefaultAsync(m => m.Id == id);
            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
