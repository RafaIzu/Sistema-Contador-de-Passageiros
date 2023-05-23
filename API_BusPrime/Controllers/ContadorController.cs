using BusPrime.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusPrime.Controllers
{
    [ApiController]
    [Route("contador")]
    public class ContadorController : Controller
    {
        private readonly ModelDBContext db;
        public ContadorController(ModelDBContext _db)
        {
            db = _db;
        }

        public IActionResult List(string? searchFilter, int page = 1, int take = 15)
        {
            int count = db.Contagem.Count();
            var dob = ((double)count / (double)take);
            double totalPages = Math.Ceiling(dob);

            int start = (page - 1) * take;
            ListView<IEnumerable<Contagem>> pagination = new ListView<IEnumerable<Contagem>>
            {
                currentPage = page,
                take = take,
                totalPages = int.Parse(totalPages.ToString()),
                totalItems = count,
                list = db.Contagem.Include(x => x.Onibus).Include(x => x.Motorista),
                searchFilter = searchFilter,
            };
            if (!String.IsNullOrEmpty(searchFilter))
            {
                searchFilter = searchFilter.ToLower();
                pagination.list = pagination.list.Where(x =>
                            {
                                var onibusEmpresa = x.Onibus.Empresa.ToLower();
                                var onibusPlaca = x.Onibus.Placa.ToLower();
                                var onibusAssentos = x.Onibus.NumeroAssentos.ToString().ToLower();
                                var motoristaNome = x.Motorista.Nome.ToLower();
                                var motoristaRG = x.Motorista.RG.ToLower();
                                var motoristaCNH = x.Motorista.CNH.ToLower();
                                var totalPessoas = x.TotalPessoas.ToString().ToLower();
                                var dataContagem = x.DataContagem.ToString("dd/MM/yyyy");

                                return onibusEmpresa.Contains(searchFilter)
                                        || searchFilter.Contains(onibusEmpresa)
                                        || onibusPlaca.Contains(searchFilter)
                                        || searchFilter.Contains(onibusPlaca)
                                        || onibusAssentos.Contains(searchFilter)
                                        || searchFilter.Contains(onibusAssentos)
                                        || motoristaNome.Contains(searchFilter)
                                        || searchFilter.Contains(motoristaNome)
                                        || motoristaRG.Contains(searchFilter)
                                        || searchFilter.Contains(motoristaRG)
                                        || motoristaCNH.Contains(searchFilter)
                                        || searchFilter.Contains(motoristaCNH)
                                        || totalPessoas.Contains(searchFilter)
                                        || searchFilter.Contains(totalPessoas)
                                        || dataContagem.Contains(searchFilter)
                                        || searchFilter.Contains(dataContagem);
                            });
                pagination.totalItems = pagination.list.Count();
                pagination.totalPages = (int)(pagination.totalItems / take);
                pagination.totalPages = pagination.totalPages == 0 && pagination.totalItems > 0 ? 1 : pagination.totalPages;
            }

            pagination.list = pagination.list.OrderByDescending(x => x.DataContagem)
                                                .ThenBy(x => x.Onibus.Empresa)
                                                .ThenBy(x => x.Motorista.Nome)
                                                .Skip(start)
                                                .Take(take)
                                                .ToList();


            return View(pagination);
        }


        [HttpPost]
        public ActionResult SendCount([FromQuery] ContagemRequest model)
        {
            try
            {
                if (!db.Motorista.Any(x => x.Id == model.idMot))
                    return BadRequest("Id de motorista não encontrado.");

                if (!db.Onibus.Any(x => x.Id == model.idBus))
                    return BadRequest("Id de ônibus não encontrado.");

                var date = DateTime.Now.AddHours(-3); //gtm sp

                Contagem contagem = new Contagem()
                {
                    Motorista_Id = model.idMot,
                    TotalPessoas = model.contP,
                    Onibus_Id = model.idBus,
                    DataContagem = date,
                };
                
                db.Contagem.Add(contagem);
                db.SaveChanges();

                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}