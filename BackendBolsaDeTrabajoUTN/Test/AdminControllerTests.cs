using BackendBolsaDeTrabajoUTN.Controllers;
using BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces;
using BackendBolsaDeTrabajoUTN.Entities;
using BackendBolsaDeTrabajoUTN.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace BackendBolsaDeTrabajoUTN.Tests.Controllers
{
    public class AdminControllerTests
    {
        [Fact]
        public void CreateCareer_WithValidRequest_ReturnsCreatedResult()
        {
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var knowledgeRepositoryMock = new Mock<IKnowledgeRepository>();
            var adminController = new AdminController(adminRepositoryMock.Object, knowledgeRepositoryMock.Object);

            var request = new AddCareerRequest
            {
                CareerName = "Computer Science",
                CareerAbbreviation = "CS",
                CareerType = "Engineering",
                CareerTotalSubjects = 10
            };

            
            var userClaims = new[]
            {
                new Claim("userType", "Admin")
            };
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(userClaims));
            adminController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userPrincipal
                }
            };

            adminRepositoryMock.Setup(repo => repo.GetCareers()).Returns(new List<Career>());
            adminRepositoryMock.Setup(repo => repo.CreateCareer(It.IsAny<Career>()));

            
            var result = adminController.CreateCareer(request);

            
            var createdResult = Assert.IsType<CreatedResult>(result);
            var careerResponse = Assert.IsType<CareerResponse>(createdResult.Value);

            Assert.Equal(request.CareerName, careerResponse.CareerName);
            Assert.Equal(request.CareerAbbreviation, careerResponse.CareerAbbreviation);
            Assert.Equal(request.CareerType, careerResponse.CareerType);
            Assert.Equal(request.CareerTotalSubjects, careerResponse.CareerTotalSubjects);
            
        }
    }
}

