using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventSharing.Data;
using EventSharing.ViewModels;
using EventSharing.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace EventSharing.Controllers
{
    //On travaille avec UserViewModel car User hérite de IdentityUser et on ne peut pas faire de CRUD sur IdentityUser,
    //on le considère pas comme entité classique
    //alors que UserViewModel est une classe simple qui contient
    //les propriétés nécessaires pour afficher les utilisateurs dans la vue.
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;


        public UsersController(ApplicationDbContext context, IMapper _mapper, UserManager<User> _userManager)
        {
            _context = context;
            this._mapper = _mapper;
            this._userManager = _userManager;
        }

        // GET: UserViewModels
        public async Task<IActionResult> Index()
        {
            return _context.Set<User>() != null ?
                View(_mapper.Map<List<UserViewModel>> (await _context.Set<User>().ToListAsync())) :
                Problem("Entity set 'ApplicaionDbContext.UserViewModel'is null");
        }

        // GET: UserViewModels/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userViewModel =_mapper.Map<UserViewModel>( await _context.Set<User>()
                .FirstOrDefaultAsync(m => m.Id == id));
            if (userViewModel == null)
            {
                return NotFound();
            }

            return View(userViewModel);
        }

        // GET: UserViewModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserViewModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create(UserViewModel userViewModel)
        {
            // 👇 Ajouter ceci pour voir les erreurs de validation
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                foreach (var error in errors)
                {
                    Console.WriteLine("VALIDATION ERROR: " + error);
                }

                return View(userViewModel);
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = userViewModel.Name,
                Email = userViewModel.Email,
                UserName = userViewModel.Email,
                PhoneNumber = userViewModel.PhoneNumber,
                EmailConfirmed = userViewModel.EmailConfirmed
            };

            var result = await _userManager.CreateAsync(user, userViewModel.Password);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                Console.WriteLine("IDENTITY ERROR: " + error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(userViewModel);
        }        // GET: UserViewModels/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userViewModel =_mapper.Map<UserViewModel>( await _context.Set<User>().FindAsync(id));
            if (userViewModel == null)
            {
                return NotFound();
            }
            return View(userViewModel);
        }

        // POST: UserViewModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel userViewModel)
        {
            if (id != userViewModel.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Set<User>().FindAsync(id);
                    user.Name = userViewModel.Name;
                    user.PhoneNumber = userViewModel.PhoneNumber;
                    user.EmailConfirmed = userViewModel.EmailConfirmed;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserViewModelExists(userViewModel.Id))
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
            return View(userViewModel);
        }

        // GET: UserViewModels/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userViewModel =_mapper.Map<User>( await _context.Set<User>()
                .FirstOrDefaultAsync(m => m.Id == id));
            if (userViewModel == null)
            {
                return NotFound();
            }

            return View(userViewModel);
        }

        // POST: UserViewModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Set<User>().FindAsync(id);
            if (user != null)
            {
                _context.Set<User>().Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserViewModelExists(string id)
        {
            return _context.Set<User>().Any(e => e.Id == id);
        }
    }
}
