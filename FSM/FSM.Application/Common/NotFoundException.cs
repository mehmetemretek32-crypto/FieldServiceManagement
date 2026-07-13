namespace FSM.Application.Common;

// Bir kayıt bulunamadığında (veya soft-delete ile pasife çekildiğinde) fırlatılan ortak istisna.
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
