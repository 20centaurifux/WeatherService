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
    public class UserStore<T> : IUserPasswordStore<T> where T : User
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
    }
}