using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.ServiceGroup;

namespace MockDoor.Shared.Models.Tenant
{
    public class BaseTenantDto : TenantBase, ICopyTo<BaseTenantDto>
    {
        public List<BaseServiceGroupDto> RegisteredServiceGroups { get; set; }

        public bool IsValid(IDictionary<object, object> validationDictionary = null)
        {
            return GeneralHelper.TryValidateFullObject(this, new ValidationContext(this, null, validationDictionary), null);
        }

        public BaseTenantDto CopyTo(BaseTenantDto target)
        {
            if (target == null)
                throw new NotSupportedException($"{nameof(BaseTenantDto)}: Cannot copy to a null target");

            target.Id = Id;
            target.Name = Name;
            target.Path = Path;
            target.SimulateTime = SimulateTime;

            return target;
        }
    }
}
