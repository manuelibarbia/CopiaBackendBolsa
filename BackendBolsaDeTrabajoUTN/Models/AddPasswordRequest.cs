namespace BackendBolsaDeTrabajoUTN.Models
{
    public class AddPasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
