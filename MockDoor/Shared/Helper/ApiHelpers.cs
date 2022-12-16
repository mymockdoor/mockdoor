using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MockDoor.Shared.Models.Utility;

namespace MockDoor.Shared.Helper
{
    public static class ApiHelpers
    {
        public static BadRequestResultDto ToBadRequestResult(this List<ValidationResult> validationResults, string title = "Bad Request due to the following errors")
        {
            var result = new BadRequestResultDto()
            {
                Status = 400,
                Title = title,
                TraceId = Guid.NewGuid().ToString(),
                Type = "ValidationResult"
            };

            var errors = new Dictionary<string, List<string>>();

            foreach (var vr in validationResults)
            {
                if (!string.IsNullOrWhiteSpace(vr.ErrorMessage))
                {
                    foreach (var member in vr.MemberNames)
                    {

                        if (errors.ContainsKey(member))
                        {
                            errors[member].Add(vr.ErrorMessage);
                        }
                        else
                        {
                            errors.Add(member, new List<string>() { vr.ErrorMessage });
                        }
                    }
                }
            }

            result.Errors = errors;

            return result;
        }
    }
}
