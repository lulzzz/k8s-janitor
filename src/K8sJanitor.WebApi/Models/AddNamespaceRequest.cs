namespace K8sJanitor.WebApi.Models
{
    public class AddNamespaceRequest
    {
        public string NamespaceName { get; set; }
        public string RoleName { get; set; }
    }
}