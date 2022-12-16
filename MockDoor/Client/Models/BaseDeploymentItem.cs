namespace MockDoor.Client.Models;

internal class BaseDeploymentItem
{
    public string Description { get; set; }

    public string FailureMessage { get; set; }

    public DeploymentStatus DeploymentStatus { get; set; }
}