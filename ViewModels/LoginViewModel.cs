using System.ComponentModel.DataAnnotations;

namespace csbb0328.ViewModels{
    public class LoginViewModel{
        [Required(ErrorMessage = "You must include an email address.")]
        public string email {get; set;}
        [RequiredAttribute(ErrorMessage = "Please include a password.")]
        [DataTypeAttribute(DataType.Password)]
        public string password {get; set;}
    }
}