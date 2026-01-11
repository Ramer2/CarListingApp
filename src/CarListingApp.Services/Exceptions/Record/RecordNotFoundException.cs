namespace CarListingApp.Services.Exceptions.Record;

public class RecordNotFoundException : Exception
{
    public RecordNotFoundException()
    {
    }

    public RecordNotFoundException(string? message) : base(message)
    {
    }
}