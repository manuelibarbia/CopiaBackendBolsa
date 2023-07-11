using BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces;
using BackendBolsaDeTrabajoUTN.DBContexts;
using BackendBolsaDeTrabajoUTN.Entities;
using BackendBolsaDeTrabajoUTN.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendBolsaDeTrabajoUTN.Data.Repository.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly TPContext _context;
        public StudentRepository(TPContext context)
        {
            _context = context;
        }

        public ICollection<Offer> GetOffers(int id)
        {
            return _context.Students.Include(a => a.Offers).Where(a => a.UserId == id).Select(a => a.Offers).FirstOrDefault() ?? new List<Offer>();
        }

        public void CreateStudent(Student newStudent)
        {
            try
            {
                _context.Students.Add(newStudent);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {


                throw new Exception("el error es" + ex);
            }
        }

        public void AddStudentAdressInfo(int id, AddStudentAdressInfoRequest newStudentAdressInfo)
        {
            try
            {
                _context.Students.First(s => s.UserId == id).FamilyStreet = newStudentAdressInfo.FamilyStreet;
                _context.Students.First(s => s.UserId == id).FamilyStreetNumber = newStudentAdressInfo.FamilyStreetNumber;
                _context.Students.First(s => s.UserId == id).FamilyStreetLetter = newStudentAdressInfo.FamilyStreetLetter;
                _context.Students.First(s => s.UserId == id).FamilyFloor = newStudentAdressInfo.FamilyFloor;
                _context.Students.First(s => s.UserId == id).FamilyDepartment = newStudentAdressInfo.FamilyDepartment;
                _context.Students.First(s => s.UserId == id).FamilyCountry = newStudentAdressInfo.FamilyCountry;
                _context.Students.First(s => s.UserId == id).FamilyProvince = newStudentAdressInfo.FamilyProvince;
                _context.Students.First(s => s.UserId == id).FamilyLocation = newStudentAdressInfo.FamilyLocation;
                _context.Students.First(s => s.UserId == id).FamilyPersonalPhone = newStudentAdressInfo.FamilyPersonalPhone;
                _context.Students.First(s => s.UserId == id).FamilyOtherPhone = newStudentAdressInfo.FamilyOtherPhone;

                _context.Students.First(s => s.UserId == id).PersonalStreet = newStudentAdressInfo.PersonalStreet;
                _context.Students.First(s => s.UserId == id).PersonalStreetNumber = newStudentAdressInfo.PersonalStreetNumber;
                _context.Students.First(s => s.UserId == id).PersonalStreetLetter = newStudentAdressInfo.PersonalStreetLetter;
                _context.Students.First(s => s.UserId == id).PersonalFloor = newStudentAdressInfo.PersonalFloor;
                _context.Students.First(s => s.UserId == id).PersonalDepartment = newStudentAdressInfo.PersonalDepartment;
                _context.Students.First(s => s.UserId == id).PersonalCountry = newStudentAdressInfo.PersonalCountry;
                _context.Students.First(s => s.UserId == id).PersonalProvince = newStudentAdressInfo.PersonalProvince;
                _context.Students.First(s => s.UserId == id).PersonalLocation = newStudentAdressInfo.PersonalLocation;
                _context.Students.First(s => s.UserId == id).PersonalPersonalPhone = newStudentAdressInfo.PersonalPersonalPhone;
                _context.Students.First(s => s.UserId == id).PersonalOtherPhone = newStudentAdressInfo.PersonalOtherPhone;

                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("Estudiante no encontrado o parámetros no válidos");
            }
        }

        public void AddStudentUniversityInfo(int id, AddStudentUniversityInfoRequest newStudentUniversityInfo)
        {
            try
            {
                var student = _context.Students.FirstOrDefault(s => s.UserId == id);

                if (student != null)
                {
                    student.Specialty = newStudentUniversityInfo.Specialty;
                    student.ApprovedSubjectsQuantity = newStudentUniversityInfo.ApprovedSubjectsQuantity;
                    student.SpecialtyPlan = newStudentUniversityInfo.SpecialtyPlan;
                    student.CurrentStudyYear = newStudentUniversityInfo.CurrentStudyYear;
                    student.StudyTurn = newStudentUniversityInfo.StudyTurn;
                    student.AverageMarksWithPostponement = newStudentUniversityInfo.AverageMarksWithPostponement;
                    student.AverageMarksWithoutPostponement = newStudentUniversityInfo.AverageMarksWithoutPostponement;
                    student.CollegeDegree = newStudentUniversityInfo.CollegeDegree;

                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Estudiante no encontrado");
                }
            }
            catch
            {
                throw new Exception("Parámetros no válidos");
            }
        }

        public void UploadStudentCV(int studentId, IFormFile file)
        {
            try
            {
                    byte[] fileBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }

                    CVFile newCvFile = new CVFile
                    {
                        Name = file.FileName,
                        File = fileBytes,
                        StudentId = studentId,
                        Student = _context.Students.FirstOrDefault(s => s.UserId == studentId),
                        CVPendingConfirmation = true,
                        CVIsActive = true,
                    };

                var existentCvFile = _context.CVFiles.FirstOrDefault(cv => cv.StudentId == newCvFile.StudentId);
                if (existentCvFile != null)
                {
                    existentCvFile.CVId = existentCvFile.CVId;
                    existentCvFile.Name = newCvFile.Name;
                    existentCvFile.File = newCvFile.File;
                    existentCvFile.Student = newCvFile.Student;
                    existentCvFile.CVPendingConfirmation = true;
                    existentCvFile.CVIsActive = true;
                }
                else
                {
                    _context.CVFiles.Add(newCvFile);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void RemoveStudent(int id)
        {
            try
            {
                var student = _context.Students.FirstOrDefault(s => s.UserId == id);
                if (student == null)
                {
                    throw new Exception("Estudiante no encontrado");
                }
                student.UserIsActive = false;
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("Usuario no encontrado");
            }
        }

        public CVFile GetStudentCv(int studentId)
        {
            try
            {
                CVFile cVFile = _context.CVFiles.FirstOrDefault(cv => cv.StudentId == studentId && cv.CVIsActive == true);
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

        public bool CheckCVIsRejected(int studentId)
        {
            var cvFile = _context.CVFiles.FirstOrDefault(cv => cv.StudentId == studentId && cv.CVIsActive == false && cv.CVPendingConfirmation == true);
            if (cvFile == null)
            {
                return false;
            }
            return true;
        }

        public bool CheckCVExists(int studentId)
        {
            var cvFile = _context.CVFiles.FirstOrDefault(cv => cv.StudentId == studentId && cv.CVIsActive == true);
            if (cvFile == null)
            {
                return false;
            }
            return true;
        }

        public bool CheckCVIsAccepted(int studentId)
        {
            var cvFile = _context.CVFiles.FirstOrDefault(cv => cv.StudentId == studentId && cv.CVIsActive == true && cv.CVPendingConfirmation == false);
            if (cvFile == null)
            {
                return false;
            }
            return true;
        }

        public List<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public List<Student> GetStudents()
        {
            return _context.Students.ToList();
        }

        public Student GetStudentById(int id)
        {
            try
            {
                return _context.Students.FirstOrDefault(s => s.UserId == id && s.UserIsActive == true);
            }
            catch (Exception ex)
            {
                throw new Exception (ex.Message);
            }
        }
    }
}