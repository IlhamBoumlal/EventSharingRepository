using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventSharing.Data;
using EventSharing.Models;
using AutoMapper;
using EventSharing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
namespace EventSharing.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public EventsController(ApplicationDbContext context, IMapper _mapper, UserManager<User> _userManager)
        {
            _context = context;
            this._mapper = _mapper;
            this._userManager = _userManager;
        }

        // GET: Events
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Index()
        {
            List<Event> events;
            var currentUser= await _userManager.GetUserAsync(User);//User :utilisateur courant
            bool isOrganizer = await _userManager.IsInRoleAsync(currentUser, "Organizer");
            if(isOrganizer)
            {
                events= await _context.Events
                    .Where(e => e.Creator.Email.Equals(User.FindFirstValue(ClaimTypes.Email)))
                    .Include(e => e.Category)
                    .ToListAsync();
            }
            else
            {
                events = await _context.Events.Include(e=>e.Category)
                        .ToListAsync();
            }

            return _context.Events != null ?
                View(_mapper.Map<List<EventViewModel>>(events) ):
                Problem("Entity set'ApplicationDbContext.Events' is null");
        }

        // GET: Events/Details/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //@event est un mot clé en C#, dans on utilise @ pour pouvoir l'utiliser comme nom de variable
            var eventVm =_mapper.Map<EventViewModel>(await _context.Events
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id));
            if (eventVm == null)
            {
                return NotFound();
            }

            return View(eventVm);
        }

        // GET: Events/Create
        [Authorize(Roles = "Admin,Organizer")]
        public IActionResult Create()
        {
            var eventVm = new EventViewModel
            {
                CategoriesVm = _mapper.Map<List<CategoryViewModel>>(_context.Categories.ToList())
            };
            return View(eventVm);
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartDate,EndDate, CategoryId")] EventViewModel eventVm)
        {
            if (ModelState.IsValid)
            {
                var @event = _mapper.Map<Event>(eventVm);
                @event.Category = _context.Categories
                    .FirstOrDefault(c=>c.Id.Equals(eventVm.CategoryId));
                @event.Creator = _context.Users.FirstOrDefault(o => o.Email.Equals(User.FindFirstValue(ClaimTypes.Email)));
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            eventVm.CategoriesVm = _mapper.Map<List<CategoryViewModel>>(_context.Categories.ToList());
            return View(_mapper.Map<EventViewModel>(eventVm));
        }

        // GET: Events/Edit/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventVm = _mapper.Map<EventViewModel>(await _context.Events
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id));
            if (eventVm == null)
            {
                return NotFound();
            }
            eventVm.CategoriesVm = _mapper.Map<List<CategoryViewModel>>(_context.Categories.ToList());
            return View(eventVm);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartDate,EndDate, CategoryId")] EventViewModel eventVm)
        {
            if (id != eventVm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    var @event = _mapper.Map<Event>(eventVm);
                    @event.Category = null;
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventVm.Id))
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
            return View(eventVm);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<EventViewModel>(@event));
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
