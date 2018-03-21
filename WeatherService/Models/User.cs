using Microsoft.AspNetCore.Identity;
using System;
using LinqToDB.Mapping;
using System.Collections.Generic;

namespace WeatherService.Models
{
    [Table(Name = "AspnetUser")]
    public class User : IdentityUser
    {
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Column(Name = "AccessFailedCount")]
        public override int AccessFailedCount { get => base.AccessFailedCount; set => base.AccessFailedCount = value; }

        [Column(Name = "ConcurrencyStamp")]
        public override string ConcurrencyStamp { get => base.ConcurrencyStamp; set => base.ConcurrencyStamp = value; }

        [Column(Name = "Email")]
        public override string Email { get => base.Email; set => base.Email = value; }

        [Column(Name = "EmailConfirmed")]
        public override bool EmailConfirmed { get => base.EmailConfirmed; set => base.EmailConfirmed = value; }

        [PrimaryKey]
        [Column(Name = "Id")]
        public override string Id { get => base.Id; set => base.Id = value; }

        [Column(Name = "LockoutEnabled")]
        public override bool LockoutEnabled { get => base.LockoutEnabled; set => base.LockoutEnabled = value; }

        //[Column(Name = "LockoutEnd")]
        //public override DateTimeOffset? LockoutEnd { get => base.LockoutEnd; set => base.LockoutEnd = value; }

        [Column(Name = "NormalizedEmail")]
        public override string NormalizedEmail { get => base.NormalizedEmail; set => base.NormalizedEmail = value; }

        [Column(Name = "NormalizedUserName")]
        public override string NormalizedUserName { get => base.NormalizedUserName; set => base.NormalizedUserName = value; }

        [Column(Name = "PasswordHash")]
        public override string PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }

        [Column(Name = "PhoneNumber")]
        public override string PhoneNumber { get => base.PhoneNumber; set => base.PhoneNumber = value; }

        [Column(Name = "PhoneNumberConfirmed")]
        public override bool PhoneNumberConfirmed { get => base.PhoneNumberConfirmed; set => base.PhoneNumberConfirmed = value; }

        [Column(Name = "SecurityStamp")]
        public override string SecurityStamp { get => base.SecurityStamp; set => base.SecurityStamp = value; }

        [Column(Name = "TwoFactorEnabled")]
        public override bool TwoFactorEnabled { get => base.TwoFactorEnabled; set => base.TwoFactorEnabled = value; }

        [Column(Name = "UserName")]
        public override string UserName { get => base.UserName; set => base.UserName = value; }
    }
}