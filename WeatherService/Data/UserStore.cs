using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherService.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using LinqToDB;

namespace WeatherService.Data
{
    public class UserStore<T> : IUserPasswordStore<T>, IUserRoleStore<T> where T : User
    {
        public Task<IdentityResult> CreateAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                if (db.Insert(user) > 0)
                {
                    return Success();
                }

                return Fail();
            }
        }

        public Task<IdentityResult> DeleteAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                db.User.Where(u => u.Id == user.Id).Delete();

                return Success();
            }
        }

        public void Dispose() { }

        public Task<T> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                return (Task<T>)Convert.ChangeType(db.User.FirstOrDefaultAsync(u => u.Id == userId), typeof(Task<T>));
            }
        }

        public Task<T> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                return (Task<T>)Convert.ChangeType(db.User.FirstOrDefaultAsync(u => u.NormalizedUserName.Equals(normalizedUserName)), typeof(Task<T>));
            }
        }

        public Task<string> GetNormalizedUserNameAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult<string>(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult<string>(user.Id);
        }

        public Task<string> GetUserNameAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult<string>(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(T user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.NormalizedUserName = normalizedName;

            return Task.FromResult<object>(null);
        }

        public Task SetUserNameAsync(T user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.UserName = userName;

            return Task.FromResult<object>(null);
        }

        public Task<IdentityResult> UpdateAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                if (db.Update(user) > 0)
                {
                    return Success();
                }

                return Fail();
            }
        }

        private Task<IdentityResult> Success()
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Success);
        }

        private Task<IdentityResult> Fail()
        {
            return Task.FromResult<IdentityResult>(IdentityResult.Failed(null));
        }

        public Task SetPasswordHashAsync(T user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            user.PasswordHash = passwordHash;

            return Task.FromResult<object>(null);
        }

        public Task<string> GetPasswordHashAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult(user.PasswordHash != null);
        }

        public Task AddToRoleAsync(T user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                var roleId = db.UserRole.First(r => r.NormalizedName.Equals(roleName)).Id;

                db.Insert(new UserInRole() { UserId = user.Id, RoleId = roleId });
            }

            return Task.FromResult<object>(null);
        }

        public Task RemoveFromRoleAsync(T user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                var roleId = db.UserRole.First(r => r.NormalizedName.Equals(roleName)).Id;

                db.Delete(new UserInRole() { UserId = user.Id, RoleId = roleId });
            }

            return Task.FromResult<object>(null);
        }

        public Task<IList<string>> GetRolesAsync(T user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                IList<string> l = db.UserInRole.Where(m => m.UserId.Equals(user.Id)).Select(m => m.Role.Name).ToList();

                return Task.FromResult(l);
            }
        }

        public Task<bool> IsInRoleAsync(T user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                return Task.FromResult(db.UserInRole.Count(m => m.UserId.Equals(user.Id) && m.Role.NormalizedName.Equals(roleName)) > 0);
            }
        }

        public Task<IList<T>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                IList<T> l = new List<T>();

                foreach(var u in db.UserInRole.Where(m => m.Role.NormalizedName.Equals(roleName)).Select(m => m.User))
                {
                    l.Add((T)u);
                }

                return Task.FromResult(l);
            }
        }
    }
}