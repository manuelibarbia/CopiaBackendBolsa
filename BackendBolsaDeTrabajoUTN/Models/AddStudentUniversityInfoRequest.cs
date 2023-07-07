namespace BackendBolsaDeTrabajoUTN.Models
{
    public class AddStudentUniversityInfoRequest
    {
        //// Domicilio familiar

        public string Specialty { get; set; }
        public int ApprovedSubjectsQuantity { get; set; }
        public int SpecialtyPlan { get; set; }
        public int CurrentStudyYear { get; set; }
        public string StudyTurn { get; set; }
        public decimal AverageMarksWithPostponement { get; set; }
        public decimal AverageMarksWithoutPostponement { get; set; }
        public string CollegeDegree { get; set; }
    }
}
