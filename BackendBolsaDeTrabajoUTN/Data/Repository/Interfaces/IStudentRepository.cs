using BackendBolsaDeTrabajoUTN.Entities;
using BackendBolsaDeTrabajoUTN.Models;

namespace BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces
{
    public interface IStudentRepository
    {
        public ICollection<Offer> GetOffers(int id);
        public void CreateStudent(Student newStudent);
        public void AddStudentAdressInfo(int id, AddStudentAdressInfoRequest newStudentAdressInfo);
        public void AddStudentUniversityInfo(int id, AddStudentUniversityInfoRequest newStudentUniversityInfo);
        public void UploadStudentCV(int studentId, IFormFile file);
        public void RemoveStudent(int id);
        public CVFile GetStudentCv(int studentId);
        public bool CheckCVExists(int studentId);
        public List<User> GetUsers();
        public List<Student> GetStudents();
        public Student GetStudentById(int file);
    }
}