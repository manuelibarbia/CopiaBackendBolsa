using BackendBolsaDeTrabajoUTN.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces
{
    public interface IStudentOfferRepository
    {
        void AddStudentToOffer(int offerId, int studentId);
        void DeleteStudentToOffer(int offerId, int studentId);
        List<StudentOffer> GetStudentOffers(int studentId);
        public List<Student> GetStudentsInOffers(int offerId);
        public List<StudentOffer> GetStudentOfferHistory(int studentId); //FUNCIÓN NUEVA
    }
}
