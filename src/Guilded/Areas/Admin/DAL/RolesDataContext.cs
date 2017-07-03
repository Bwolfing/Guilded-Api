using Guilded.DAL.Abstract;
using Guilded.Data;
using Guilded.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Guilded.Areas.Admin.DAL
{
    public class RolesDataContext : ReadWriteDataContextBase, IRolesDataContext
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesDataContext(ApplicationDbContext context,
            RoleManager<ApplicationRole> roleManager) : base(context)
        {
            _roleManager = roleManager;
        }

        public IQueryable<ApplicationRole> GetRoles()
        {
            return _roleManager.Roles;
        }

        public Task<ApplicationRole> GetRoleByIdAsync(string id)
        {
            return _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<ApplicationRole> CreateRoleAsync(ApplicationRole roleToCreate)
        {
            var result = await _roleManager.CreateAsync(roleToCreate);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create role '{roleToCreate.Name}'");
            }
            return await GetRoleByIdAsync(roleToCreate.Id);
        }

        public async Task<ApplicationRole> UpdateRoleAsync(ApplicationRole roleToUpdate)
        {
            var result = await _roleManager.UpdateAsync(roleToUpdate);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to update role '{roleToUpdate.Name}': " +
                                    $"{string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            return await GetRoleByIdAsync(roleToUpdate.Id);
        }

        public async Task<IdentityResult> DeleteRole(ApplicationRole roleToDelete)
        {
            return await _roleManager.DeleteAsync(roleToDelete);
        }
    }
}