using BackendBolsaDeTrabajoUTN.Entities;
using BackendBolsaDeTrabajoUTN.Models;

namespace BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces
{
    public interface IAdminRepository
    {
        public void CreateCareer(Career newCareer);
        public List<Career> GetCareers();
        public void CreateKnowledge(Knowledge newKnowledge, Knowledge newKnowledge1, Knowledge newKnowledge2);
        public void DeleteCareer(int id);
        public void DeleteKnowledge(int id);
        public void DeleteUser(int id);
        public void DeleteOffer(int id);
        List<Company> GetPendingCompanies();
        public void UpdatePendingCompany(int companyId);
        public void DeletePendingCompany(int companyId);
        public List<Student> GetStudentsWithPendingCV();
        public CVFile GetStudentCV(int studentId);
        public void AcceptPendingCVFile(int studentId);
        public void DeletePendingCVFile(int studentId);
    }
}
