using BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces;
using BackendBolsaDeTrabajoUTN.Entities;
using BackendBolsaDeTrabajoUTN.DBContexts;


namespace BackendBolsaDeTrabajoUTN.Data.Repository
{
   public class CompanyRepository : ICompanyRepository
   {
        private readonly TPContext _context;
        public CompanyRepository(TPContext context)
        {
            _context = context;
        }


        public void CreateCompany(Company newCompany)
        {
            try
            {
                _context.Companies.Add(newCompany);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {


                throw new Exception("el error es" + ex);
            }
        }

        public void RemoveCompany(int id)
        {
            try
            {
                var company = _context.Companies.FirstOrDefault(s => s.UserId == id);
                if (company == null)
                {
                    throw new Exception("Empresa no encontrada");
                }
                company.UserIsActive = false;
                var companyOffers = _context.Offers.Where(o => o.CompanyId == id).ToList();
                
                foreach (var offer in companyOffers)
                {
                    offer.OfferIsActive = false;
                }
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("Usuario no encontrado");
            }
        }

        public void CreateOffer(Offer newOffer)
        {
            try
            {
                _context.Offers.Add(newOffer);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("el error es" + ex);
            }
        }
        public List<User> GetUsers()
        {
            return _context.Users.ToList();
        }
        public List<Company> GetCompanies()
        {
            return _context.Companies.ToList();
        }

        public List<Student> GetStudentsInOffer(int offerId)
        {
            try
            {
                var studentsInOffer = _context.Offers.FirstOrDefault(o => o.OfferId == offerId && o.OfferIsActive == true).Students.ToList();
                List<Student> studentsToReturn = new List<Student>();
                foreach (var student in studentsInOffer)
                {
                    var studentOffer = _context.StudentOffers.First(so => so.StudentId == student.UserId && so.OfferId == offerId);
                    if (studentOffer != null)
                    {
                        studentsToReturn.Add(student);
                    }
                }
                return studentsToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Knowledge> GetStudentKnowledge(int userId)
        {
            var knowledge = _context.StudentKnowledge
                .Where(sk => sk.UserId == userId && sk.StudentKnowledgeIsActive == true)
                .Select(sk => new Knowledge
                {
                    KnowledgeId = sk.Knowledge.KnowledgeId,
                    Type = sk.Knowledge.Type,
                    Level = sk.Knowledge.Level,
                })
                .ToList();
            return knowledge;
        }

        public CVFile GetStudentCv(int studentId)
        {
            try
            {
                CVFile cVFile = _context.CVFiles.FirstOrDefault(c => c.StudentId == studentId && c.CVIsActive == true);
                if (cVFile == null)
                {
                    throw new Exception("No se encontró el CV del estudiante");
                }
                return cVFile;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}