using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Interface
{
    public interface ICustomerService
    {
        ICollection<Customer> GetAllCustomers(out int status);
        int SaveCustomer(Customer customer, out long id, out IEnumerable<string> errors);
        int UpdateCustomer(Customer customer);
        int SaveCustomers(IEnumerable<Customer> customers);
        Customer GetCustomer(long id);
        int DeleteCustomer(long idCustomer);
    }
}
