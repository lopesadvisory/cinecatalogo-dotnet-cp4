namespace CineCatalogo.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public static NotFoundException For(string entity, object id) =>
        new($"{entity} com id '{id}' não foi encontrado.");
}
