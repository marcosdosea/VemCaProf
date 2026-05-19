using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.EntityFrameworkCore;
using Core;
using Core.DTO;
using Core.Service;
using Service;

namespace ServiceTests;

[TestClass]
public class AulaServiceTests
{
    [TestMethod]
    public void Update_NullDto_ThrowsServiceException()
    {
        // Arrange
        var mockContext = new Mock<VemCaProfContext>();
        var service = new AulaService(mockContext.Object);

        // Act & Assert
        var ex = Assert.ThrowsException<ServiceException>(() => service.Update(null!));
        Assert.AreEqual("Dados da aula não podem ser nulos", ex.Message);
    }

    [TestMethod]
    public void Update_MinValueStartDate_ThrowsServiceException()
    {
        // Arrange
        var mockContext = new Mock<VemCaProfContext>();
        var service = new AulaService(mockContext.Object);
        var dto = new AulaDTO { Id = 1, DataHorarioInicio = DateTime.MinValue, DataHorarioFinal = DateTime.Now, Descricao = "desc", Valor = 1, IdDisciplina = 1, IdResponsavel = 1, IdAluno = 1, IdProfessor = 1 };

        // Act & Assert
        var ex = Assert.ThrowsException<ServiceException>(() => service.Update(dto));
        Assert.AreEqual("campo obrigatório", ex.Message);
    }

    [TestMethod]
    public void Update_MinValueEndDate_ThrowsServiceException()
    {
        // Arrange
        var mockContext = new Mock<VemCaProfContext>();
        var service = new AulaService(mockContext.Object);
        var dto = new AulaDTO { Id = 1, DataHorarioInicio = DateTime.Now, DataHorarioFinal = DateTime.MinValue, Descricao = "desc", Valor = 1, IdDisciplina = 1, IdResponsavel = 1, IdAluno = 1, IdProfessor = 1 };

        // Act & Assert
        var ex = Assert.ThrowsException<ServiceException>(() => service.Update(dto));
        Assert.AreEqual("campo obrigatório", ex.Message);
    }

    [TestMethod]
    public void Update_EmptyDescription_ThrowsServiceException()
    {
        // Arrange
        var mockContext = new Mock<VemCaProfContext>();
        var service = new AulaService(mockContext.Object);
        var dto = new AulaDTO { Id = 1, DataHorarioInicio = DateTime.Now, DataHorarioFinal = DateTime.Now.AddHours(1), Descricao = "   ", Valor = 1, IdDisciplina = 1, IdResponsavel = 1, IdAluno = 1, IdProfessor = 1 };

        // Act & Assert
        var ex = Assert.ThrowsException<ServiceException>(() => service.Update(dto));
        Assert.AreEqual("A descrição da aula é obrigatório", ex.Message);
    }

    [TestMethod]
    public void Update_NonPositiveValor_ThrowsServiceException()
    {
        // Arrange
        var mockContext = new Mock<VemCaProfContext>();
        var service = new AulaService(mockContext.Object);
        var dto = new AulaDTO { Id = 1, DataHorarioInicio = DateTime.Now, DataHorarioFinal = DateTime.Now.AddHours(1), Descricao = "desc", Valor = 0, IdDisciplina = 1, IdResponsavel = 1, IdAluno = 1, IdProfessor = 1 };

        // Act & Assert
        var ex = Assert.ThrowsException<ServiceException>(() => service.Update(dto));
        Assert.AreEqual("Valor maior que 0", ex.Message);
    }

    [TestMethod]
    public void Update_IdDisciplinaInvalid_ThrowsServiceException()
    {
        // Arrange
        var mockContext = new Mock<VemCaProfContext>();
        var service = new AulaService(mockContext.Object);
        var dto = new AulaDTO { Id = 1, DataHorarioInicio = DateTime.Now, DataHorarioFinal = DateTime.Now.AddHours(1), Descricao = "desc", Valor = 1, IdDisciplina = 0, IdResponsavel = 1, IdAluno = 1, IdProfessor = 1 };

        // Act & Assert
        var ex = Assert.ThrowsException<ServiceException>(() => service.Update(dto));
        Assert.AreEqual("campo obrigatório", ex.Message);
    }

    [TestMethod]
    public void Update_IdResponsavelInvalid_ThrowsServiceException()
    {
        // Arrange
        var mockContext = new Mock<VemCaProfContext>();
        var service = new AulaService(mockContext.Object);
        var dto = new AulaDTO { Id = 1, DataHorarioInicio = DateTime.Now, DataHorarioFinal = DateTime.Now.AddHours(1), Descricao = "desc", Valor = 1, IdDisciplina = 1, IdResponsavel = 0, IdAluno = 1, IdProfessor = 1 };

        // Act & Assert
        var ex = Assert.ThrowsException<ServiceException>(() => service.Update(dto));
        Assert.AreEqual("campo obrigatório", ex.Message);
    }

