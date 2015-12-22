using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBSC.ViewModel.Login
{
    public class LoginViewModel
    {
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Senha { get; set; }
    }
}
