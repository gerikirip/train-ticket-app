using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace TrainTicketApp.Models
{
    public partial class Owner
    {
        public Owner()
        {
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Keresztnév kötelező!")]
        [StringLength(50, ErrorMessage = "Túl hosszú!")]
        [RegularExpression(@"^[A-zÀ-úŐőÜüŰű]+$", ErrorMessage = "Csak betűket használhat")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Vezetéknév kötelező!")]
        [StringLength(50, ErrorMessage = "Túl hosszú!")]
        [RegularExpression(@"^[A-zÀ-úŐőÜüŰű]+$", ErrorMessage = "Csak betűket használhat")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email kötelező!")]
        [EmailAddress(ErrorMessage = "Nem jó a formátum!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lakcím kötelező!")]
        [StringLength(100, ErrorMessage = "Túl hosszú!")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Telefonszám kötelező!")]
        [Phone(ErrorMessage = "Nem jó a formátum!")]
        [StringLength(12, ErrorMessage = "Túl hosszú!")]
        public string PhoneNumber { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
