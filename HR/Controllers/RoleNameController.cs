﻿using HR.DTO;
using HR.Models;
using HR.Repository;
using HR.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "Admin")]
    public class RoleNameController : ControllerBase
    {
        public RoleManager<IdentityRole> roleManager { get; }
        public readonly HRDbcontext db;
        public readonly IRoleNameRepository _roleNameRepository;

        public RoleNameController(RoleManager<IdentityRole> roleManager, HRDbcontext _dbl, IRoleNameRepository _roleNameRepository)
        {
            this.roleManager = roleManager;
            this.db = _dbl;
            this._roleNameRepository = _roleNameRepository;
        }
        [HttpGet("GetAllRoles")]
      //[Authorize(Roles = "Permission.View")]
        public async Task<IActionResult> GetAllRoles()
        {

            List<RoleName> rolesWithPermissions = await _roleNameRepository.GetAllRoles();
                return Ok(rolesWithPermissions);
            
        }
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRoleName(RoleDTO roleDto)
        {
            if (ModelState.IsValid)
            { 
                RoleName roleName = new RoleName
                {
                    GroupName = roleDto.Name,
                    Permissions = new List<permission>(),
                };

                  ////
                    foreach (var permissionDto in roleDto.Permissions)
                    {
                        // Convert PermissionDTO to Permission entity
                        permission perm = new permission
                        {
                            name = permissionDto.name,
                            create = permissionDto.Create ?? false,
                            delete = permissionDto.Delete ?? false,
                            view = permissionDto.View ?? false,
                            update = permissionDto.Update ?? false
                        };


                        roleName.Permissions.Add(perm);
                    

                }
                RoleName createdRole = await _roleNameRepository.RoleNameCreate(roleName);
                if (createdRole != null)
                {
                    return Ok(createdRole);
                }
                else
                {
                    return BadRequest("Failed to create role.");
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [HttpGet("GetGroupById/{id:int}")]
        public async Task<IActionResult> GetGroupById(int id)
        {
            var group = await _roleNameRepository.GetRoleNameById(id);
            if (group == null)
            {
                return NotFound();
            }
            return Ok(group);
        }
        [HttpPut("UpdateRole/{id}")]
        public async Task<IActionResult> UpdateRole(int id, RoleDTO roleDto)
        {
            var existingRole = await _roleNameRepository.GetRoleNameById(id);
            if (existingRole == null)
            {
                return NotFound();
            }
            existingRole.GroupName = roleDto.Name;
            if (roleDto.Permissions != null)
            {
                existingRole.Permissions.Clear();

                foreach (var permissionDto in roleDto.Permissions)
                {
                    var permission = new permission
                    {
                        name = permissionDto.name,
                        create = permissionDto.Create,
                        view = permissionDto.View,
                        update = permissionDto.Update,
                        delete = permissionDto.Delete
                    };
                    existingRole.Permissions.Add(permission);
                }
            }

            await _roleNameRepository.RoleNameUpdate(existingRole);

            return NoContent();
        }
        [HttpDelete("DeleteRole/{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var roleToDelete = await _roleNameRepository.GetRoleNameById(id);
            if (roleToDelete == null)
            {
                return NotFound();
            }
            await _roleNameRepository.RoleNameDelete(roleToDelete);
            return Ok();
        }
    //    [HttpGet("{id}")]
    //    public IActionResult GetRoleById(string id) {
    //        var role = db.Roles.FirstOrDefault(x => x.Id==id);
    //        return Ok(role);
    //    }
    //    [HttpDelete("{id}")]
    //    public IActionResult DeleteRoleById(string id)
    //    {
    //        var role = db.Roles.FirstOrDefault(y => y.Id==id);
    //        if(role == null) return NotFound();
    //        db.Roles.Remove(role);
    //        db.SaveChanges();
    //        return Ok(role);
    //    }
   }





    }

