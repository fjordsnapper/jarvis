using Jarvis.Api.Controllers;
using Jarvis.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Jarvis.Tests.Controllers;

[TestClass]
public class UsersControllerTests
{
    [TestMethod]
    public void GetUsers_ReturnsOkResult()
    {
        // Arrange
        var controller = new UsersController();

        // Act
        var result = controller.GetUsers();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
    }

    [TestMethod]
    public void GetUser_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var controller = new UsersController();

        // Act
        var result = controller.GetUser(999);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public void CreateUser_WithValidData_ReturnsCreatedAtAction()
    {
        // Arrange
        var controller = new UsersController();
        var request = new CreateUserRequest
        {
            Name = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "555-1234"
        };

        // Act
        var result = controller.CreateUser(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(CreatedAtActionResult));

        var createdResult = result.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);
        Assert.AreEqual(nameof(UsersController.GetUser), createdResult.ActionName);
    }

    [TestMethod]
    public void CreateUser_WithMissingName_ReturnsBadRequest()
    {
        // Arrange
        var controller = new UsersController();
        var request = new CreateUserRequest
        {
            Name = "",
            Email = "john@example.com"
        };

        // Act
        var result = controller.CreateUser(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void CreateUser_WithMissingEmail_ReturnsBadRequest()
    {
        // Arrange
        var controller = new UsersController();
        var request = new CreateUserRequest
        {
            Name = "John Doe",
            Email = ""
        };

        // Act
        var result = controller.CreateUser(request);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void CreateUser_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var controller = new UsersController();
        var request1 = new CreateUserRequest
        {
            Name = "John Doe",
            Email = "john@example.com"
        };
        var request2 = new CreateUserRequest
        {
            Name = "Jane Doe",
            Email = "john@example.com"
        };

        // Act
        controller.CreateUser(request1);
        var result = controller.CreateUser(request2);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public void UpdateUser_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var controller = new UsersController();
        var createRequest = new CreateUserRequest
        {
            Name = "John Doe",
            Email = "john@example.com"
        };
        var createResult = controller.CreateUser(createRequest);
        var createdUser = ((CreatedAtActionResult)createResult.Result).Value as User;

        var updateRequest = new UpdateUserRequest
        {
            Name = "Jane Doe",
            Email = "jane@example.com"
        };

        // Act
        var result = controller.UpdateUser(createdUser!.Id, updateRequest);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

        var okResult = result.Result as OkObjectResult;
        var updatedUser = okResult?.Value as User;
        Assert.IsNotNull(updatedUser);
        Assert.AreEqual("Jane Doe", updatedUser.Name);
        Assert.AreEqual("jane@example.com", updatedUser.Email);
    }

    [TestMethod]
    public void UpdateUser_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var controller = new UsersController();
        var updateRequest = new UpdateUserRequest
        {
            Name = "Jane Doe"
        };

        // Act
        var result = controller.UpdateUser(999, updateRequest);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public void DeleteUser_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var controller = new UsersController();
        var createRequest = new CreateUserRequest
        {
            Name = "John Doe",
            Email = "john@example.com"
        };
        var createResult = controller.CreateUser(createRequest);
        var createdUser = ((CreatedAtActionResult)createResult.Result).Value as User;

        // Act
        var result = controller.DeleteUser(createdUser!.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
    }

    [TestMethod]
    public void DeleteUser_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var controller = new UsersController();

        // Act
        var result = controller.DeleteUser(999);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public void DeleteUser_ThenGetUser_ReturnsNotFound()
    {
        // Arrange
        var controller = new UsersController();
        var createRequest = new CreateUserRequest
        {
            Name = "John Doe",
            Email = "john@example.com"
        };
        var createResult = controller.CreateUser(createRequest);
        var createdUser = ((CreatedAtActionResult)createResult.Result).Value as User;

        // Act
        controller.DeleteUser(createdUser!.Id);
        var getResult = controller.GetUser(createdUser.Id);

        // Assert
        Assert.IsNotNull(getResult);
        Assert.IsInstanceOfType(getResult.Result, typeof(NotFoundObjectResult));
    }
}
