namespace FSM.Domain.Common;

// Kalıcı silme yerine pasife çekme (Soft Delete) destekleyen entity'ler bu arayüzü uygular.
public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}
