using AutoMapper.Internal;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using SSMO.Data;
using SSMO.Data.Models;
using SSMO.Models.Reports.ServiceOrders;
using SSMO.Models.Reports.ServiceOrders.ModelsForPrint;
using SSMO.Models.ServiceOrders;
using SSMO.Models.TransportCompany;
using SSMO.Services.Addresses;
using SSMO.Services.Customer;
using SSMO.Services.FiscalAgent;
using SSMO.Services.MyCompany;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace SSMO.Services.TransportService
{
    public class TransportService : ITransportService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IAddressService addressService;
        private readonly ISupplierService supplierService;
        private readonly IMycompanyService mycompanyService;
        private readonly ICurrency currency;
        private readonly ICustomerService customerService;
        private readonly IFiscalAgentService fiscalAgentService;
        public TransportService(ApplicationDbContext dbContext, IAddressService addressService,
            ISupplierService supplierService, IMycompanyService mycompanyService,
            ICurrency currency, ICustomerService customerService, IFiscalAgentService fiscalAgentService)
        {
            this.dbContext = dbContext;
            this.addressService = addressService;
            this.supplierService= supplierService;
            this.mycompanyService = mycompanyService;   
            this.currency = currency;
            this.customerService = customerService;
            this.fiscalAgentService = fiscalAgentService;
        }

        //create trasnport company
        public bool CreateTransportCompany
            (string name, string eik, string vat, string phoneNumber, 
            string email, string city, string country, string address, string manager, string userId)
        {
            var addressId = addressService.CreateAddress(address, city, country, null, null, null,null,
                null,null,null,null,null);

            var transportCompany = new TransportCompany
            {
                Name = name,
                Eik = eik,
                Vat = vat,
                PhoneNumber = phoneNumber,
                Email = email,
                ContactPerson = manager,
                ServiceOrders = new List<ServiceOrder>(),
                AddressId = addressId,
                UserId= userId
            };

            if(transportCompany == null)
            {
                return false;
            }

            dbContext.TransportCompanies.Add(transportCompany);
            dbContext.SaveChanges();

            return true;
        }

        //create trasnport order 
        public bool CreateTransportOrder
            (int? number, DateTime date, int transportCompanyId, string loadingAddress,
             string eta, string etd, string deliveryAddress, string truckNumber, decimal cost, 
            int myCompanyId, int? supplierId, int? supplierOrderId, int? customerId, int? customerOrderId, 
            int? fiscalAgent, int currencyId, int? vat, string driverName, string driverPhone,
            string comment, string weight, string paymentMethod, string paymentTerms, string payer)
        {
            var transportOrder = new ServiceOrder
            {
                Cost = cost,
                CurrencyId = currencyId,
                CustomerOrderId = customerOrderId,
                Date = date,
                LoadingAddress = loadingAddress,
                DeliveryAddress = deliveryAddress,
                SupplierOrderId = supplierOrderId,
                Eta = eta,
                Etd = etd,
                FiscalAgentId = fiscalAgent,
                MyCompanyId = myCompanyId,
                TransportCompanyId = transportCompanyId,
                TruckNumber = truckNumber,
                Vat = vat ?? 0,
                DriverName = driverName,
                DriverPhone = driverPhone,
                Comment = comment,//koj e plateca i koga shte se plashta
                GrossWeight= weight,
                Payer = payer,
                PaymentMethod = paymentMethod,
                PaymentTerms= paymentTerms
            };

            if(number != null)
            {             
                transportOrder.Number = (int)number;
            }
            else
            {
                transportOrder.Number = dbContext.ServiceOrders.Select(a=>a.Number).LastOrDefault() + 1;               
            }

            if(transportOrder == null)
            {
                return false;
            }

            transportOrder.AmountAfterVat = cost + cost * vat / 100 ?? 0;
            transportOrder.Balance = transportOrder.AmountAfterVat;

            dbContext.ServiceOrders.Add(transportOrder);    
            dbContext.SaveChanges();
            return true;
        }

        public bool EditTransportCompany
            (int id, string name, string eik, string vat, string phoneNumber, 
            string email, string city, string country, string address, string manager)
        {
            var companyForEdit = dbContext.TransportCompanies.FirstOrDefault(a=>a.Id == id);
            if(companyForEdit == null) { return false; }

            //edit address of the company
            addressService.ЕditAddress
                (companyForEdit.AddressId,address,city,country,null,null,null,null,null,null,
                null, null, null);

            companyForEdit.ContactPerson = manager;
            companyForEdit.Name = name;
            companyForEdit.Eik = eik;
            companyForEdit.Vat = vat;
            companyForEdit.PhoneNumber = phoneNumber;
            companyForEdit.Email = email;

            dbContext.SaveChanges();
            return true;
        }

        public ServiceOrderForEditViewModel GetForEditTransportOrder(int id)
        {
            var order = dbContext.ServiceOrders.Find(id);
            if(order == null) { return null;}
            var mycompany = dbContext.MyCompanies.Find(order.MyCompanyId);           
            var currencyName = dbContext.Currencies
                .Where(n=>n.Id == order.CurrencyId)
                .Select(n=>n.Name)  
                .FirstOrDefault();

            var fiscalAgent = dbContext.FiscalAgents
                .Find(order.FiscalAgentId);

            var orderForEdit = new ServiceOrderForEditViewModel
            {
                Comment = order.Comment,
                TruckNumber = order.TruckNumber,
                Vat = order.Vat,
                AmountAfterVat = order.AmountAfterVat,
                CurrencyId = order.CurrencyId,
                Cost = order.Cost,
                Date = order.Date,
                DeliveryAddress = order.DeliveryAddress,
                DriverName = order.DriverName,
                DriverPhone = order.DriverPhone,
                Eta = order.Eta,
                Etd = order.Etd,
                LoadingAddress = order.LoadingAddress,
                GrossWeight = order.GrossWeight,
                Currency = currencyName,    
                PaymentMethod= order.PaymentMethod,
                PaymentTerms= order.PaymentTerms,   
                Payer = order.Payer
            };

            if(fiscalAgent != null)
            {
                orderForEdit.FiscalAgentName = fiscalAgent.Name;
            }
            //spisak dostavchici
            orderForEdit.MyCompanies = mycompanyService.GetCompaniesForTransportOrder();//spisak na moite firmi
            orderForEdit.Currencies = currency.AllCurrency();//spisak valuti
            orderForEdit.FiscalAgents = fiscalAgentService.FiscalAgentsCollection();
            orderForEdit.TransportCompany = TransportCompanies();

            return orderForEdit;
        }

        public bool FirstTransport()
        {
            var check = dbContext.ServiceOrders.Any() == true;
            if (!check)
            {
                return true;

            }
            return false;
        }

        public EditTransportCompanyFormModel GetTransportCompanyForEdit(int id)
        {
            var company = dbContext.TransportCompanies.Find(id);

            if(company == null) { return null; }

            var address = dbContext.Addresses.Find(company.AddressId);

            var editCompany = new EditTransportCompanyFormModel
            {
                Id = id,
                Name = company.Name,
                Eik = company.Eik,
                Vat = company.Vat,
                ContactPerson = company.ContactPerson,
                Email = company.Email,
                PhoneNumber = company.PhoneNumber,
                Address = address.Street,
                City = address.City,
                Country = address.Country,
            };

            return editCompany;
        }

        public ServiceOrderDetailsPrintViewModel ServiceOrderPrintDetails(int id)
        {           
            var order = dbContext.ServiceOrders.Find(id);
            if(order == null) { return null;}

            var delimiter = ",";

            var trasnportCompany = dbContext.TransportCompanies.Find(order.TransportCompanyId);
            var transportCompanyAddress = dbContext.Addresses.Find(trasnportCompany.AddressId);
            var mycompany = dbContext.MyCompanies.Find(order.MyCompanyId);
            var companyAddress = dbContext.Addresses.Find(mycompany.AddressId);
            var fiscalAgent = dbContext.FiscalAgents.Find(order.FiscalAgentId);
          
            var serviceOrder = new ServiceOrderDetailsPrintViewModel
            {
                Number= order.Number,
                Date = order.Date,
                Comment = order.Comment,
                Cost = order.Cost,
                Currency = dbContext.Currencies.Where(i => i.Id == order.CurrencyId)
                   .Select(n => n.Name).FirstOrDefault(),
                DeliveryAddress = order.DeliveryAddress,
                DriverName = order.DriverName,
                DriverPhone = order.DriverPhone,
                Etd = order.Etd,
                GrossWeight = order.GrossWeight,
                Payer = order.Payer,
                PaymentTerms= order.PaymentTerms,
                PaymentMethod= order.PaymentMethod,
                LoadingAddress = order.LoadingAddress,
                TruckNumber = order.TruckNumber,
                MyCompany = new MyCompanyDetailsTransportOrderPrintViewModel
                {
                    Name = mycompany.Name,
                    Vat = mycompany.VAT,
                    Address = companyAddress.Street + delimiter + companyAddress.City+delimiter + companyAddress.Country,
                    CorrespondAddress = companyAddress.CorrespondStreet + delimiter + companyAddress.CorrespondCity +delimiter + companyAddress.CorrespondCountry
                },
                TransportCompany = new TransportCompanyDetailsOrderPrintViewModel
                {
                    Name = trasnportCompany.Name,
                    Vat = trasnportCompany.Vat,
                    Address = transportCompanyAddress.Street + delimiter + transportCompanyAddress.City + delimiter + transportCompanyAddress.Country
                }                
            };

            if(fiscalAgent!= null )
            {
                serviceOrder.FiscalAgent = new FiscalAgentDetailsForPrintViewModel
                {
                    FiscalAgentName = fiscalAgent.Name,
                    FiscalAgentDetails = fiscalAgent.Details
                };
            }

            var descriptionId = new List<int>();

            if (order.CustomerOrderId != null)
            {
                var products = dbContext.CustomerOrderProductDetails
                      .Where(c => c.CustomerOrderId == order.CustomerOrderId)
                      .Select(d => d.ProductId)
                      .ToList();

                 descriptionId = dbContext.Products
                    .Where(i=>products.Contains(i.Id))
                    .Select(n => n.DescriptionId)
                    .ToList();
            }
            else if (order.SupplierOrderId != null)
            {
                descriptionId = dbContext.Products
                     .Where(c => c.SupplierOrderId == order.SupplierOrderId)
                     .Select(d => d.DescriptionId)
                     .ToList();                
            }

            var descriptions = dbContext.Descriptions
                    .Where(a => descriptionId.Contains(a.Id))
                    .Select(n => n.Name).ToList();

            serviceOrder.ProductType = string.Join(delimiter, descriptions);

            return serviceOrder;
        }

        public List<TransportCompanyListViewModel> TransportCompanies()
        {
            return dbContext.TransportCompanies.Select(n=>new TransportCompanyListViewModel
            {
                Id = n.Id,
                Name = n.Name,
            }).ToList();           
        }

        public bool EditTransportOrder
            (int id, DateTime date, int transportCompanyId, string loadingAddress, 
            string eta, string etd, string deliveryAddress, string truckNumber, decimal cost, 
            int myCompanyId, int? supplierOrderId, int? customerOrderId, int? fiscalAgent, 
            int currencyId, int? vat, string driverName, string driverPhone, string comment, 
            string weight, string paymentMethod, string paymentTerms, 
            string payer)
        {
            var order = dbContext.ServiceOrders.FirstOrDefault(a => a.Id == id);
            if (order == null)
            {
                return false;
            }

            order.Date = date;
            order.TransportCompanyId = transportCompanyId;
            order.LoadingAddress = loadingAddress;
            order.Eta= eta;
            order.Etd = etd;
            order.DeliveryAddress = deliveryAddress;
            order.TruckNumber = truckNumber;
            order.Cost = cost;
            order.MyCompanyId = myCompanyId;            
            order.SupplierOrderId = supplierOrderId;
            order.CustomerOrderId = customerOrderId;
            order.FiscalAgentId = fiscalAgent;
            order.CurrencyId = currencyId;
            order.Vat = vat ?? 0;
            order.DriverName = driverName;
            order.DriverPhone = driverPhone;
            order.Comment = comment;
            order.GrossWeight = weight;
            order.PaymentMethod = paymentMethod;
            order.Payer = payer;
            order.PaymentTerms= paymentTerms;

            dbContext.SaveChanges();

           return true;
        }
    }
}
