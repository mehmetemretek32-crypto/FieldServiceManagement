using FSM.Application.DTOs;

namespace FSM.Application.Services; 

public interface ICustomerService
{
    // 1. Tüm aktif müşterileri listele
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();

    // 2. ID'ye göre tek bir müşteriyi getir
    Task<CustomerDto> GetCustomerByIdAsync(int id);

    // 3. Yeni bir müşteri ekle (Geriye eklenen müşterinin ID'sini dönsün)
    Task<int> CreateCustomerAsync(CreateCustomerDto createCustomerDto);

    // 4. Mevcut bir müşteriyi güncelle (ID ve yeni bilgileri alarak)
    Task UpdateCustomerAsync(int id, CreateCustomerDto updateCustomerDto);

    // 5. Müşteriyi sistemden sil (Geçen hafta yaptığımız Soft Delete mantığıyla)
    Task DeleteCustomerAsync(int id);
}