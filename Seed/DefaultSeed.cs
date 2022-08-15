using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApi.Entities.DbFlags;
using WebApi.Entities.Events;
using WebApi.Entities.Misc;
using WebApi.Entities.Users;
using WebApi.Entities.Venues;
using WebApi.Helpers;

namespace WebApi.Seed
{
    public static class DefaultSeeds
    {
        public static bool DataSeeded { get; set; } = false;

        public async static Task<bool> CheckIsSeeded(DataContext dataContext)
        {
            var dataSeeded = await dataContext.DbFlags.Where(f => f.Name.Equals("DataSeeded")).FirstOrDefaultAsync();

            if (dataSeeded == null)
            {
                return false;
            }

            DataSeeded = true;
            return true;
        }

        public static async Task<bool> SeedAsync(DataContext dataContext)
        {
            // add addresses
            string addressJSON = File.ReadAllText(@"Seed" + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar + "address.json");
            List<Address> addressList = JsonConvert.DeserializeObject<List<Address>>(addressJSON);
            dataContext.Address.AddRange(addressList);

            // add venueTypes
            string venueTypesJSON = File.ReadAllText(@"Seed" + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar + "venueTypes.json");
            List<VenueType> venueTypesList = JsonConvert.DeserializeObject<List<VenueType>>(venueTypesJSON);
            dataContext.VenueTypes.AddRange(venueTypesList);

            // add venues
            string venuesJSON = File.ReadAllText(@"Seed" + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar + "venues.json");
            List<Venue> venuesList = JsonConvert.DeserializeObject<List<Venue>>(venuesJSON);

            await dataContext.SaveChangesAsync();

            foreach (var veneu in venuesList)
            {
                dataContext.Add(veneu);
                dataContext.Entry(veneu).Property("VenueTypeId").CurrentValue = new Random().Next(1, 250);
                dataContext.Entry(veneu).Property("AddressId").CurrentValue = new Random().Next(1, 250);
            }

            // add tags
            string tagsJSON = File.ReadAllText(@"Seed" + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar + "tags.json");
            List<Tag> tagsList = JsonConvert.DeserializeObject<List<Tag>>(tagsJSON);
            dataContext.Tags.AddRange(tagsList);

            await dataContext.SaveChangesAsync();

            // add events
            string eventsJSON = File.ReadAllText(@"Seed" + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar + "events.json");
            List<Event> eventList = JsonConvert.DeserializeObject<List<Event>>(eventsJSON);

            foreach (var eventfromDb in eventList)
            {
                dataContext.Add(eventfromDb);
                dataContext.Entry(eventfromDb).Property("VenueId").CurrentValue = new Random().Next(1, 1000);
                eventfromDb.Tags.Add(tagsList[new Random().Next(1, 75)]);
                eventfromDb.Tags.Add(tagsList[new Random().Next(76, 150)]);
                eventfromDb.Tags.Add(tagsList[new Random().Next(151, 250)]);                
            }

            // add users
            string usersJSON = File.ReadAllText(@"Seed" + Path.DirectorySeparatorChar + "Data" + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar + "users.json");
            List<User> usersList = JsonConvert.DeserializeObject<List<User>>(usersJSON);

            foreach (var user in usersList)
            {
                dataContext.Add(user);
                dataContext.Entry(user).Property("PasswordHash").CurrentValue = BCrypt.Net.BCrypt.HashPassword(dataContext.Entry(user).Property("UserName").CurrentValue.ToString());
                user.UserDetails.ImageUrl = "https://thispersondoesnotexist.com/image";
                user.UserDetails.Description = "Hey, I am free every Friday and Monday. Feel free to contact me I am up for soccer and tenis.";
                user.Tags.Add(tagsList[new Random().Next(1, 75)]);
                user.Tags.Add(tagsList[new Random().Next(76, 150)]);
                user.Tags.Add(tagsList[new Random().Next(151, 250)]);   
            }

            dataContext.Users.AddRange(usersList);

            await dataContext.SaveChangesAsync();

            var dataSeeded = new DbFlags() { Name = "DataSeeded", Value = true };
            dataContext.DbFlags.Add(dataSeeded);

            await dataContext.SaveChangesAsync();

            DataSeeded = true;

            return true;
        }
    }
}
