using CourseManagerAPI.DBContexts;
using CourseManagerAPI.Entities;
using CourseManagerAPI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace CourseManager.API.Test
{
    public class AuthorRepositoryTests
    {
        [Fact]
        public void GetAuthor_PageSizeIsThree_ReturnsThreeAuthors()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CourseContext>()
                .UseInMemoryDatabase($"CourseDatabaseForTesting{Guid.NewGuid()}")
                .Options;

            using (var context = new CourseContext(options))
            {
                context.Countries.Add(new CourseManagerAPI.Entities.Country()
                {
                    Id = "BE",
                    Description = "Belgium"
                });
                context.Countries.Add(new CourseManagerAPI.Entities.Country()
                {
                    Id = "US",
                    Description = "United States of America"
                });

                context.Authors.Add(new CourseManagerAPI.Entities.Author()
                { FirstName = "Kevin", LastName = "Dockx", CountryId = "BE" });
                context.Authors.Add(new CourseManagerAPI.Entities.Author()
                { FirstName = "Gill", LastName = "Cleeren", CountryId = "BE" });
                context.Authors.Add(new CourseManagerAPI.Entities.Author()
                { FirstName = "Julie", LastName = "Lerman", CountryId = "US" });
                context.Authors.Add(new CourseManagerAPI.Entities.Author()
                { FirstName = "Shawn", LastName = "Wildermuth", CountryId = "BE" });
                context.Authors.Add(new CourseManagerAPI.Entities.Author()
                { FirstName = "Deborah", LastName = "Kurata", CountryId = "US" });

                context.SaveChanges();
            }

            using(var context = new CourseContext(options))
            { 
                var authorRepository = new AuthorRepository(context);

                // Act
                var author = authorRepository.GetAuthors(1, 3);

                // Assert
                Assert.Equal(3, author.Count());
            }
        }

        [Fact]
        public void GetAuthor_EmptyGuid_ThrowsArgumentException()
        {
            // Arrange 
            var options = new DbContextOptionsBuilder<CourseContext>()
                .UseInMemoryDatabase($"CourseDatabaseForTesting{Guid.NewGuid()}")
                .Options;

            using(var context = new CourseContext(options))
            {
                var authorRepository = new AuthorRepository(context);

                // Assert
                Assert.Throws<ArgumentException>(
                    // Act
                    () => authorRepository.GetAuthor(Guid.Empty));
            }
        }

        [Fact]
        public void AddAuthor_AuthorWithoutCountryId_AuthorHasBEAsCountryId()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CourseContext>()
                .UseInMemoryDatabase($"CourseDatabaseForTesting{Guid.NewGuid()}")
                .Options;

            using (var context = new CourseContext(options))
            {
                context.Countries.Add(new CourseManagerAPI.Entities.Country()
                {
                    Id = "BE",
                    Description = "Belgium"
                });

                context.SaveChanges();
            }

            using (var context = new CourseContext(options))
            {
                var authorRepository = new AuthorRepository(context);
                var authorToAdd = new Author()
                {
                    FirstName = "Kevin",
                    LastName = "Dockx",
                    Id = Guid.Parse("d84d3d7e-3fbc-4956-84a5-5c57c2d86d7b")
                };

                // Act 
                authorRepository.AddAuthor(authorToAdd);
                authorRepository.SaveChanges();
            }

            using (var context = new CourseContext(options))
            {
                // Assert
                var authorRepository = new AuthorRepository(context);
                var addedAuthor = authorRepository.GetAuthor(
                    Guid.Parse("d84d3d7e-3fbc-4956-84a5-5c57c2d86d7b"));
                Assert.Equal("BE", addedAuthor.CountryId);
            }
        }
    }
}
