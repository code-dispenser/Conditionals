using Xunit.Sdk;

namespace Conditionals.Core.Tests.SharedDataAndFixtures.Models
{
    public record Address(string AddressLine, string Town, string City, string PostCode);
    public record Customer(string CustomerName, int CustomerNo, decimal TotalSpend, int MemberYears, Address? Address = null);
    public record Supplier(string SupplierName, int SupplierNo, decimal TotalPurchases);

    public record Person(string FirstName, string Surname, DateOnly DateOfBirth)
    {
        public int Age => (DateTime.UtcNow.DayOfYear < DateOfBirth.DayOfYear) ? DateTime.UtcNow.Year - DateOfBirth.Year - 1 : DateTime.UtcNow.Year - DateOfBirth.Year;
    }


    public record InjectedStrategy(string StrategyValue);


    public class BadObject
    {
        public string Problem => throw new NotImplementedException();

        public string Name { get; }

        public BadObject(string name)
        {
            Name = name;
        }         

            
        
    }
}
