using ServiceInterface.Model;
using System.Collections.Generic;

namespace ServiceInterface.Interface
{
    public interface ICustomerService
    {
        (int, IEnumerable<Customer>) GetAllCustomers();
        int SaveCustomer(Customer customer, out IEnumerable<string> errors);
        int UpdateCustomer(Customer customer);
        int SaveCustomers(IEnumerable<Customer> customers);
        Customer GetCustomer(long id);
        int DeleteCustomer(long additiveId);
    }
}
