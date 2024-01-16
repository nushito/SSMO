namespace SSMO.Repository
{
    public interface ICustomerOrderRepository
    {
        int? GetCustomerOrderNumberById(int? id);

        string GetCustomerPoNumberById(int? id);    
    }
}
