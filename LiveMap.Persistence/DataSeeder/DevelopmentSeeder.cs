﻿using Bogus;
using LiveMap.Domain.Models;
using LiveMap.Persistence.DbModels;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Coordinate = NetTopologySuite.Geometries.Coordinate;

namespace LiveMap.Persistence.DataSeeder; 

// TODO: Make seeder configurable via file. Create a config Options object for the seeder, and use that.
public static class DevelopmentSeeder
{
    private static Faker<SqlMap> GetMapFaker()
    {
        return new Faker<SqlMap>()
            .RuleFor(m => m.Id, f => f.Random.Guid())
            .RuleFor(m => m.Name, f => f.Lorem.Word())
            .RuleFor(m => m.Position, f => new(
                f.Address.Latitude(),
                f.Address.Longitude()))
            .RuleFor(m => m.Border, (f, m) => CreateIrregularPolygon(
                m.Position.X,
                m.Position.Y,
                radius: f.Random.Double(0.01d, 0.05d),  // Random radius between 0.01 and 0.05 degrees
                numberOfPoints: f.Random.Int(25, 70)));  // Random points between 25 and 70 for irregularity
    }

    private static Polygon CreateIrregularPolygon(double centerX, double centerY, double radius, int numberOfPoints = 40)
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        // Create a list of coordinates for the polygon's boundary
        var coordinates = new Coordinate[numberOfPoints + 1];  // +1 to close the ring
        Random rand = new Random();

        for (int i = 0; i < numberOfPoints; i++)
        {
            // Calculate the angle for each point (same as in a circle)
            double angle = i * (2 * Math.PI / numberOfPoints);

            // Calculate the point's x and y coordinates (latitude, longitude) as if it was a perfect circle
            double x = centerX + radius * Math.Cos(angle);
            double y = centerY + radius * Math.Sin(angle);

            // Introduce slight random variation to make the border imperfect
            // Random variation between -0.001 and 0.001 for both x and y directions
            double variation = rand.NextDouble() * 0.002 - 0.001;

            x += variation;
            y += variation;

            // Save the new (slightly distorted) coordinate
            coordinates[i] = new Coordinate(x, y);
        }

        // Close the polygon by adding the first coordinate again at the end
        coordinates[numberOfPoints] = coordinates[0];

        // Create the LinearRing and the Polygon
        var linearRing = new LinearRing(coordinates);
        return geometryFactory.CreatePolygon(linearRing);
    }

    private static Faker<SqlPointOfInterest> GetPointOfInterestFaker(
        List<SqlMap> maps, 
        List<PointOfInterestStatus> statusses, 
        List<Category> categories)
    {
        return new Faker<SqlPointOfInterest>()
            .RuleFor(p => p.Id, f => f.Random.Guid())
            .RuleFor(p => p.Title, f => f.Lorem.Sentence(3))
            .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
            .RuleFor(p => p.Position, f => new(f.Address.Latitude(), f.Address.Longitude())
            {
                SRID = 4326
            })

            .RuleFor(p => p.CategoryName, f => f.PickRandom(categories.Select(c => c.CategoryName)))
            .RuleFor(p => p.Category, (f, p) => categories.Where(c => c.CategoryName == p.CategoryName).First())

            .RuleFor(p => p.StatusName, f => f.PickRandom(statusses.Select(s => s.Status)))
            .RuleFor(p => p.Status, (f, p) => statusses.Where(poi => poi.Status == p.StatusName).First())

            .RuleFor(p => p.MapId, f => maps[f.Random.Int(0, maps.Count - 1)].Id)
            .RuleFor(p => p.Map, (f, p) => maps.Where(map => map.Id == p.MapId).First());
    }

    public static async Task SeedDatabase(LiveMapContext context)
    {
        List<Category> categories = [
            new() { CategoryName = "Store" }, 
            new() { CategoryName = "Information" }, 
            new() { CategoryName = "First-aid & Medical" },
            new() { CategoryName = "Trash bin" },
            new() { CategoryName = "Parking" },
            new() { CategoryName = "Entertainment" },
        ];

        List<PointOfInterestStatus> statusses = [
            new() { Status = "Active" },
            new() { Status = "Inactive" },
            new() { Status = "Pending" },
        ];

        List<SqlMap> maps = GetMapFaker().Generate(1);

        List<SqlPointOfInterest> sqlPointOfInterests = GetPointOfInterestFaker(
            maps: maps,
            statusses: statusses,
            categories: categories
        ).Generate(50);

        await context.Categories.AddRangeAsync(categories);
        await context.PoIStatusses.AddRangeAsync(statusses);
        await context.Maps.AddRangeAsync(maps);
        await context.AddRangeAsync(sqlPointOfInterests);
        await context.SaveChangesAsync();
    }
}