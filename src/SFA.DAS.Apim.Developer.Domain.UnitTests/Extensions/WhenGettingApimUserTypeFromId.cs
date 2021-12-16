using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Apim.Developer.Domain.Extensions;
using SFA.DAS.Apim.Developer.Domain.Models;

namespace SFA.DAS.Apim.Developer.Domain.UnitTests.Extensions
{
    public class WhenGettingApimUserTypeFromId
    {
        [Test]
        public void Then_If_Numeric_Then_Provider()
        {
            //Act
            var actual = 123456.ToString().ApimUserType();
            
            //Assert
            actual.Should().Be(ApimUserType.Provider);
        }
        
        [Test]
        public void Then_If_Guid_Then_External()
        {
            //Act
            var actual = Guid.NewGuid().ToString().ApimUserType();
            
            //Assert
            actual.Should().Be(ApimUserType.External);
        }
        
        [Test]
        public void Then_If_AlphaNumeric_Then_Employer()
        {
            //Act
            var actual = "ABC123".ApimUserType();
            
            //Assert
            actual.Should().Be(ApimUserType.Employer);
        }
    }
}