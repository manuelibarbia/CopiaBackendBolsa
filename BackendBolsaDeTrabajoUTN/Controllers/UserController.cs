using Microsoft.AspNetCore.Mvc;
using BackendBolsaDeTrabajoUTN.Models;
using BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces;
using System.Text;
using System.Configuration;
using BackendBolsaDeTrabajoUTN.DBContexts;
using SendGrid.Helpers.Mail;
using SendGrid;
using DotNetEnv;
using BackendBolsaDeTrabajoUTN.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace BackendBolsaDeTrabajoUTN.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly TPContext _context;

        public UserController(IUserRepository userRepository, TPContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        [Authorize]
        [HttpPut]
        [Route("changePassword/{userId}")]
        public IActionResult ChangePassword(int userId, AddPasswordRequest request)
        {
            var user = _userRepository.GetUserById(userId);
            if (user != null)
            {
                try
                {
                    var hashedCurrentPassword = Security.CreateSHA512(request.CurrentPassword);
                    if (hashedCurrentPassword != user.Password)
                    {
                        throw new Exception("La contraseña actual ingresada no es correcta");
                    }
                    ValidatePassword(user.Password, request.NewPassword, request.ConfirmNewPassword, user.UserName);
                    _userRepository.SaveNewPassword(userId, request.NewPassword);
                    return Ok(new { message = "Contraseña cambiada exitosamente" });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return NotFound("Usuario no econtrado");
            }
        }

        [HttpPost]
        [Route("recoverPassword")]
        public IActionResult RecoverPassword(PasswordRequestBody passwordRequestBody)
        {
            var user = _userRepository.RecoverPassword(passwordRequestBody);

            if (user == null)
            {
                return NotFound();
            }

            string newPassword = GenerateRandomPassword();

            user.Password = newPassword;
            _context.SaveChanges();
            
            SendEmail(user.UserEmail, "Recuperación de contraseña", $"Hola {user.UserName}. Tu nueva contraseña es: {newPassword}");

            return Ok("Cambio correcto, revisa tu correo principal");
        }


        [NonAction]
        private string GenerateRandomPassword()
        {
            string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            Random random = new Random();
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < 8; i++) // Genera una contraseña de longitud 8
            {
                int randomIndex = random.Next(validChars.Length);
                password.Append(validChars[randomIndex]);
            }

            return password.ToString();
        }

        private void SendEmail(string destinatario, string asunto, string contenido)
        {
            // Env.Load(); // Cargar variables de entorno desde el archivo .env

            string apiKey = "hola"; //Env.GetString("API_KEY");

            var client = new SendGridClient(apiKey);


            var from = new EmailAddress("luciano3924@gmail.com", "Bolsa de Trabajo");
            var to = new EmailAddress(destinatario);
            var msg = MailHelper.CreateSingleEmail(from, to, asunto, contenido, contenido);
            try
            {
                var response = client.SendEmailAsync(msg).GetAwaiter().GetResult();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo electrónico: {ex.Message}");
            }
        }

        [NonAction]
        public void ValidatePassword(string currentPassword, string newPassword, string confirmPassword, string userName)
        {
            try
            {
                if (newPassword.Length < 8)
                {
                    throw new Exception("Contraseña insegura, debe tener al menos 8 caracteres");
                }

                bool hasUpperCase = newPassword.Any(char.IsUpper);
                bool hasLowerCase = newPassword.Any(char.IsLower);
                bool hasDigit = newPassword.Any(char.IsDigit);
                bool hasSpecialChar = newPassword.Any(c => !char.IsLetterOrDigit(c));

                if (!hasUpperCase || !hasLowerCase || !hasDigit || !hasSpecialChar)
                {
                    throw new Exception("Contraseña insegura, debe contener al menos una letra mayúscula, una minúscula, un número y un caracter especial");
                }

                if (newPassword.ToLower() == userName.ToLower())
                {
                    throw new Exception("Contraseña insegura, no puede ser igual al nombre de usuario");
                }

                var hashedNewPassword = Security.CreateSHA512(newPassword);
                if (hashedNewPassword == currentPassword)
                {
                    throw new Exception("Contraseña insegura, no puede ser igual a la actual");
                }

                if (newPassword != confirmPassword)
                {
                    throw new Exception("Los campos introducidos de contraseña no coinciden");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}