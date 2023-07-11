using BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces;
using BackendBolsaDeTrabajoUTN.DBContexts;
using BackendBolsaDeTrabajoUTN.Entities;
using System.Linq;

namespace BackendBolsaDeTrabajoUTN.Data.Repository.Implementations
{
    public class AdminRepository : IAdminRepository
    {
        private readonly TPContext _context;
        public AdminRepository(TPContext context)
        {
            _context = context;
        }

        public void CreateCareer(Career newCareer)
        {
            try
            {
                _context.Careers.Add(newCareer);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Career> GetCareers()
        {
            var careers = _context.Careers.ToList();
            return careers;
        }

        public void CreateKnowledge(Knowledge newKnowledge, Knowledge newKnowledge1, Knowledge newKnowledge2)
        {
            try
            {
                _context.Knowledges.Add(newKnowledge);
                _context.Knowledges.Add(newKnowledge1);
                _context.Knowledges.Add(newKnowledge2);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteCareer(int id)
        {
            try
            {
                var career = _context.Careers.FirstOrDefault(c => c.CareerId == id);
                if (career == null)
                {
                    throw new Exception("Carrera no encontrada");
                }
                career.CareerIsActive = false;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void DeleteKnowledge(int id)
        {
            try
            {
                var knowledge =_context.Knowledges.FirstOrDefault(k => k.KnowledgeId == id);
                if (knowledge == null)
                {
                    throw new Exception("Conocimiento no encontrado");
                }
                knowledge.KnowledgeIsActive = false;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteUser(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == id);
                if (user == null)
                {
                    throw new Exception("Usuario no encontrado");
                }
                user.UserIsActive = false;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteOffer(int id)
        {
            try
            {
                var offer = _context.Offers.FirstOrDefault(o => o.OfferId == id);
                if (offer == null)
                {
                    throw new Exception("Oferta no encontrada");
                }
                var studentOffers = _context.StudentOffers.Where(x => x.OfferId == id).ToList();
                if (studentOffers.Count > 0)
                {
                    foreach (var studentOffer in studentOffers)
                    {
                        studentOffer.StudentOfferIsActive = false;
                    }
                }
                offer.OfferIsActive = false;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Company> GetPendingCompanies()
        {
            return _context.Companies
                .Where(u => u.UserType == "Company" && u.CompanyPendingConfirmation == true)
                .ToList();
        }

        public void UpdatePendingCompany(int companyId)
        {
            Company company = _context.Companies.FirstOrDefault(c => c.UserId == companyId);
            if (company != null)
            {
                company.CompanyPendingConfirmation = false;
                _context.Update(company);
                _context.SaveChanges();
            }
        }

        public List<Student> GetStudentsWithPendingCV()
        {
            try
            {
                var pendingCvFiles = _context.CVFiles.Where(cv => cv.CVPendingConfirmation == true && cv.CVIsActive == true).ToList();
                if (pendingCvFiles.Count == 0)
                {
                    throw new Exception("No hay CVs pendientes de confirmar");
                }
                var studentsWithPendingCV = new List<Student>();
                foreach (CVFile pendingCV in pendingCvFiles)
                {
                    studentsWithPendingCV.Add(_context.Students.First(s => s.UserId == pendingCV.StudentId));
                }
                return studentsWithPendingCV;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AcceptPendingCVFile(int studentId) //No implementado en front
        {
            try
            {
                CVFile cVFile = _context.CVFiles.FirstOrDefault(cv => cv.StudentId == studentId);
                if (cVFile == null)
                {
                    throw new Exception("El CV no existe");
                }
                cVFile.CVPendingConfirmation = false;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeletePendingCVFile(int studentId)
        {
            try
            {
                CVFile cVFile = _context.CVFiles.FirstOrDefault(cv => cv.StudentId == studentId);
                if (cVFile == null)
                {
                    throw new Exception("El CV no existe");
                }
                cVFile.CVIsActive = false;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
