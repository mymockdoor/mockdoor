using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Tenant;

namespace MockDoor.Shared.Models.Utility;

public class FullDatabaseDto : IValidatableObject
{
    public IEnumerable<FullTenantDto> Tenants { get; set; }

    public IEnumerable<string> AppliedMigrations { get; set; }
    
    public string DatabaseType { get; set; }

    public string CodeVersion { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var groupResult in ValidateTenants(Tenants?.ToList(), validationContext)) yield return groupResult;
    }
    

    private IEnumerable<ValidationResult> ValidateTenants(List<FullTenantDto> tenants, ValidationContext validationContext)
    {
        if (tenants?.Count > 0)
        {
            foreach (var tenant in tenants)
            {
                var validationResults = GeneralHelper.ValidateFullObject(tenant, new ValidationContext(tenant, null, validationContext.Items));
                
                foreach (var validationResult in validationResults)
                {
                    yield return validationResult;
                }
            }
        }
    }
    
}