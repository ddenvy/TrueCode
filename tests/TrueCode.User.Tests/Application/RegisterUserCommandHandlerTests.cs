using FluentAssertions;
using Moq;
using TrueCode.User.Application.Commands.RegisterUser;
using TrueCode.User.Application.DTOs;
using TrueCode.User.Domain.Entities;
using TrueCode.User.Domain.Interfaces;
using TrueCode.Shared.Infrastructure.Services;
using TrueCode.Shared.Domain.Interfaces;

namespace TrueCode.User.Tests.Application;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHashService> _passwordHashServiceMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHashServiceMock = new Mock<IPasswordHashService>();
        _jwtServiceMock = new Mock<IJwtService>();
        _handler = new RegisterUserCommandHandler(
            _userRepositoryMock.Object, 
            _unitOfWorkMock.Object,
            _passwordHashServiceMock.Object,
            _jwtServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenValidCommand()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "password123");
        var hashedPassword = "hashed_password123";
        var userId = Guid.NewGuid();
        var token = "jwt_token_123";

        _userRepositoryMock.Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((TrueCode.User.Domain.Entities.User?)null);
        _passwordHashServiceMock.Setup(x => x.HashPassword(command.Password))
                               .Returns(hashedPassword);
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TrueCode.User.Domain.Entities.User>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync((TrueCode.User.Domain.Entities.User user, CancellationToken ct) => user);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ReturnsAsync(1);
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<Guid>(), command.Name))
                      .Returns(token);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be(token);
        result.User.Should().NotBeNull();
        result.User.Name.Should().Be(command.Name);

        _userRepositoryMock.Verify(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()), Times.Once);
        _passwordHashServiceMock.Verify(x => x.HashPassword(command.Password), Times.Once);
        _userRepositoryMock.Verify(x => x.AddAsync(It.Is<TrueCode.User.Domain.Entities.User>(u => u.Name == command.Name && u.Password == hashedPassword), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenUserAlreadyExists()
    {
        // Arrange
        var command = new RegisterUserCommand("existinguser", "password123");
        var existingUser = TrueCode.User.Domain.Entities.User.Create("existinguser", "hashedpassword");

        _userRepositoryMock.Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(existingUser);

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("*уже существует*");

        _userRepositoryMock.Verify(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()), Times.Once);
        _passwordHashServiceMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TrueCode.User.Domain.Entities.User>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenRepositoryFails()
    {
        // Arrange
        var command = new RegisterUserCommand("testuser", "password123");
        var hashedPassword = "hashed_password123";

        _userRepositoryMock.Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((TrueCode.User.Domain.Entities.User?)null);
        _passwordHashServiceMock.Setup(x => x.HashPassword(command.Password))
                               .Returns(hashedPassword);
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TrueCode.User.Domain.Entities.User>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync((TrueCode.User.Domain.Entities.User user, CancellationToken ct) => user);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var act = async () => await _handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<Exception>()
                 .WithMessage("Database error");

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}