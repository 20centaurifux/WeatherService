using Microsoft.AspNetCore.Identity;
using System;
using LinqToDB.Mapping;

namespace WeatherService.Models
{
    [Table(Name = "AspnetRole")]
    public class UserRole : IdentityRole
    {
        public UserRole() => Id = Guid.NewGuid().ToString();

        [Column(Name = "ConcurrencyStamp")]
        public override string ConcurrencyStamp { get => base.ConcurrencyStamp; set => base.ConcurrencyStamp = value; }

        [PrimaryKey]
        [Column(Name = "Id")]
        public override string Id { get => base.Id; set => base.Id = value; }

        [Column(Name = "Name")]
        public override string Name { get => base.Name; set => base.Name = value; }

        [Column(Name = "NormalizedName")]
        public override string NormalizedName { get => base.NormalizedName; set => base.NormalizedName = value; }
    }
}