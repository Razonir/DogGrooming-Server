using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogGrooming_Server.Data.Models
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public DateTime RegisterDate { get; set; }


        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public User? User { get; set; }
    }
}
