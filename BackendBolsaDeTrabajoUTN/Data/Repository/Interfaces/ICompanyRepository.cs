using BackendBolsaDeTrabajoUTN.Entities;


namespace BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces
{
    public interface ICompanyRepository
    {
        public void CreateCompany(Company newCompany);
        public void RemoveCompany(int id);
        public void CreateOffer(Offer newOffer);
        public List<User> GetUsers();
        public List<Company> GetCompanies();
        public List<Student> GetStudentsInOffer(int offerId);
        public List<Knowledge> GetStudentKnowledge(int userId);
        public CVFile GetStudentCv(int studentId);
    }
}
