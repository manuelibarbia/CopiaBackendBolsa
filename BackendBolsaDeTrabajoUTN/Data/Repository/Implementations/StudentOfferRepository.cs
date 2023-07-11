using BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces;
using BackendBolsaDeTrabajoUTN.DBContexts;
using BackendBolsaDeTrabajoUTN.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendBolsaDeTrabajoUTN.Data.Repository.Implementations
{
    public class StudentOfferRepository : IStudentOfferRepository
    {
        private readonly TPContext _context;
        public StudentOfferRepository(TPContext context)
        {
            _context = context;
        }

        public void AddStudentToOffer(int offerId, int studentId)
        {
            var offer = _context.Offers.FirstOrDefault(o => o.OfferId == offerId);
            var student = _context.Students.FirstOrDefault(s => s.UserId == studentId);

            if (offer == null || student == null)
            {
                throw new Exception("No existe la oferta o el estudiante");
            }

            var isAssociated = _context.StudentOffers.Any(os => os.StudentId == student.UserId && os.OfferId == offerId && os.StudentOfferIsActive == true);
            if (isAssociated)
            {
                throw new Exception("Ya estás asociado a esta oferta.");
            }
            var studentOfferExists = _context.StudentOffers.FirstOrDefault(os => os.StudentId == student.UserId && os.OfferId == offerId);
            try
            {
                if (studentOfferExists == null)
                {
                    var studentOffer = new StudentOffer
                    {
                        OfferId = offerId,
                        StudentId = studentId,
                        StudentOfferIsActive = true,
                        ApplicationDate = DateTime.Now
                    };
                    _context.StudentOffers.Add(studentOffer);
                }
                else
                {
                    _context.StudentOffers.First(so => so.StudentId == student.UserId && so.OfferId == offer.OfferId).StudentOfferIsActive = true;
                    _context.StudentOffers.First(so => so.StudentId == student.UserId && so.OfferId == offer.OfferId).ApplicationDate = DateTime.Now;
                }
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw new Exception(ex.InnerException.Message);
                }
                 throw new Exception (ex.Message);   
            }
        }

        public void DeleteStudentToOffer(int offerId, int studentId)
        {
            var offer = _context.Offers.FirstOrDefault(o => o.OfferId == offerId);
            var student = _context.Students.FirstOrDefault(s => s.UserId == studentId);

            if (offer == null || student == null)
            {
                throw new Exception("No existe la oferta o el estudiante");
            }

            var studentOffer = _context.StudentOffers.FirstOrDefault(so => so.StudentId == student.UserId && so.OfferId == offerId);
            if (studentOffer == null)
            {
                throw new Exception("El estudiante no está asociado a esta oferta.");
            }

            studentOffer.StudentOfferIsActive = false;
            _context.SaveChanges();
        }

        public List<StudentOffer> GetStudentOffers(int studentId)
        {
            var studentOffers = _context.StudentOffers
                .Where(so => so.StudentId == studentId && so.StudentOfferIsActive == true && so.Offer.OfferIsActive == true)
                .Include(so => so.Offer)
                .ThenInclude(o => o.Company)
                .OrderByDescending(so => so.ApplicationDate)
                .Select(so => new StudentOffer
                {
                    Offer = so.Offer,
                    ApplicationDate = so.ApplicationDate,
                })
                .ToList();

            return studentOffers;
        }

        public List<StudentOffer> GetStudentOfferHistory(int studentId) //FUNCIÓN NUEVA
        {
            var offerHistory = _context.StudentOffers
                .Where(so => so.StudentId == studentId)
                .Include(so => so.Offer)
                .ThenInclude(o => o.Company)
                .OrderByDescending(so => so.ApplicationDate)
                .Select(so => new StudentOffer
                {
                    Offer = so.Offer,
                    ApplicationDate = so.ApplicationDate,
                    StudentOfferIsActive = so.StudentOfferIsActive
                })
                .ToList();

            return offerHistory;
        }

        public List<Student> GetStudentsInOffers(int offerId)
        {
            var students = _context.StudentOffers
                .Where(so => so.OfferId == offerId && so.StudentOfferIsActive == true)
                .Select(so => new Student
                {
                    UserId = so.Student.UserId,
                    UserName = so.Student.UserName,
                    Name = so.Student.Name,
                    Surname = so.Student.Surname,
                    AltEmail = so.Student.AltEmail,
                })
                .ToList();
            return students;
        }
    }
}
