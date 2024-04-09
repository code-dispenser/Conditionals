using Conditionals.Core.ConsoleDemos.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Conditionals.Core.ConsoleDemos.Common.StaticData;

public static class DemoData
{
    public static IReadOnlyList<Customer>           Customers           { get;}
    public static IReadOnlyList<CustomerAccount>    CustomerAccounts    { get; }
    public static IReadOnlyList<Address>            CustomerAddresses   { get; }
    public static IReadOnlyList<OrderHistoryView>   OrderHistories      { get; }
    public static IReadOnlyList<Order>              SampleOrders        { get; }

    static DemoData()
    {
        Customers           = CreateCustomers();
        CustomerAccounts    = CreateAccounts();
        CustomerAddresses   = CreateAddresses();
        OrderHistories      = CreateOrderHistories();
        SampleOrders        = CreateOrders();
    }

    public static StoreCardApplication? StoreCardApplicationForCustomer(int customerID = 1)
    {
        var customer     = GetCustomer(customerID);
        var address      = GetAddress(customerID);
        var orderHistory = GetOrderHistory(customerID);

        return new StoreCardApplication(customer!.CustomerID, customer.CustomerName, AgeFromDOB(customer.DOB), address!.Country, orderHistory!.TotalOrders);
    }

    public static Customer GetCustomer(int customerID = 1)

        => Customers.Where(c => c.CustomerID == customerID).SingleOrDefault() ?? Customers[0];

    public static Address GetAddress(int customerID = 1)

        => CustomerAddresses.Where(a => a.CustomerID == customerID).SingleOrDefault() ?? CustomerAddresses[0];

    public static CustomerAccount GetAccount(int customerID = 1)

        => CustomerAccounts.Where(a => a.CustomerID == customerID).SingleOrDefault() ?? CustomerAccounts[0];

    public static OrderHistoryView GetOrderHistory(int customerID = 1)
        => OrderHistories.Where(o => o.CustomerID == customerID).SingleOrDefault() ?? OrderHistories[0];

    public static CustomerInfo GetCustomerInfo(int customerID = 1)
    {
        var customer = GetCustomer(customerID);

        return new CustomerInfo(customer.CustomerID, customer.CustomerName, GetAccount(customerID).AccountNo, GetAddress(customerID));
    }
    private static IReadOnlyList<Customer> CreateCustomers()

        => new List<Customer>()
        {
            new (1,"Danielle Baker",new DateOnly(2010,05,09),"01695 720312",CustomerType.Student),
            new (2, "Frederik Goodwin", new DateOnly(1974,01,30),"(732) 528-6445", CustomerType.Subscriber),
            new (3,"Alexander Griffiths", new DateOnly(1986,09,27),"020 8204 0666", CustomerType.Ordinary),
            new (4,"Gerard Donnelly", new DateOnly(1950,07,12),"(866) 592-2742", CustomerType.Pensioner)
        };
    private static IReadOnlyList<Address> CreateAddresses()

        => new List<Address>()
        {
            new (1,"35 Gateford Rd","Worksop","United Kingdom"),
            new (2,"9080 Dixie Hwy","Louisville","United States"),
            new (3, "Hyndburn Rd","Accrington", "United Kingdom"),
            new (4,"131 Kennilworth Rd","Marlton","United States")
        };

    private static IReadOnlyList<CustomerAccount> CreateAccounts()

        => new List<CustomerAccount>()
        {
            new (1, 10001,null),
            new (2, 10002,"348886240255193"),
            new (3, 10003,"378955349175801"),
            new (4, 10004,"3589106335091920")

        };
    private static IReadOnlyList<OrderHistoryView> CreateOrderHistories()

        => new List<OrderHistoryView>()
        {
            new (1, new DateOnly(2022,9,5),new DateOnly(2023,2,15),5, 356.50M),
            new (2, new DateOnly(2020,3,16), new DateOnly(2023,10,10),43, 2589.99M),
            new (3, new DateOnly(2018,4,3), new DateOnly(2023,2,26),30,1005.50M),
            new (4, new DateOnly(2010,4,3), new DateOnly(2023,9,7),73,3158.27M),

        };


    private static IReadOnlyList<Order> CreateOrders()

        => new List<Order>()
        {
            new (1, 101, new DateOnly(2022, 9, 5), 20.50M),
            new (1, 192, new DateOnly(2022, 10, 15), 50.00M),
            new (1, 225, new DateOnly(2022, 11, 25), 95.75M),
            new (1, 567, new DateOnly(2023, 1, 5), 80.00M),
            new (1, 723, new DateOnly(2023, 2, 15), 110.25M)
        };

    public static Device GetTenantDevice(int tenantID)

    => new Device
        (
            TenantID: tenantID,
            new Probe[] {
                            new Probe(TenantID: tenantID, ProbeID: Guid.Parse("4e5a2275-76ed-4b02-af97-7334d0959931"), BatteryLevel: 4, ProbeValue: 51, ResponseTimeMs: 7, ErrorCount: 0),
                            new Probe(TenantID: tenantID, ProbeID: Guid.Parse("2f0a0f4d-9088-4c2e-bd77-116c5bcce769"), BatteryLevel: 9, ProbeValue: 75, ResponseTimeMs: 2, ErrorCount: 0)
                         },
            Online: true,
            PowerOnHours: 507
       );

    private static int AgeFromDOB(DateOnly DOB)

        => (DateTime.UtcNow.DayOfYear < DOB.DayOfYear) ? DateTime.UtcNow.Year - DOB.Year - 1 : DateTime.UtcNow.Year - DOB.Year;
}
