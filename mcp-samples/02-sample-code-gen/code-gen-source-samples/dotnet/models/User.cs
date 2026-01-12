using System;
using System.ComponentModel.DataAnnotations;

namespace YourProject.Models
{
    /// <summary>
    /// Represents a User entity
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
