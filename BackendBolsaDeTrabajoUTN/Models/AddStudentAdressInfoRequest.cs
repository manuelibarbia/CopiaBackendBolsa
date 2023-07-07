﻿namespace BackendBolsaDeTrabajoUTN.Models
{
    public class AddStudentAdressInfoRequest
    {
        //// Domicilio familiar

        public string FamilyStreet { get; set; }
        public int FamilyStreetNumber { get; set; }
        public string FamilyStreetLetter { get; set; }
        public int FamilyFloor { get; set; }
        public string FamilyDepartment { get; set; }
        public string FamilyCountry { get; set; }
        public string FamilyProvince { get; set; }
        public string FamilyLocation { get; set; }
        public long FamilyPersonalPhone { get; set; }
        public long FamilyOtherPhone { get; set; }

        // Domicilio personal

        public string PersonalStreet { get; set; }
        public int PersonalStreetNumber { get; set; }
        public string PersonalStreetLetter { get; set; }
        public int PersonalFloor { get; set; }
        public string PersonalDepartment { get; set; }
        public string PersonalCountry { get; set; }
        public string PersonalProvince { get; set; }
        public string PersonalLocation { get; set; }
        public long PersonalPersonalPhone { get; set; }
        public long PersonalOtherPhone { get; set; }
    }
}
