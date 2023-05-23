using BusPrime.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NToastNotify;
//using NToastNotify;

namespace BusPrime.Controllers
{
    [Route("motorista")]
    public class MotoristaController : Controller
    {
        private readonly ModelDBContext db;
        private readonly IToastNotification _toastNotification;

        public MotoristaController(ModelDBContext _db, IToastNotification toastNotification)
        //public MotoristaController(ModelDBContext _db)
        {
            db = _db;
            _toastNotification = toastNotification;
            /*
            toastr.AddSuccessToastMessage("Woo hoo - it works!");  // Success Toast
            toastr.AddInfoToastMessage("Here is some information.");  // Info Toast
            toastr.AddErrorToastMessage("Woops an error occured.");  // Error Toast
            toastr.AddWarningToastMessage("Here is a simple warning!");  // Warning Toast
            */
        }

        public IActionResult List(string searchFilter, int page = 1, int take = 15)
        {
            _toastNotification.AddSuccessToastMessage("Woo hoo - it works!");  // Success Toast
            int count = db.Motorista.Count();
            var dob = ((double)count / (double)take);
            double totalPages = Math.Ceiling(dob);
            int start = (page - 1) * take;
            ListView<IEnumerable<Motorista>> pagination = new ListView<IEnumerable<Motorista>>
            {
                currentPage = page,
                take = take,
                totalPages = int.Parse(totalPages.ToString()),
                totalItems = count,
                list = db.Motorista,
                searchFilter = searchFilter,
            };
            if (!String.IsNullOrEmpty(searchFilter))
            {
                searchFilter = searchFilter.ToLower();
                pagination.list = pagination.list
                            .Where(x => x.Nome.ToLower().Contains(searchFilter)
                                     || searchFilter.Contains(x.Nome.ToLower())
                                     || x.CNH.ToLower().Contains(searchFilter)
                                     || searchFilter.Contains(x.CNH.ToLower())
                                     || x.RG.ToLower().Contains(searchFilter)
                                     || searchFilter.Contains(x.RG.ToLower())
                            );
                pagination.totalItems = pagination.list.Count();
                pagination.totalPages = (int)(pagination.totalItems / take);
                pagination.totalPages = pagination.totalPages == 0 && pagination.totalItems > 0 ? 1 : pagination.totalPages;
            }

            pagination.list = pagination.list.OrderBy(x => x.Nome)
                                                .Skip(start)
                                                .Take(take)
                                                .ToList();
            return View(pagination);
        }

        [HttpGet("cadastrar")]
        public IActionResult Create()
        {
            return View(new Motorista());
        }
        [HttpPost("cadastrar"), ValidateAntiForgeryToken]
        public IActionResult CreateConfirmed(Motorista model)
        {
            if (!ModelState.IsValid) {
                var errors = ModelState.Values.Where(x => x.Errors.Count > 0).ToList();
                // toastr.AddErrorToastMessage("Não foi possível finalizar operação");  
                AddErrors(errors);
                return View("Create", model);
            }
            if (db.Motorista.Any(x => x.CNH == model.CNH))
            {
                // toastr.AddErrorToastMessage("Não foi possível finalizar operação");  
                ModelState.AddModelError("CNH", "Essa CNH já está cadastrada");
                return View("Create", model);
            }
            if (db.Motorista.Any(x => x.RG == model.RG))
            {
                // toastr.AddErrorToastMessage("Não foi possível finalizar operação");  
                ModelState.AddModelError("RG", "Esse RG já está cadastrado");
                return View("Create", model);
            }
            try
            {
                db.Motorista.Add(model);
                db.SaveChanges();
                // toastr.AddSuccessToastMessage("Operação concluída com sucesso!"); 
            } 
            catch( Exception e)
            {
                // toastr.AddErrorToastMessage("Não foi possível finalizar operação");  
                // toastr.AddErrorToastMessage(e.Message);  
            }

            return RedirectToAction("List");
        }

        [HttpGet("editar/{id}")]
        public IActionResult Edit([FromRoute] int id)
        {
            var model = db.Motorista.Find(id);
            if (model == null)
            {
                // toastr.AddErrorToastMessage("Não foi possível finalizar operação");
                return RedirectToAction("List");
            }

            return View(model);
        }

        [HttpPost("editar"), ValidateAntiForgeryToken]
        public IActionResult EditConfirmed(Motorista model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.Where(x => x.Errors.Count > 0).ToList();
                AddErrors(errors);
                // toastr.AddErrorToastMessage("Não foi possível finalizar operação");
                return View("Edit", model);
            }
            if (db.Motorista.Any(x => x.CNH == model.CNH && x.Id != model.Id))
            {
                // toastr.AddErrorToastMessage("Não foi possível finalizar operação");
                ModelState.AddModelError("CNH", "Essa CNH já está cadastrada");
                return View("Edit", model);
            }
            if (db.Motorista.Any(x => x.RG == model.RG && x.Id != model.Id))
            {
                // toastr.AddErrorToastMessage("Não foi possível finalizar operação");
                ModelState.AddModelError("RG", "Esse RG já está cadastrado");
                return View("Edit", model);
            }
            try
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                // toastr.AddSuccessToastMessage("Operação concluída com sucesso!");
            }
            catch (Exception e)
            {
                // toastr.AddErrorToastMessage("Não foi possível finalizar operação");
                // toastr.AddErrorToastMessage(e.Message);
            }

            return RedirectToAction("List");
        }
        
        [HttpPost("delete/{id}"), ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var model = db.Motorista
                .Include(x => x.Contagem)
                .FirstOrDefault(x => x.Id == id);

            if (model == null)
            {
                ModelState.AddModelError("Delete", "Não foi possível excluir esse model");
                // toastr.AddErrorToastMessage("Não foi possível finalizar operação");
                return RedirectToAction("List");
            }
            try
            {
                db.Remove(model);
                db.SaveChanges();
                // toastr.AddSuccessToastMessage("Operação concluída com sucesso!");
            } 
            catch(Exception e)
            {
                ModelState.AddModelError("Delete", e.Message);
                // toastr.AddErrorToastMessage("Não foi possível finalizar operação");
                // toastr.AddErrorToastMessage(e.Message);
            }
            return RedirectToAction("List");
        }



        private void AddErrors(List<ModelStateEntry> errors)
        {
            foreach (var error in errors)
            {
                var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(error);
                var a = JObject.Parse(jsonString);
                var b = a["Errors"].ToList();
                string key = a["Key"].ToString();
                foreach (var e in b)
                {
                    string message = e["ErrorMessage"].ToString();
                    ModelState.AddModelError(key, message);
                    Console.WriteLine(e);
                }
            }
        }

    }
}
