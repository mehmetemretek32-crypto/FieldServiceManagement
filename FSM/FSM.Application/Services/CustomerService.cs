using FSM.Application.DTOs;
using FSM.Domain.Entities;
using FSM.Domain.Interfaces;
// Kendi projendeki Repository namespace'ini buraya eklemeyi unutma (örn: using FSM.Infrastructure.Repositories;)

namespace FSM.Application.Services;

public class CustomerService : ICustomerService
{
    // Veritabanı işlemleri için Repository bağımlılığını (Dependency Injection) alıyoruz
    private readonly IGenericRepository<Customer> _customerRepository;

    public CustomerService(IGenericRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _customerRepository.GetAllAsync();

        // Sadece silinmemiş (aktif) müşterileri filtreleyip DTO'ya çeviriyoruz
        return customers
            .Where(c => !c.IsDeleted)
            .Select(c => new CustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                CompanyName = c.CompanyName,
                PhoneNumber = c.PhoneNumber,
                Email = c.Email,
                Address = c.Address
            }).ToList();
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null || customer.IsDeleted)
        {
            throw new Exception($"ID'si {id} olan aktif bir müşteri bulunamadı.");
        }

        return new CustomerDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            CompanyName = customer.CompanyName,
            PhoneNumber = customer.PhoneNumber,
            Email = customer.Email,
            Address = customer.Address
        };
    }

    public async Task<int> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
    {
        var newCustomer = new Customer
        {
            FirstName = createCustomerDto.FirstName,
            LastName = createCustomerDto.LastName,
            CompanyName = createCustomerDto.CompanyName,
            PhoneNumber = createCustomerDto.PhoneNumber,
            Email = createCustomerDto.Email,
            Address = createCustomerDto.Address,
            IsDeleted = false
        };

        await _customerRepository.AddAsync(newCustomer);
        return newCustomer.Id;
    }

    public async Task UpdateCustomerAsync(int id, CreateCustomerDto updateCustomerDto)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null || customer.IsDeleted)
        {
            throw new Exception($"Güncellenmek istenen müşteri bulunamadı (ID: {id}).");
        }

        // Yeni bilgileri var olan entity'nin üzerine yazıyoruz
        customer.FirstName = updateCustomerDto.FirstName;
        customer.LastName = updateCustomerDto.LastName;
        customer.CompanyName = updateCustomerDto.CompanyName;
        customer.PhoneNumber = updateCustomerDto.PhoneNumber;
        customer.Email = updateCustomerDto.Email;
        customer.Address = updateCustomerDto.Address;

        await _customerRepository.UpdateAsync(customer);
    }

    public async Task DeleteCustomerAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);

        if (customer == null || customer.IsDeleted)
        {
            throw new Exception($"Silinmek istenen müşteri zaten yok veya daha önceden silinmiş (ID: {id}).");
        }

        // Soft Delete: Fiziksel olarak silmiyoruz, sadece bayrağı indiriyoruz
        customer.IsDeleted = true;
        await _customerRepository.UpdateAsync(customer);
    }
}