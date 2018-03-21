using LinqToDB.Mapping;

namespace WeatherService.Models
{
    [Table(Name = "AspnetUserInRole")]
    public class UserInRole
    {
        [PrimaryKey]
        [Column(Name = "UserId")]
        public string UserId { get; set; }

        [PrimaryKey]
        [Column(Name = "RoleId")]
        public string RoleId { get; set; }

        [Association(ThisKey = "UserId", OtherKey = "Id")]
        public User User;

        [Association(ThisKey = "RoleId", OtherKey = "Id")]
        public UserRole Role;
    }
}