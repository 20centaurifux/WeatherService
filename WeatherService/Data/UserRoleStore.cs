using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeatherService.Models;
using LinqToDB;

namespace WeatherService.Data
{
    public class UserRoleStore<T> : IRoleStore<T> where T : UserRole
    {
        public Task<IdentityResult> CreateAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                if(db.Insert(role) > 0)
                {
                    return Success();
                }

                return Fail();
            }
        }

        public Task<IdentityResult> DeleteAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                db.UserRole.Where(r => r.Id == role.Id).Delete();

                return Success();
            }
        }

        public void Dispose() { }

        public Task<T> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                return (Task<T>)Convert.ChangeType(db.UserRole.FirstOrDefaultAsync(r => r.Id == roleId), typeof(Task<T>));
            }
        }

        public Task<T> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                return (Task<T>)Convert.ChangeType(db.UserRole.FirstOrDefaultAsync(r => r.NormalizedName.Equals(normalizedRoleName)), typeof(Task<T>));
            }
        }

        public Task<string> GetNormalizedRoleNameAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult<string>(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult<string>(role.Id);
        }

        public Task<string> GetRoleNameAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return Task.FromResult<string>(role.Name);
        }

        public Task SetNormalizedRoleNameAsync(T role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            role.NormalizedName = normalizedName;

            return Task.FromResult<object>(null);
        }

        public Task SetRoleNameAsync(T role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            role.Name = roleName;

            return Task.FromResult<object>(null);
        }

        public Task<IdentityResult> UpdateAsync(T role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var db = new WeatherDb())
            {
                if(db.Update(role) > 0)
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
    }
}