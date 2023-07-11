using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces;
using BackendBolsaDeTrabajoUTN.Entities;
using BackendBolsaDeTrabajoUTN.Models;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace BackendBolsaDeTrabajoUTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IKnowledgeRepository _knowledgeRepository;

        public AdminController(IAdminRepository adminRepository, IKnowledgeRepository knowledgeRepository)
        {
            _adminRepository = adminRepository;
            _knowledgeRepository = knowledgeRepository;
        }

        [Authorize]
        [HttpPost]
        [Route("createCareer")]
        public IActionResult CreateCareer(AddCareerRequest request)
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            { 
                try
                {
                    List<Career> careers = _adminRepository.GetCareers();
                    ValidateCareerName(careers, request.CareerName);
                    ValidateCareerTotalSubjects(request.CareerTotalSubjects);
                    Career newCareer = new()
                    {
                        CareerName = request.CareerName,
                        CareerAbbreviation = request.CareerAbbreviation,
                        CareerType = request.CareerType,
                        CareerTotalSubjects = request.CareerTotalSubjects,
                        CareerIsActive = true
                    };
                    CareerResponse response = new()
                    {
                        CareerName = newCareer.CareerName,
                        CareerAbbreviation = newCareer.CareerAbbreviation,
                        CareerType = newCareer.CareerType,
                        CareerTotalSubjects = newCareer.CareerTotalSubjects
                    };
                    _adminRepository.CreateCareer(newCareer);
                    return Created("Carrera creada", response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("El usuario no está autorizado para crear Carreras");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("createKnowledge")]
        public IActionResult CreateKnowledge(AddKnowledgeRequest request)
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            {
                try
                {
                    List<Knowledge> knowledge = _knowledgeRepository.GetAllKnowledge();
                    ValidateKnowledgeType(knowledge, request.Type);
                    Knowledge newKnowledge = new()
                    {

                        Type = request.Type,
                        Level = "Bajo",
                        KnowledgeIsActive = true
                    };

                    Knowledge newKnowledge1 = new()
                    {

                        Type = request.Type,
                        Level = "Medio",
                        KnowledgeIsActive = true
                    };

                    Knowledge newKnowledge2 = new()
                    {

                        Type = request.Type,
                        Level = "Alto",
                        KnowledgeIsActive = true
                    };

                    _adminRepository.CreateKnowledge(newKnowledge, newKnowledge1, newKnowledge2);
                    return Ok("Conocimiento creado");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("El usuario no esta autorizado para crear Conocimientos");
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("deleteCareer")]
        public IActionResult DeleteCareer(int id)
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            {
                try
                {
                    _adminRepository.DeleteCareer(id);
                    return Ok("Carrera borrada");
                }
                catch (Exception ex)
                {
                    return Problem(ex.Message);
                }
            }
            else
            {
                return BadRequest("El usuario no está autorizado para borrar carreras");
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("deleteKnowledge/{id}")]
        public IActionResult DeleteKnowledge(int id)
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            {
                try
                {
                    _adminRepository.DeleteKnowledge(id);
                    return Ok("Conocimiento borrado");
                }
                catch (Exception ex)
                {
                    return Problem(ex.Message);
                }
            }
            else
            {
                return BadRequest("El usuario no está autorizado para borrar conocimientos");
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("deleteUser/{userId}")]
        public IActionResult DeleteUser(int id) //función no usada en front
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            {
                try
                {
                    _adminRepository.DeleteUser(id);
                    return Ok("Usuario borrado");
                }
                catch (Exception ex)
                {
                    return Problem(ex.Message);
                }
            }
            else
            {
                return BadRequest("El usuario no está autorizado para borrar otros usuarios");
            }
        }

        [Authorize]
        [HttpDelete] //No implementada en front
        [Route("deleteOffer/{offerId}")]
        public IActionResult DeleteOffer(int id)
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            {
                try
                {
                    _adminRepository.DeleteOffer(id);
                    return Ok("Oferta borrada");
                }
                catch (Exception ex)
                {
                    return Problem(ex.Message);
                }
            }
            else
            {
                return BadRequest("El usuario no está autorizado para borrar Ofertas");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getAllPendingCompanies")]
        public IActionResult GetAllPendingCompanies()
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            {
                try
                {
                    List<Company> pendingCompanies = _adminRepository.GetPendingCompanies();
                    return Ok(pendingCompanies);
                }
                catch (Exception ex)
                {
                    return Problem(ex.Message);
                }
            }
            else
            {
                return BadRequest("El usuario no está autorizado para listar empresas");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("updatePendingCompany/{companyId}")]
        public IActionResult UpdatePendingCompany(int companyId)
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            {
                    try
                     {
                        _adminRepository.UpdatePendingCompany(companyId);
                        return Ok(new { Message = "Estado cambiado"});
                     }
                    catch (Exception ex)
                    {
                        return Problem(ex.Message);
                    }
            }
            else
            {
                    return BadRequest("El usuario no está autorizado para modificar estado de empresas");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getStudentsWithPendingCV")]
        public IActionResult GetStudentsWithPendingCV()
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            {
                try
                {
                    List<Student> studentsWithPendingCV = _adminRepository.GetStudentsWithPendingCV();
                    return Ok(studentsWithPendingCV);
                }
                catch (Exception ex)
                {
                    return NotFound(ex.Message);
                }
            }
            else
            {
                return BadRequest("El usuario no está autorizado para listar estudiantes");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getStudentCV/{studentId}")]
        public ActionResult GetStudentCV(int studentId)
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            {
                try
                {
                    var cv = _adminRepository.GetStudentCV(studentId);
                    return File(cv.File, "application/octet-stream", cv.Name);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("El usuario no está autorizado para descargar el CV de los estudiantes");
            }
        }

        [Authorize]
        [HttpPut]
        [Route("acceptPendingCVFile/{studentId}")]
        public IActionResult AcceptPendingCVFile (int studentId)
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            {
                try
                {
                    _adminRepository.AcceptPendingCVFile(studentId);
                    return Ok(new { Message = "CV confirmado" });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("El usuario no está autorizado para modificar estado de CVs");
            } 
        }

        [Authorize]
        [HttpDelete]
        [Route("deletePendingCVFile/{studentId}")]
        public IActionResult DeletePendingCVFile(int studentId)
        {
            var userType = User.Claims.FirstOrDefault(c => c.Type == "userType")?.Value;
            if (userType == "Admin")
            {
                try
                {
                    _adminRepository.DeletePendingCVFile(studentId);
                    return Ok(new { Message = "CV borrado" });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("El usuario no está autorizado para modificar estado de CVs");
            }
        }

        [NonAction]
        public void ValidateCareerName(List<Career> careers, string careerName)
        {
            try
            {
                var careerNameInUse = careers.FirstOrDefault(c => c.CareerName.ToLower() == careerName.ToLower() && c.CareerIsActive == true);
                if (careerNameInUse != null)
                {
                    throw new Exception("Esta carrera ya existe");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [NonAction]
        public void ValidateCareerTotalSubjects(int totalSubjects) //Manejar que no introduzca un caracter no numérico en front
        {
            try
            {
                if (totalSubjects <= 0)
                {
                    throw new Exception("Total de materias no válido, debe ser mayor que 0");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [NonAction]
        public void ValidateKnowledgeType(List<Knowledge> knowledge, string knowledgeType)
        {
            try
            {
                var knowledgeTypeInUse = knowledge.FirstOrDefault(k => k.Type.ToLower() == knowledgeType.ToLower() && k.KnowledgeIsActive == true);
                if (knowledgeTypeInUse != null)
                {
                    throw new Exception("Este conocimiento ya existe");
                }
            }
            catch (Exception ex)
            {
                throw new Exception (ex.Message);
            }
        }
    }
}
