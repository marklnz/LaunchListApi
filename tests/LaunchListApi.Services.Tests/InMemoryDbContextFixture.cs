using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using LaunchListApi.Model;
using LaunchListApi.Model.DataAccess;
using LaunchListApi.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace LaunchListApi.Services.Tests
{

    /// <summary>
    /// Sets up an in-memory SQLite database and seeds it, for use in unit tests where database access is required. 
    /// Should ONLY be used where data access logic cannot be mocked or faked, or where it would be too complex and time consuming to do so.
    /// </summary>
    public class InMemoryDbContextFixture : IDisposable
    {
        private DbConnection _connection;
        private bool seeded; 

        public LaunchListApiContext ctx { get; }

        private DbContextOptions<LaunchListApiContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<LaunchListApiContext>()
                .UseSqlite(_connection).Options;
        }

        public InMemoryDbContextFixture()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                var options = CreateOptions();
                using (var context = new LaunchListApiContext(options))
                {
                    context.Database.EnsureCreated();

                    // Seed data
                    if (seeded == false)
                    {
                        _seedData(context);
                        seeded = true;
                    }
                }
            }

            this.ctx = new LaunchListApiContext(CreateOptions());
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        /** Data lists used to fill up the sample client data. */
        string[] COLORS = new string[] { "Maroon", "Red", "Orange", "Yellow", "Olive", "Green", "Purple", "Fuchsia", "Lime", "Teal", "Aqua", "Blue", "Navy", "Black", "Gray" };
        string[] NAMES = new string[] { "Maia", "Asher", "Olivia", "Atticus", "Amelia", "Jack", "Charlotte", "Theodore", "Isla", "Oliver", "Isabella", "Jasper", "Cora", "Levi", "Violet", "Arthur", "Mia", "Thomas", "Elizabeth", "Phil", "Mark", "Greg", "Zoe" };
        string[] STATUSES = new string[] { "Active", "Suspended", "Cancelled" };
        string[] AGENCIES = new string[] { "CCS Disability Action", "Parkinsonism - Auckland", "Blind Foundation", "Age Concern Auckland", "Stroke Foundation" };

        private void _seedData(LaunchListApiContext dc)
        {
            // Seed reference data
            using (dc)
            {
                //// Tenant
                //dc.Add(new Tenant() { Name = "Ridewise Dev", ContactEmail = "mark.lawrence@eyede.co.nz", ContactName = "Mark Lawrence", Status = EntityStatus.Active });
                //dc.SaveChanges();
                //dc.SaveChangesAsync();

                //// Agencies
                //List<Agency> agencies = new List<Agency>();
                //AGENCIES.ToList().ForEach(a => agencies.Add(new Agency() { EventStreamId = Guid.NewGuid(), EventVersionTimestamp = DateTimeOffset.Now, Name = a, Status = EntityStatus.Active, Tenant = dc.Tenants.First() }));

                //dc.Agencies.AddRange(agencies);
                //dc.SaveChanges();

                //// Add agency contacts
                //foreach (Agency thisAgency in dc.Agencies)
                //{
                //    thisAgency.Contacts = new List<AgencyContact>();

                //    thisAgency.Contacts.Add(new AgencyContact()
                //    {
                //        Designation = "Administrator",
                //        EmailAddress = "test.Contact@eyede.com",
                //        FullName = $"Admin @{thisAgency.Name}"
                //    });

                //    thisAgency.Contacts.Add(new AgencyContact()
                //    {
                //        Designation = "Assessor",
                //        EmailAddress = "test.Contact@eyede.com",
                //        FullName = $"Assessor @{thisAgency.Name}"
                //    });
                //}

                //dc.SaveChanges();

                //// Clients
                //for (int i = 0; i < 100; i++)
                //{
                //    dc.Clients.Add(this._createNewClient(dc, i));
                //}
                //dc.SaveChanges();

                //// Transport Operators
                //List<TransportOperator> operators = new List<TransportOperator>();
                //COLORS.ToList().ForEach(c => operators.Add(new TransportOperator()
                //{
                //    EventStreamId = Guid.NewGuid(),
                //    EventVersionTimestamp = DateTimeOffset.Now,
                //    Name = string.Concat(c, " Cabs"),
                //    Status = EntityStatus.Active,
                //    Tenant = dc.Tenants.First()
                //}));
                //dc.TransportOperators.AddRange(operators);
                //dc.SaveChanges();

                //List<Driver> drivers = new List<Driver>();
                //COLORS.ToList().ForEach(c => drivers.Add(new Driver()
                //{
                //    FirstNames = NAMES[new Random().Next(0, NAMES.Length - 1)],
                //    Surname = c,
                //    ContactNumber = "123-4567",
                //    DriverLicenceExpiryDate = DateTimeOffset.Now.AddYears(2),
                //    DriverLicenceNumber = "AB123456",
                //    LicenceNumber = new Random().Next(100, 999).ToString(),
                //    PassengerLicenceRenewalDue = DateTimeOffset.Now.AddYears(1),
                //    Status = EntityStatus.Active,
                //    TransportOperator = dc.TransportOperators.First()
                //}));
                //dc.Drivers.AddRange(drivers);
                //dc.SaveChanges();

                //// Vehicles
                //for (int i = 0; i < 20; i++)
                //{
                //    dc.Vehicles.Add(new Vehicle()
                //    {
                //        RegistrationPlate = string.Concat("ABC1", i.ToString()),
                //        FleetNumber = (20 + i).ToString(),
                //        CofExpiryDate = DateTimeOffset.Now.AddYears(2),
                //        MeterExemptionExpiryDate = DateTimeOffset.Now.AddYears(2),
                //        RegistrationDate = DateTimeOffset.Now.AddYears(-1),
                //        IsHoistCapable = new Random().Next(1, 2000) < 1500,
                //        OdometerReading = 12500,
                //        Status = EntityStatus.Active,
                //        TransportOperator = dc.TransportOperators.First()
                //    });
                //}
                //dc.SaveChanges();
            }
        }

        //private Client _createNewClient(RidewiseDbContext db, int clientIndex)
        //{
        //    DateTime today = DateTimeOffset.Now.Date;
        //    string agencyName = AGENCIES[new Random().Next(0, AGENCIES.Length - 1)];

        //    return new Client()
        //    {
        //        EventStreamId = Guid.NewGuid(),
        //        EventVersionTimestamp = DateTimeOffset.Now,
        //        ClientNumber = clientIndex.ToString("D8"),
        //        GivenNames = NAMES[new Random().Next(0, NAMES.Length - 1)],
        //        Surname = "Client" + clientIndex.ToString("D2"),
        //        Status = (EntityStatus)Enum.Parse(typeof(EntityStatus), STATUSES[new Random().Next(0, STATUSES.Length - 1)]),
        //        // This is hacky, but it gives a better distribution of the values - get a random number in the range from 1 to 200, 
        //        // then divide that by 100 and add 1 to the quotient to get 1 or 2 as the final integer value which we cast to the Enum to get the value. 
        //        DisabilityType = (DisabilityType)((new Random().Next(1, 200) / 100) + 1),
        //        DateOfBirth = today.AddDays(0 - (new Random().Next(0, 29200))),
        //        PhysicalAddress = new Address()
        //        {
        //            AddressLine1 = new Random().Next(0, 100).ToString() + " " + COLORS[new Random().Next(0, COLORS.Length - 1)] + " Street",
        //            AddressLine2 = COLORS[new Random().Next(0, COLORS.Length - 1)] + "ville",
        //            AddressLine4 = COLORS[new Random().Next(0, COLORS.Length - 1)] + " City",
        //            Country = "New Zealand",
        //            Postcode = "1000"
        //        },
        //        Agency = db.Agencies.First(a => a.Name == agencyName),
        //        HoistUser = new Random().Next(1, 2000) < 800,
        //        NextReviewDate = today.Date.AddDays(new Random().Next(0, 730)).Date,
        //    };
        //}
    }


}