    [TestMethod]
    public void Update_IdAlunoInvalid_ThrowsServiceException()
    {
        // Arrange
        var mockContext = new Mock<VemCaProfContext>();
        var service = new AulaService(mockContext.Object);
        var dto = new AulaDTO { Id = 1, DataHorarioInicio = DateTime.Now, DataHorarioFinal = DateTime.Now.AddHours(1), Descricao = "desc", Valor = 1, IdDisciplina = 1, IdResponsavel = 1, IdAluno = 0, IdProfessor = 1 };

        // Act & Assert
        var ex = Assert.ThrowsException<ServiceException>(() => service.Update(dto));
        Assert.AreEqual("campo obrigatório", ex.Message);
    }

    [TestMethod]
    public void Update_IdProfessorInvalid_ThrowsServiceException()
    {
        // Arrange
        var mockContext = new Mock<VemCaProfContext>();
        var service = new AulaService(mockContext.Object);
        var dto = new AulaDTO { Id = 1, DataHorarioInicio = DateTime.Now, DataHorarioFinal = DateTime.Now.AddHours(1), Descricao = "desc", Valor = 1, IdDisciplina = 1, IdResponsavel = 1, IdAluno = 1, IdProfessor = 0 };

        // Act & Assert
        var ex = Assert.ThrowsException<ServiceException>(() => service.Update(dto));
        Assert.AreEqual("campo obrigatório", ex.Message);
    }

    [TestMethod]
    public void Update_ValidDto_UpdatesAulaAndReturnsTrue()
    {
        // Arrange
        var mockSet = new Mock<DbSet<Aula>>();
        var aula = new Aula { Id = 42, DataHorarioInicio = DateTime.Now.AddDays(-1), DataHorarioFinal = DateTime.Now, Descricao = "old", Status = Core.Enums.StatusEnum.Agendada, Valor = 10, MetodoPagamento = null, IdDisciplina = 2, IdResponsavel = 2, IdAluno = 2, IdProfessor = 2 };

        mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns<object[]>(ids => (int)ids[0] == aula.Id ? aula : null);
        mockSet.Setup(m => m.Update(It.IsAny<Aula>())).Returns<Aula>(a => null!);

        var mockContext = new Mock<VemCaProfContext>();
        mockContext.Setup(c => c.Aulas).Returns(mockSet.Object);
        mockContext.Setup(c => c.SaveChanges()).Returns(1);

        var service = new AulaService(mockContext.Object);

        var dto = new AulaDTO
        {
            Id = 42,
            DataHorarioInicio = DateTime.Today,
            DataHorarioFinal = DateTime.Today.AddHours(1),
            Descricao = " new desc ",
            Status = Core.Enums.StatusEnum.Paga,
            Valor = 50,
            MetodoPagamento = "P",
            IdDisciplina = 3,
            IdResponsavel = 4,
            IdAluno = 5,
            IdProfessor = 6
        };

        // Act
        var result = service.Update(dto);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(dto.DataHorarioInicio, aula.DataHorarioInicio);
        Assert.AreEqual(dto.DataHorarioFinal, aula.DataHorarioFinal);
        Assert.AreEqual("new desc", aula.Descricao);
        Assert.AreEqual(dto.Status, aula.Status);
        Assert.AreEqual(dto.Valor, aula.Valor);
        Assert.AreEqual(dto.MetodoPagamento, aula.MetodoPagamento);
        Assert.AreEqual(dto.IdDisciplina, aula.IdDisciplina);
        Assert.AreEqual(dto.IdResponsavel, aula.IdResponsavel);
        Assert.AreEqual(dto.IdAluno, aula.IdAluno);
        Assert.AreEqual(dto.IdProfessor, aula.IdProfessor);

        mockSet.Verify(m => m.Update(aula), Times.Once);
        mockContext.Verify(c => c.SaveChanges(), Times.Once);
    }

    [TestMethod]
    public void Update_FindThrows_WrapsExceptionInServiceException()
    {
        // Arrange
        var mockSet = new Mock<DbSet<Aula>>();
        mockSet.Setup(m => m.Find(It.IsAny<object[]>())).Throws(new Exception("db fail"));

        var mockContext = new Mock<VemCaProfContext>();
        mockContext.Setup(c => c.Aulas).Returns(mockSet.Object);

        var service = new AulaService(mockContext.Object);

        var dto = new AulaDTO
        {
            Id = 99,
            DataHorarioInicio = DateTime.Today,
            DataHorarioFinal = DateTime.Today.AddHours(1),
            Descricao = "desc",
            Valor = 10,
            IdDisciplina = 1,
            IdResponsavel = 1,
            IdAluno = 1,
            IdProfessor = 1
        };

        // Act
        var ex = Assert.ThrowsException<ServiceException>(() => service.Update(dto));

        // Assert
        Assert.IsTrue(ex.Message.Contains("Erro ao atualizar aula ID 99"));
        Assert.IsNotNull(ex.InnerException);
        Assert.AreEqual("db fail", ex.InnerException!.Message);
    }

    // Helpers are allowed as inner classes only
}
