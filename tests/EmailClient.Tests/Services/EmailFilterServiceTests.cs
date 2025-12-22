using Xunit;
using System.Collections.Generic;
using EmailClient.Application.Services;
using EmailClient.Core.Models;
using System;

namespace EmailClient.Tests.Services
{
    public class EmailFilterServiceTests
    {
        private readonly EmailFilterService _filterService;

        public EmailFilterServiceTests()
        {
            _filterService = new EmailFilterService();
        }

        [Fact]
        public void FilterBySubject_ShouldReturnMatchingEmails_WhenKeywordExists()
        {
            // Arrange (Підготовка)
            var emails = new List<EmailMessage>
            {
                new EmailMessage { Subject = "Optima Practice", From = "tutor@optima.edu" },
                new EmailMessage { Subject = "Discount", From = "shop@store.com" },
                new EmailMessage { Subject = "Practice Report", From = "student@optima.edu" }
            };
            string keyword = "Practice";

            // Act (Дія)
            var result = _filterService.FilterBySubject(emails, keyword);

            // Assert (Перевірка)
            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Subject == "Optima Practice");
            Assert.Contains(result, e => e.Subject == "Practice Report");
        }

        [Fact]
        public void FilterBySubject_ShouldReturnAllEmails_WhenKeywordIsEmpty()
        {
            // Arrange
            var emails = new List<EmailMessage>
            {
                new EmailMessage { Subject = "Test", From = "test@test.com" }
            };

            // Act
            var result = _filterService.FilterBySubject(emails, "");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void FilterBySubject_ShouldBeCaseInsensitive()
        {
            // Arrange
            var emails = new List<EmailMessage>
            {
                new EmailMessage { Subject = "OPTIMA", From = "test@test.com" }
            };

            // Act
            var result = _filterService.FilterBySubject(emails, "optima");

            // Assert
            Assert.Single(result);
        }
    }
}