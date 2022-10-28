using AutoMapper;
using AutoMapper.QueryableExtensions;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Products;
using SSMO.Models.Reports.CustomerOrderReportForEdit;
using SSMO.Models.Reports.PaymentsModels;
using SSMO.Models.Reports.PrrobaCascadeDropDown;
using SSMO.Services.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSMO.Services.Reports
{
    public class ReportsService : IReportsService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfigurationProvider mapper;
        private readonly IProductService productService;

        public ReportsService(ApplicationDbContext context, IConfigurationProvider mapper, IProductService productService)
        {
            _context = context;
            this.mapper = mapper;//.ConfigurationProvider;
            this.productService = productService;   
        }
        public IEnumerable<CustomerOrderDetailsModel> AllCustomerOrders(string customerName,
            int currentpage, int customerOrdersPerPage)
        {
            if (String.IsNullOrEmpty(customerName))
            {
               return new List<CustomerOrderDetailsModel>();
            }

            var customerId = _context.Customers.Where(a => a.Name.ToLower() == customerName.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var queryOrders = _context.CustomerOrders
                    .Where(a => a.CustomerId == customerId);
                    
           // var totalOrders = queryOrders.Count();  

            var orders = queryOrders.ProjectTo<CustomerOrderDetailsModel>(this.mapper).ToList();

            var customerOrdersList = orders.Skip((currentpage - 1) * customerOrdersPerPage).Take(customerOrdersPerPage);

            return customerOrdersList;
        }

        public CustomerOrderForEdit CustomerOrderDetailsForEdit(int id)
        {
            var corder = _context.CustomerOrders.Find(id);
            var customerId = _context.CustomerOrders.Where(a => a.Id == id).Select(a=>a.CustomerId).FirstOrDefault();

            var orderForEdit = new CustomerOrderForEdit
            {
                CurrencyId = corder.CurrencyId,
                CustomerPoNumber = corder.CustomerPoNumber,
                CustomerId = customerId,
                Date = corder.Date,
                DeliveryAddress = corder.DeliveryAddress,
                LoadingPlace = corder.LoadingPlace,
                DeliveryTerms = corder.DeliveryTerms,
                FscCertificate = corder.FSCSertificate,
                FscClaim = corder.FSCClaim,
                MyCompanyId = corder.MyCompanyId,
                PaidAmountStatus = corder.PaidAmountStatus,
                Origin = corder.Origin,
                StatusId = corder.StatusId,
                Vat = corder.Vat              
            };
         
            orderForEdit.Products = (List<ProductCustomerFormModel>)productService.DetailsPerCustomerOrder(id);

            return orderForEdit;
        }

        public IEnumerable<CustomerOrderPaymentDetailsModel> CustomersPaymentDetails(string customerName, int currentpage, int customerOrdersPerPage)
        {
            if (String.IsNullOrEmpty(customerName))
            {
                return new List<CustomerOrderPaymentDetailsModel>();
            }

            var customerId = _context.Customers.Where(a => a.Name.ToLower() == customerName.ToLower())
                                .Select(a => a.Id)
                                .FirstOrDefault();

            var customerOrdersId = _context.CustomerOrders.Where(c => c.CustomerId == customerId).Select(i => i.Id).ToList();

            var queryInvoices = _context.Documents
                    .Where(a => customerOrdersId.Contains(a.CustomerOrderId));

            // var totalOrders = queryOrders.Count();  

            var invoices = queryInvoices.ProjectTo<CustomerOrderPaymentDetailsModel>(this.mapper).ToList();

            var customerPaymentList = invoices.Skip((currentpage - 1) * customerOrdersPerPage).Take(customerOrdersPerPage);

            return customerPaymentList;
        }

        public CustomerOrderDetailsModel Details(int id)
        {
            var findorder = _context.CustomerOrders.Where(a => a.Id == id);
            var order = findorder.ProjectTo<CustomerOrderDetailsModel>(mapper).FirstOrDefault();
            var supplierOrderDetail = _context.SupplierOrders
                .Where(o => o.CustomerOrderId == id)
                .Select(a => new
                {
                    supplierNumber = a.Number,
                    supplierId = a.SupplierId
                }).FirstOrDefault();

            var myCompanyId = findorder.
                Select(id=>id.MyCompanyId).
                FirstOrDefault();

            var myCompanyName = _context.MyCompanies
                .Where(id => id.Id == myCompanyId)
                .Select(n => n.Name)
                .FirstOrDefault();

            order.SupplierOrderNumber = supplierOrderDetail.supplierNumber;
            var supplierName = _context.Suppliers
                .Where(id => id.Id == supplierOrderDetail.supplierId)
                .Select(n => n.Name)
                .FirstOrDefault();
            order.SupplierName = supplierName;  
            order.MyCompanyName = myCompanyName;
            return (CustomerOrderDetailsModel)order;
        }

        public bool EditCustomerOrder(int id, 
            string number, System.DateTime date, 
            int myCompanyId, string deliveryTerms,
            string loadingPlace, string deliveryAddress, 
            int currencyId, int status, string fscClaim, 
            string fscCertificate, decimal paidAdvance, bool paidStatus,
             List<ProductCustomerFormModel> products)
        {
            var order = _context.CustomerOrders.Find(id);

            if (order == null)
            {
                return false;
            }

            order.CustomerPoNumber = number;
            order.Date = date;
            order.MyCompanyId = myCompanyId;
            order.DeliveryTerms = deliveryTerms;    
            order.LoadingPlace = loadingPlace;
            order.DeliveryAddress = deliveryAddress;
            order.CurrencyId = currencyId;
            order.StatusId = status;
            order.FSCClaim = fscClaim;
            order.FSCSertificate = fscCertificate;
            order.PaidAvance = paidAdvance;
            order.PaidAmountStatus = paidStatus;
            order.Balance = order.TotalAmount - paidAdvance;

            var productsPerCorder = _context.Products.Where(co => co.CustomerOrderId == order.Id).ToList();

            if (products.Count != 0) 
            {
                for (int i = 0; i < products.Count; i++)
                {
                    var descriptionId = _context.Descriptions.Where(n=>n.Name == products[i].Description).Select(i=>i.Id).FirstOrDefault();
                    productsPerCorder.ElementAt(i).DescriptionId = descriptionId;
                    var gradeId = _context.Grades.Where(n => n.Name == products[i].Grade).Select(i => i.Id).FirstOrDefault();
                    productsPerCorder.ElementAt(i).GradeId = gradeId;
                    var sizeId= _context.Sizes.Where(n => n.Name == products[i].Size).Select(i => i.Id).FirstOrDefault();
                    productsPerCorder.ElementAt(i).SizeId = sizeId;
                    productsPerCorder.ElementAt(i).Price = products.ElementAt(i).Price;
                    productsPerCorder.ElementAt(i).Pallets = products.ElementAt(i).Pallets;
                    productsPerCorder.ElementAt(i).SheetsPerPallet = products.ElementAt(i).SheetsPerPallet;
                }

            }

            _context.SaveChanges();

            return true;
        }

        public void EditProductFromOrder(ProductCustomerFormModel productModel)
        {

        }

        public IEnumerable<CustomerOrderListViewBySupplier> GetCustomerOrdersBySupplier(int customerId, string supplierId)
        {
            var customerOrdersBySupplier = _context.CustomerOrders.Where(co => co.Id == customerId)
                .Where(or => or.SupplierOrder.Any(i => i.Id == int.Parse(supplierId)))
                .Select(n => new CustomerOrderListViewBySupplier
                { 
                    OrderConfirmationNumber = n.OrderConfirmationNumber,
                    CustomerPoNumber = n.CustomerPoNumber,
                    Date = n.Date,
                    DeliveryAddress = n.DeliveryAddress,
                    LoadingPlace = n.LoadingPlace,
                    StatusId = n.StatusId,
                    PaidAmountStatus = n.PaidAmountStatus,
                    StatusName = _context.Statuses.Where(a=>a.Id == n.StatusId).Select(a=>a.Name).FirstOrDefault()
                }).ToList();

            return customerOrdersBySupplier;
        }
    }
}
