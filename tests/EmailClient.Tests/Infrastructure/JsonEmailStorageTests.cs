using Xunit;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmailClient.Infrastructure.Repositories;
using EmailClient.Core.Models;
using System;

namespace EmailClient.Tests.Infrastructure
{
    public class JsonEmailStorageTests : IDisposable
    {
        private readonly string _testFilePath;
        private readonly JsonEmailStorage _storage;

        public JsonEmailStorageTests()
        {
            _testFilePath = $"test_emails_{Guid.NewGuid()}.json";
            _storage = new JsonEmailStorage(_testFilePath);
        }

        [Fact]
        public async Task SaveAndLoad_ShouldPersistDataCorrectly()
        {
            // Arrange
            var email = new EmailMessage 
            { 
                Subject = "Integration Test", 
                From = "tester@test.com", 
                Date = DateTime.Now 
            };

            await _storage.SaveAsync(email);

            var newStorageSession = new JsonEmailStorage(_testFilePath);
            var loadedEmails = await newStorageSession.LoadAllAsync();

            // Assert
            Assert.Single(loadedEmails);
            Assert.Equal("Integration Test", loadedEmails[0].Subject);
        }

        public void Dispose()
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
    }
}