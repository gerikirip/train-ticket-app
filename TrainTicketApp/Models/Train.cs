using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace TrainTicketApp.Models
{
    public partial class Train
    {
        public Train()
        {
            TrainTimes = new HashSet<TrainTime>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Indulási hely kötelező!")]
        [StringLength(50, ErrorMessage = "Túl hosszú!")]
        [RegularExpression(@"^[A-zÀ-úŐőÜüŰű]+$", ErrorMessage = "Csak betűket használhat")]
        public string OriginStation { get; set; }

        [Required(ErrorMessage = "Érkezési hely kötelező!")]
        [StringLength(50, ErrorMessage = "Túl hosszú!")]
        [RegularExpression(@"^[A-zÀ-úŐőÜüŰű]+$", ErrorMessage = "Csak betűket használhat")]
        public string DestStation { get; set; }

        [Required(ErrorMessage = "Menetidő kötelező!")]
        [DataType(DataType.Time)]
        public TimeSpan TravelTime { get; set; }

        [Required(ErrorMessage = "Távolság kötelező!")]
        [Range(0,10000, ErrorMessage = "Pozitív távolságot adjon meg")]
        public int Distance { get; set; }

        public virtual ICollection<TrainTime> TrainTimes { get; set; }
    }
}
