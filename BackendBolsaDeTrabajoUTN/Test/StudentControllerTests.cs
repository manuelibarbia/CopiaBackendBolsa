using BackendBolsaDeTrabajoUTN.Controllers;
using BackendBolsaDeTrabajoUTN.Data.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Security.Claims;
using Xunit;

namespace BackendBolsaDeTrabajoUTN.Tests.Controllers
{
    public class StudentControllerTests
    {
        [Fact]
        public void DeleteStudent_ValidId_ReturnsOk()
        {
           
            var studentRepositoryMock = new Mock<IStudentRepository>();
            var controller = new StudentController(studentRepositoryMock.Object, null, null, null);

            var studentId = 4;

            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, studentId.ToString())
            }));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            
            var result = controller.DeleteStudent();

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Alumno borrado del sistema.", okResult.Value);

            studentRepositoryMock.Verify(repo => repo.RemoveStudent(studentId), Times.Once);
        }

        [Fact]
        public void DeleteStudent_Exception_ReturnsProblem()
        {
            
            var studentRepositoryMock = new Mock<IStudentRepository>();
            studentRepositoryMock.Setup(repo => repo.RemoveStudent(It.IsAny<int>())).Throws(new Exception("Error al eliminar el alumno."));
            var controller = new StudentController(studentRepositoryMock.Object, null, null, null);

           
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "1")
            }));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            
            var result = controller.DeleteStudent();

            
            var problemResult = Assert.IsType<ObjectResult>(result);
            var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
            Assert.Equal("Error al eliminar el alumno.", problemDetails.Detail);

            studentRepositoryMock.Verify(repo => repo.RemoveStudent(It.IsAny<int>()), Times.Once);
        }

    }
}
