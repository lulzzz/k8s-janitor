﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using K8sJanitor.WebApi.Models;
using K8sJanitor.WebApi.Services;
using K8sJanitor.WebApi.Validators;
using Serilog;

namespace K8sJanitor.WebApi.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IAddRoleRequestValidator _addRoleRequestValidator;
        private readonly IConfigMapService _configMapService;
        public RoleController(
            IAddRoleRequestValidator addRoleRequestValidator, 
            IConfigMapService configMapService
        )
        {
            _addRoleRequestValidator = addRoleRequestValidator;
            _configMapService = configMapService;
        }

        [HttpPost("")]
        public async Task<ActionResult<string>> AddRole([FromBody]AddRoleRequest addRoleRequest)
        {
            if (!_addRoleRequestValidator.TryValidateAddRoleRequest(addRoleRequest, out string validationError))
            {
                Log.Warning($"Create role called with invalid input. Validation error: {validationError}");
                return BadRequest(validationError);
            }

            var updatedMapRolesYaml = string.Empty;

            try
            {
                await _configMapService.AddRole(
                    addRoleRequest.RoleName,
                    addRoleRequest.RoleArn
                );
            }
            catch (Exception ex)
            {
                Log.Error($"An error occured trying to create the role mapping: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occured trying to create the role mapping: {ex.Message}");
            }

            return Ok(updatedMapRolesYaml);
        }
    }
}
