using Fiorello13.Data;
using Fiorello13.Models;
using Fiorello13.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fiorello13.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            List<Product> products = await _context.Products.Where(p=> p.IsDeleted==false)
                .Include(m => m.Category)
                .Include(m => m.Images)
                .OrderByDescending(m=> m.Id)
                .Take(8)
                .ToListAsync();
            return View(products);
        }
        public async Task<IActionResult> LoadMore()
        {

            List<Product> products = await _context.Products.Where(p => p.IsDeleted == false)
               
                .OrderByDescending(m => m.Id)
                .Skip(4)
                .Take(4)
                .ToListAsync();
            return Json(products);
        }

        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id is null) return NotFound();
            Product dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null) return BadRequest();

            List<BasketVM> basket;
            if (Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            else
            {
                basket = new List<BasketVM>();
            }

            var existProduct = basket.Find(m => m.Id == dbProduct.Id);
            if (existProduct == null)
            {
                basket.Add(new BasketVM
                {
                    Id = dbProduct.Id,
                    Count = 1

                });
            }
            else
            {
                existProduct.Count++; 
            }

        
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));

            return RedirectToAction("Index", "Home");
           
        }

        public IActionResult Test()
        {



            return Json(JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]));

        }
    }
}
