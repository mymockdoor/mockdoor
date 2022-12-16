namespace MockDoor.Shared.Constants;

public static class ErrorMessageConstants
{
    public const string MicroserviceId = "Invalid microservice id";
    
    public const string NewMicroserviceId = "Invalid microservice id (ID must be 0), new Microservice IDs are automatically assigned and cannot be set on creation.";
    
    public const string MicroservicePath = "Invalid microservice path";
    
    public const string MicroserviceName = "Invalid microservice name";
    
    public const string MicroserviceTargetUrl = "Invalid microservice target url";
    
    public const string MicroserviceNotFound = "Microservice not found";
    
    public const string MicroserviceDisabled = "Microservice is disabled";

    public const string MicroserviceAssociatedXFailedToLoad = "Microservice associated {0} failed to load";
    
    public const string ServiceGroupId = "Invalid service group id";
    
    public const string NewServiceGroupId = "Invalid id (ID must be 0), new service group IDs are automatically assigned and cannot be set on creation.";
    
    public const string ServiceGroupNotFound = "Service group not found";
    
    public const string ServiceGroupName = "Invalid service group name";
    
    public const string ServiceGroupPath = "Invalid service group path";
    
    public const string ServiceGroupDisabled = "Service group is disabled";
    
    public const string RequestId = "Invalid request id";
    
    public const string RequestNotFound = "Request not found";
    
    public const string ResponseId = "Invalid response id";
    
    public const string ResponseNotFound = "Response not found";
    
    public const string TenantId = "Invalid tenant id";
    
    public const string TenantPath = "Invalid tenant path";
    
    public const string TenantName = "Invalid tenant name";
    
    public const string TenantNotFound = "Tenant not found";
    
    public const string ScopeId = "Invalid scope id";
    
    // ReSharper disable once InconsistentNaming
    public const string InvalidOrMissingRequestBody = "Invalid or missing body";
    
    public const string IdMissMatch = "Id mismatch";
    
    public const string NewTenantId = "Invalid id (ID must be 0), new tenant IDs are automatically assigned and cannot be set on creation.";
    
    public const string FailedToCreate = "Failed to create";
    
    public const string SimulateTimeModelWasNotProvided = "Simulate time model was not provided";
}