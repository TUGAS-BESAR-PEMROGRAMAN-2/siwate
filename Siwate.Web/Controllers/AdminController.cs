using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Siwate.Web.Data;
using Siwate.Web.Models;
using Siwate.Web.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Siwate.Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly SiwateDbContext _context;
        private readonly IMachineLearningService _mlService;

        public AdminController(SiwateDbContext context, IMachineLearningService mlService)
        {
            _context = context;
            _mlService = mlService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.QuestionCount = await _context.Questions.CountAsync();
            ViewBag.UserCount = await _context.Users.CountAsync();
            return View();
        }

        public async Task<IActionResult> Questions()
        {
            var questions = await _context.Questions.OrderByDescending(q => q.CreatedAt).ToListAsync();
            return View(questions);
        }

        [HttpPost]
        public async Task<IActionResult> AddQuestion(string questionText)
        {
            if (!string.IsNullOrWhiteSpace(questionText))
            {
                _context.Questions.Add(new Question { QuestionText = questionText });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Questions");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(Guid id)
        {
            var q = await _context.Questions.FindAsync(id);
            if (q != null)
            {
                _context.Questions.Remove(q);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Questions");
        }


    }
}
