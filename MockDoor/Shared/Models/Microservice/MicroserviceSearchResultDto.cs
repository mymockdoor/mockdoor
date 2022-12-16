namespace MockDoor.Shared.Models.Microservice;

public class MicroserviceSearchResultDto : MicroserviceResultDto
{
    public int TenantId { get; set; }
    
    public string TenantName { get; set; }

    public int ServiceGroupId { get; set; }
    
    public string ServiceGroupName { get; set; }

    public int TotalRequests { get; set; }
}