using Domain;
using Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Infrastructure.Persistences
{
    public class ProductionContextSeed : SeedDataBase
    {
        private readonly ProductionContext _dbContext;
        private ILogger<ProductionContextSeed> _logger;

        public ProductionContextSeed(ProductionContext dbContext, string contentRootPath, string seedDataFolder, ILogger<ProductionContextSeed> logger) : base(dbContext, contentRootPath, seedDataFolder)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task SeedAsync()
        {
            try
            {
                var policy = CreatePolicy(_logger, nameof(ProductionContextSeed));

                await policy.ExecuteAsync(async () =>
                {
                    if (!_dbContext.Customers.Any())
                    {
                        var customers = GetCustomersFromFile();
                        if (customers.Any())
                        {
                            await _dbContext.Customers.AddRangeAsync(customers);
                            _dbContext.SaveChanges();
                        }
                    }

                    if (!_dbContext.ProductLevels.Any())
                    {
                        var productLevels = GetProductLevelsFromFile();
                        if (productLevels.Any())
                        {
                            await _dbContext.ProductLevels.AddRangeAsync(productLevels);
                            _dbContext.SaveChanges();
                        }
                    }

                    // Create staff from register SSO
                    //if (!_dbContext.Staffs.Any())
                    //{
                    //    var staffs = GetStaffsFromFile();
                    //    if (staffs.Any())
                    //    {
                    //        await _dbContext.Staffs.AddRangeAsync(staffs);
                    //        _dbContext.SaveChanges();
                    //    }
                    //}

                    if (!_dbContext.Steps.Any())
                    {
                        var steps = GetStepsFromFile();
                        if (steps.Any())
                        {
                            await _dbContext.Steps.AddRangeAsync(steps);
                            _dbContext.SaveChanges();
                        }
                    }

                    if (!_dbContext.Roles.Any())
                    {
                        var roles = GetRoles();
                        if (roles.Any())
                        {
                            await _dbContext.Roles.AddRangeAsync(roles);
                            _dbContext.SaveChanges();
                        }
                    }

                    if (!_dbContext.Groups.Any())
                    {
                        var groups = GetGroups();
                        if (groups.Any())
                        {
                            await _dbContext.Groups.AddRangeAsync(groups);
                            _dbContext.SaveChanges();
                        }
                    }

                    if (!_dbContext.Shifts.Any())
                    {
                        var shifts = GetShifts();
                        if (shifts.Any())
                        {
                            await _dbContext.Shifts.AddRangeAsync(shifts);
                            _dbContext.SaveChanges();
                        }
                    }

                });
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductionContextSeed Error", ex.Message);
            }
        }

        private IEnumerable<Role> GetRoles()
        {
            return Enumeration.GetAll<Role>();
        }

        private IEnumerable<Domain.Group> GetGroups()
        {
            return Enumeration.GetAll<Domain.Group>();
        }

        private IEnumerable<Shift> GetShifts()
        {
            return Enumeration.GetAll<Shift>();
        }

        // Customers
        private IEnumerable<Customer> GetCustomersFromFile()
        {
            string csvFile = GetPathToFile("Customers.csv");

            if (!File.Exists(csvFile))
            {
                return new List<Customer>();
            }

            string[] requiredHeaders = { "CoCode", "Location", "Currency", "CodeProduct", "ProductName",
                "UnitPrice", "Saler", "Note", "PSE", "PSW" };

            string[] headers = csvFile.GetHeaders(requiredHeaders);

            return File.ReadAllLines(csvFile)
                                        .Skip(1) // skip header row
                                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                        .SelectTry(columns => CreateCustomer(columns, headers))
                                        .OnCaughtException(ex => { _logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x is not null);
        }

        private Customer CreateCustomer(string[] columns, string[] headers)
        {
            return Customer.Create(
                    "CoCode".GetColumnValue(columns, headers),
                    EnumExtension.ParseEnum<Location>("Location".GetColumnValue(columns, headers)),
                    EnumExtension.ParseEnum<Currency>("Currency".GetColumnValue(columns, headers)),
                    "CodeProduct".GetColumnValue(columns, headers),
                    "ProductName".GetColumnValue(columns, headers),
                    double.Parse("UnitPrice".GetColumnValue(columns, headers)),
                    "Saler".GetColumnValue(columns, headers),
                    "Note".GetColumnValue(columns, headers),
                    double.Parse("PSE".GetColumnValue(columns, headers)),
                    double.Parse("PSW".GetColumnValue(columns, headers))
                );
        }


        // ProductLevels
        private IEnumerable<ProductLevel> GetProductLevelsFromFile()
        {
            string csvFile = GetPathToFile("ProductLevels.csv");

            if (!File.Exists(csvFile))
            {
                return new List<ProductLevel>();
            }

            string[] requiredHeaders = { "Code", "Name", "Description" };

            string[] headers = csvFile.GetHeaders(requiredHeaders);

            return File.ReadAllLines(csvFile)
                                        .Skip(1) // skip header row
                                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                        .SelectTry(columns => CreateProductLevel(columns, headers))
                                        .OnCaughtException(ex => { _logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x is not null);
        }

        private ProductLevel CreateProductLevel(string[] columns, string[] headers)
        {
            return ProductLevel.Create(
                "Code".GetColumnValue(columns, headers),
                "Name".GetColumnValue(columns, headers),
                "Description".GetColumnValue(columns, headers)
                );
        }


        // Staffs
        private IEnumerable<Staff> GetStaffsFromFile()
        {
            string csvFile = GetPathToFile("Staffs.csv");

            if (!File.Exists(csvFile))
            {
                return new List<Staff>();
            }

            string[] requiredHeaders = { "FullName", "Account", "Email", "Role", "Group" };

            string[] headers = csvFile.GetHeaders(requiredHeaders);

            return File.ReadAllLines(csvFile)
                                        .Skip(1) // skip header row
                                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                        .SelectTry(columns => CreateStaff(columns, headers))
                                        .OnCaughtException(ex => { _logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x is not null);
        }

        private Staff CreateStaff(string[] columns, string[] headers)
        {
            try
            {
                return Staff.Create(
                    "UserId".GetColumnValue(columns, headers),
                    "FullName".GetColumnValue(columns, headers),
                    "Account".GetColumnValue(columns, headers),
                    "Email".GetColumnValue(columns, headers),
                    new Role[] { Enumeration.FromDisplayName<Role>("Role".GetColumnValue(columns, headers)) },
                    new Domain.Group[] { Enumeration.FromDisplayName<Domain.Group>("Group".GetColumnValue(columns, headers)) }
                    );
            }
            catch (Exception ex) { throw; }
        }


        // Steps
        private IEnumerable<Step> GetStepsFromFile()
        {
            string csvFile = GetPathToFile("Steps.csv");

            if (!File.Exists(csvFile))
            {
                return new List<Step>();
            }

            string[] requiredHeaders = { "Name", "Code", "OrderNumber", "ProductLevel", "EstimationInSeconds", "GroupId" };

            string[] headers = csvFile.GetHeaders(requiredHeaders);

            return File.ReadAllLines(csvFile)
                                        .Skip(1) // skip header row
                                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                                        .SelectTry(columns => CreateStep(columns, headers))
                                        .OnCaughtException(ex => { _logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                                        .Where(x => x is not null);
        }

        private Step CreateStep(string[] columns, string[] headers)
        {
            try
            {
                return Step.Create(
                       "Name".GetColumnValue(columns, headers),
                       "Code".GetColumnValue(columns, headers),
                       int.Parse("OrderNumber".GetColumnValue(columns, headers)),
                       _dbContext.ProductLevels.FirstOrDefault(p => p.Code == "ProductLevel".GetColumnValue(columns, headers)),
                       int.Parse("GroupId".GetColumnValue(columns, headers)),
                       int.Parse("EstimationInSeconds".GetColumnValue(columns, headers))
                    );
            }
            catch (Exception ex) { throw; }
        }

    }
}
