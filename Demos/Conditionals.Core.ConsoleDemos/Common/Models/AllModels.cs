namespace Conditionals.Core.ConsoleDemos.Common.Models;


public enum CustomerType : int
{
    Ordinary    = 0,
    Student     = 1,
    Pensioner   = 2,
    Subscriber  = 3
}

public record AppSettings(string EmailAddress, string APIUrl, string DBWriteConnectionString);
public record Address(int CustomerID, string AddressLine, string TownCity, string Country);
public record Customer(int CustomerID, string CustomerName, DateOnly DOB, string ContactTel, CustomerType CustomerType);
public record StoreCardApplication(int CustomerID, string CustomerName, int Age, string CountryOfResidence, int TotalOrders);
public record CustomerAccount(int CustomerID, int AccountNo, string? CardNoOnFile);
public record OrderHistoryView(int CustomerID, DateOnly FirstOrderDate, DateOnly LastOrderDate, int TotalOrders, decimal TotalSpend);
public record Order(int CustomerID, int OrderID, DateOnly OrderDate, decimal OrderTotal);
public record CustomerInfo(int CustomerID, string CustomerName, int AccountNo, Address Address);

public record Probe(int TenantID, Guid ProbeID, int BatteryLevel, int ProbeValue, int ResponseTimeMs, int ErrorCount);
public record Device(int TenantID, Probe[] Probes, bool Online, int PowerOnHours);