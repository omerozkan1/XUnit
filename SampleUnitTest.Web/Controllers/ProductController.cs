using Microsoft.AspNetCore.Mvc;
using SampleUnitTest.Web.Models;
using SampleUnitTest.Web.Repository;
using System.Threading.Tasks;

namespace SampleUnitTest.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _repository;

        public ProductController(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAll());
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var product = await _repository.GetById((int)id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Stock,Color")] Product product)
        {
            if (ModelState.IsValid)
            {
                await _repository.Create(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            var product = await _repository.GetById((int)id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Price,Stock,Color")] Product product)
        {
            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _repository.Update(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _repository.GetById((int)id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _repository.GetById(id);

            _repository.Delete(product);
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            var product = _repository.GetById(id).Result;

            if (product == null)
                return false;
            else
                return true;
        }
    }
}
