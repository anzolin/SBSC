//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SBSC.Model.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ItemEmprestimo
    {
        public int Id { get; set; }
        public int Id_Emprestimo { get; set; }
        public int Id_Livro { get; set; }
        public System.DateTime DtDevolucao { get; set; }
        public System.DateTime DtHrCadastro { get; set; }
    
        public virtual Emprestimo Emprestimo { get; set; }
        public virtual Livro Livro { get; set; }
    }
}