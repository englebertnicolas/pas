namespace PAS.Common.Application;

public class NotFoundException : Exception {

    public NotFoundException()
        : base($"Entity was not found.") {
    }

    public NotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' ('{key}') was not found.") {
    }
}