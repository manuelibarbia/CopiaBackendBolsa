﻿// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces;
using BackendBolsaDeTrabajoUTN.Entities;
using BackendBolsaDeTrabajoUTN.Models;
using System.Security.Claims;
using BackendBolsaDeTrabajoUTN.DBContexts;
using System.Text.RegularExpressions;

namespace BackendBolsaDeTrabajoUTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class StudentController : ControllerBase
    {
        
        private readonly IStudentRepository _studentRepository;

        private readonly ICompanyRepository _companyRepository;

        private readonly IStudentOfferRepository _studentOfferRepository;

        private readonly IStudentKnowledgeRepository _studentKnowledgeRepository;

        public StudentController(IStudentRepository studentRepository, ICompanyRepository companyRepository, IStudentOfferRepository studentOfferRepository, IStudentKnowledgeRepository studentKnowledgeRepository)
        {   
            _studentRepository = studentRepository;

            _companyRepository = companyRepository;

            _studentOfferRepository = studentOfferRepository;

            _studentKnowledgeRepository = studentKnowledgeRepository;
        }

        [HttpPost]
        [Route("createStudent")]
        public IActionResult CreateStudent(AddStudentRequest request)
        {
                try
                {
                    List<User> users = _studentRepository.GetUsers();
                    List<Student> students = _studentRepository.GetStudents();
                    ValidateUserName(users, request.UserName);
                    ValidatePassword(request.Password, request.ConfirmPassword, request.UserName);
                    ValidateFile(students, request.File);
                    ValidateUserEmail(students, users, request.UserEmail);
                    ValidateAltEmail(users, students, request.AltEmail);
                    ValidateDocumentNumber(students, request.DocumentNumber);
                    ValidateCUIL_CUIT(students, request.CUIL_CUIT);
                    ValidateBirth(request.Birth);

                    Student newStudent = new()
                    {
                        UserName = request.UserName,
                        Password = request.Password,                       
                        File = request.File,
                        Name = request.Name,
                        Surname = request.Surname,
                        UserEmail = request.UserEmail.ToLower(),
                        AltEmail = request.AltEmail.ToLower(),
                        DocumentType = request.DocumentType,
                        DocumentNumber = request.DocumentNumber,
                        CUIL_CUIT = request.CUIL_CUIT,
                        Birth = request.Birth,
                        Sex = request.Sex,
                        CivilStatus = request.CivilStatus,

                        UserIsActive = true
                    };
                    StudentResponse response = new()
                    {
                        File = newStudent.File,
                        Name = newStudent.Name,
                        Surname = newStudent.Surname,
                    };
                    _studentRepository.CreateStudent(newStudent);
                    return Created("Estudiante creado", response);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
        }

        [Authorize]
        [HttpPut]
        [Route("addStudentAdressInfo")]
        public IActionResult AddStudentAdressInfo(AddStudentAdressInfoRequest request)
        {
            try
            {
                List<Student> students = _studentRepository.GetStudents();
                List<Company> companies = _companyRepository.GetCompanies();
                ValidatePhoneNumbers(students, companies, request.FamilyPersonalPhone, request.FamilyOtherPhone, request.PersonalPersonalPhone, request.PersonalOtherPhone);
                int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _studentRepository.AddStudentAdressInfo(id, request);
                return Ok(new { message = "Domicilios modificados" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("updateStudentUniversityInfo")]
        public IActionResult AddStudentUniversityInfo(AddStudentUniversityInfoRequest newStudentUniversityInfo)
        {
            try
            {
                int id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _studentRepository.AddStudentUniversityInfo(id, newStudentUniversityInfo);
                return Ok(new { message = "Datos universitarios modificados" });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [Authorize]
        [HttpPut]
        [Route("Knowledge/{knowledgeId}/AddKnowledge")]
        public IActionResult AddStudentKnowledge(int knowledgeId)
        {
            try
            {
                int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _studentKnowledgeRepository.AddStudentKnowledge(knowledgeId, studentId);
                return Ok(new { message = "Nuevo conocimiento agregado" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("Knowledge/{knowledgeId}/DeleteKnowledge")]
        public IActionResult DeleteStudentKnowledge(int knowledgeId)
        {
            try
            {
                int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _studentKnowledgeRepository.DeleteStudentKnowledg(knowledgeId, studentId);
                return Ok(new { message = "Conocimiento borrado del estudiante" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("Knowledge/GetKnowledgesToStudent")]
        public IActionResult GetAllKnowledgeToStudent()
        {
            try
            {
                int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                List<Knowledge> knowledgesToStudent = _studentKnowledgeRepository.GetAllKnowledgeToStudent(studentId);
                return Ok(knowledgesToStudent);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("{offerId}/Students/{studentId}")]
        public ActionResult AddStudentToOffer(int offerId, int studentId)
        {
            try
            {
                var cvExists = _studentRepository.CheckCVExists(studentId);
                if (cvExists == false)
                {
                    throw new Exception("Para postularte a una oferta, primero tenés que cargar tu CV");
                }
                _studentOfferRepository.AddStudentToOffer(offerId, studentId);
                return Ok(new { message = "Registro en la oferta exitoso" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{offerId}/DeleteStudent/{studentId}")] 
        public ActionResult DeleteStudentToOffer(int offerId, int studentId) 
        { 
            try 
            { 
                _studentOfferRepository.DeleteStudentToOffer(offerId, studentId); 
                return Ok(); 
            } 
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [Authorize]
        [HttpGet("{studentId}/Offers")]
        public ActionResult GetStudentOffers(int studentId)
        {
            try
            {
                var offers = _studentOfferRepository.GetStudentOffers(studentId);
                return Ok(offers);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("deleteStudent/{id}")]
        public IActionResult DeleteStudent()
        {
            try
            {
                int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                _studentRepository.RemoveStudent(studentId);
                return Ok("Alumno borrado del sistema.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("uploadCV")]
        public IActionResult UploadCV(IFormFile file)
        {
            try
            {
                string studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (studentId == null)
                {
                    throw new Exception("No se encontró al estudiante");
                }
                if (file == null || file.Length == 0)
                {
                    throw new Exception("No se ha enviado ningún archivo.");
                }

                _studentRepository.UploadStudentCV(int.Parse(studentId), file);

                return Ok(new { message = "Archivo guardado exitosamente en la base de datos." });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("downloadCV")]
        public IActionResult DownloadCV()
        {
            try
            {
                int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                if (string.IsNullOrEmpty(studentId.ToString()))
                {
                    throw new Exception("No está logeado");
                }

                CVFile cvFile = _studentRepository.GetStudentCv(studentId);

                return File(cvFile.File, "application/octet-stream", cvFile.Name);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("getStudentOfferHistory/{studentId}")]
        public IActionResult GetStudentOfferHistory() //FUNCIÓN NUEVA
        {
            try
            {
                int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                if (string.IsNullOrEmpty(studentId.ToString()))
                {
                    throw new Exception("No está logeado");
                }
                var studentOfferHistory = _studentOfferRepository.GetStudentOfferHistory(studentId);

                return Ok(studentOfferHistory);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        public void ValidateDocumentNumber(List<Student> students, int documentNumber)
        {
            try
            {
                if (documentNumber.ToString().Length != 8)
                {
                    throw new Exception("Número de documento no válido, debe tener una longitud de 8 dígitos.");
                }
                var inUse = students.FirstOrDefault(s => s.DocumentNumber == documentNumber);
                if (inUse != null)
                {
                    throw new Exception("Número de documento ya registrado");
                }
            }
            catch (FormatException)
            {
                throw new Exception("El documento debe ser un número entero.");
            }
        }

        [NonAction]
        public void ValidateCUIL_CUIT(List<Student> students, long CUIL_CUIT)
        {
            try
            {
                if (CUIL_CUIT.ToString().Length != 11)
                {
                    throw new Exception("CUIT no válido, debe tener una longitud de 11 dígitos.");
                }
                var inUse = students.FirstOrDefault(s => s.CUIL_CUIT == CUIL_CUIT);
                if (inUse != null)
                {
                    throw new Exception("CUIT ya registrado");
                }
            }
            catch (FormatException)
            {
                throw new Exception("El CUIT debe ser un número entero.");
            }
        }

        [NonAction]
        public void ValidateFile(List<Student> students, int file)
        {
            try
            {
                if (file.ToString().Length != 5)
                {
                    throw new Exception("Legajo no válido, debe tener una longitud de 5 dígitos.");
                }
                var inUse = students.FirstOrDefault(s => s.File == file);
                if (inUse != null)
                {
                    throw new Exception("Legajo ya registrado");
                }
            }
            catch (FormatException)
            {
                throw new Exception("El legajo debe ser un número entero");
            }
        }

        [NonAction]
        public void ValidateUserName(List<User> users, string userName)
        {
            var inUse = users.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            if (inUse != null)
            {
                throw new Exception("Nombre de usuario ya utilizado");
            }
        }

        [NonAction]
        public void ValidateUserEmail(List<Student> students, List<User> users, string userEmail)
        {
            try
            {
                if (!userEmail.EndsWith("@frro.utn.edu.ar"))
                {
                    throw new Exception("El correo electrónico principal debe terminar en @frro.utn.edu.ar");
                }
                int atIndex = userEmail.IndexOf("@");
                if (atIndex <= 0)
                {
                    throw new Exception("El correo electrónico principal debe contener texto antes de @frro.utn.edu.ar");
                }
                var inUseStudent = students.FirstOrDefault(s => s.UserEmail.ToLower() == userEmail.ToLower()); //Comparamos con mails alternativos de estudiantes
                var inUseUser = users.FirstOrDefault(u => u.UserEmail.ToLower() == userEmail.ToLower()); //Comparamos con todos los email de user (incluyendo empresas)
                if (inUseStudent != null || inUseUser != null)
                {
                    throw new Exception("El correo electrónico principal introducido ya está registrado");
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [NonAction]
        public void ValidateAltEmail(List<User> users, List<Student> students, string altEmail)
        {
            try
            {
                if (altEmail.EndsWith("@frro.utn.edu.ar"))
                {
                    throw new Exception("Correo electrónico alternativo no válido, está intentando introducir un correo institucional");
                }
                if (!IsValidEmail(altEmail))
                {
                    throw new Exception("Correo electrónico alternativo no válido");
                }
                var inUseStudent = students.FirstOrDefault(s => s.AltEmail.ToLower() == altEmail.ToLower()); 
                var inUseUser = users.FirstOrDefault(u => u.UserEmail.ToLower() == altEmail.ToLower());
                if (inUseStudent != null || inUseUser != null)
                {
                    throw new Exception("El correo electrónico alternativo introducido ya está registrado");
                }
            }
            catch (FormatException)
            {
                throw new Exception("El correo electrónico alternativo debe ser válido");
            }
        }
        private bool IsValidEmail(string altEmail)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(altEmail);

                if (!Regex.IsMatch(addr.Host, @"^[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                {
                    return false;
                }

                return addr.Address == altEmail;
            }
            catch
            {
                return false;
            }
        }

        [NonAction]
        public void ValidatePassword(string password, string confirmPassword, string userName)
        {
            try
            {
                if (password.Length < 8)
                {
                    throw new Exception("Contraseña insegura, debe tener al menos 8 caracteres");
                }

                bool hasUpperCase = password.Any(char.IsUpper);
                bool hasLowerCase = password.Any(char.IsLower);
                bool hasDigit = password.Any(char.IsDigit);
                bool hasSpecialChar = password.Any(c => !char.IsLetterOrDigit(c));

                if (!hasUpperCase || !hasLowerCase || !hasDigit || !hasSpecialChar)
                {
                    throw new Exception("Contraseña insegura, debe contener al menos una letra mayúscula, una minúscula, un número y un caracter especial");
                }

                if (password == userName)
                {
                    throw new Exception("Contraseña insegura, no puede ser igual al nombre de usuario");
                }

                if (password != confirmPassword)
                {
                    throw new Exception("Los campos introducidos de contraseña no coinciden");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [NonAction]
        public void ValidateBirth(DateTime birth)
        {
            try
            {
                DateTime eighteenYearsAgo = DateTime.Now.AddYears(-18);

                if (birth > eighteenYearsAgo)
                {
                    throw new Exception("Fecha de nacimiento no válida, debes tener al menos 18 años para registrarte.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [NonAction]
        public void ValidatePhoneNumbers(List<Student> students, List<Company> companies, long familyPersonalPhone, long familyOtherPhone, long personalPersonalPhone, long personalOtherPhone)
        {
            try
            {
                if (familyPersonalPhone.ToString().Length != 10)
                {
                    throw new Exception("Teléfono de familia no válido, debe tener 10 dígitos");
                }
                if (familyOtherPhone.ToString().Length != 10)
                {
                    throw new Exception("Teléfono de familia alternativo no válido, debe tener 10 dígitos");
                }
                if (personalPersonalPhone.ToString().Length != 10)
                {
                    throw new Exception("Teléfono personal no válido, debe tener 10 dígitos");
                }
                if (personalOtherPhone.ToString().Length != 10)
                {
                    throw new Exception("Teléfono personal alternativo no válido, debe tener 10 dígitos");
                }

                var phoneNumbers = new List<long>
                {
                    familyPersonalPhone,
                    familyOtherPhone,
                    personalPersonalPhone,
                    personalOtherPhone
                };

                if (phoneNumbers.Distinct().Count() != phoneNumbers.Count)
                {
                    throw new Exception("Los números de teléfono no deben coincidir entre sí");
                }


                var familyPersonalPhoneInUseStudent = students.FirstOrDefault(s => s.FamilyPersonalPhone == familyPersonalPhone || s.FamilyOtherPhone == familyPersonalPhone || s.PersonalPersonalPhone == familyPersonalPhone || s.PersonalOtherPhone == familyPersonalPhone);
                var familyPersonalPhoneInUseCompany = companies.FirstOrDefault(c => c.CompanyPhone == familyPersonalPhone || c.CompanyPersonalPhone == familyPersonalPhone);

                var familyOtherPhoneInUseStudent = students.FirstOrDefault(s => s.FamilyPersonalPhone == familyOtherPhone || s.FamilyOtherPhone == familyOtherPhone || s.PersonalPersonalPhone == familyOtherPhone || s.PersonalOtherPhone == familyOtherPhone);
                var familyOtherPhoneInUseCompany = companies.FirstOrDefault(c => c.CompanyPhone == familyOtherPhone || c.CompanyPersonalPhone == familyOtherPhone);

                var personalPersonalPhoneInUseStudent = students.FirstOrDefault(s => s.FamilyPersonalPhone == personalPersonalPhone || s.FamilyOtherPhone == personalPersonalPhone || s.PersonalPersonalPhone == personalPersonalPhone || s.PersonalOtherPhone == personalPersonalPhone);
                var personalPersonalPhoneInUseCompany = companies.FirstOrDefault(c => c.CompanyPhone == personalPersonalPhone || c.CompanyPersonalPhone == personalPersonalPhone);

                var personalOtherPhoneInUseStudent = students.FirstOrDefault(s => s.FamilyPersonalPhone == personalOtherPhone || s.FamilyOtherPhone == personalOtherPhone || s.PersonalPersonalPhone == personalOtherPhone || s.PersonalOtherPhone == personalOtherPhone);
                var personalOtherPhoneInUseCompany = companies.FirstOrDefault(c => c.CompanyPhone == personalOtherPhone || c.CompanyPersonalPhone == personalOtherPhone);

                if (familyPersonalPhoneInUseStudent != null ||familyPersonalPhoneInUseCompany != null)
                {
                    throw new Exception("El teléfono de familia introducido ya está en uso");
                }
                if (familyOtherPhoneInUseStudent != null || familyOtherPhoneInUseCompany != null)
                {
                    throw new Exception("El teléfono de familia alternativo introducido ya está en uso");
                }
                if (personalPersonalPhoneInUseStudent != null || personalPersonalPhoneInUseCompany != null)
                {
                    throw new Exception("El teléfono personal introducido ya está en uso");
                }
                if (personalOtherPhoneInUseStudent != null || personalOtherPhoneInUseCompany != null)
                {
                    throw new Exception("El teléfono personal alternativo introducido ya está en uso");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}