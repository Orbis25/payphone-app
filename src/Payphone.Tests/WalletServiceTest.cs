using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Payphone.Application.Dtos.Core;
using Payphone.Application.Dtos.Wallets;
using Payphone.Application.Repositories;
using Payphone.Application.Services.Wallets;
using Payphone.Domain.Enums;
using Payphone.Domain.Models;
using Xunit;

namespace Payphone.Tests;

public class WalletServiceTest
{
     private readonly Mock<IWalletRepository> _walletRepoMock = new();
    private readonly Mock<IWalletTransactionRepository> _transactionRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<ILogger<WalletService>> _loggerMock = new();

    private WalletService CreateService() =>
        new(_walletRepoMock.Object, _transactionRepoMock.Object, _mapperMock.Object, _loggerMock.Object);

    [Fact]
    public async Task CreateAsync_ShouldReturnId_WhenSuccess()
    {
        var walletCode = Guid.NewGuid();
        var input = new CreateOrUpdateWallet { OwnerName = "Test", OwnerDocumentId = "123", CurrentBalance = 100 };
        var wallet = new Wallet { Id = 1, WalletCode = walletCode };
        _mapperMock.Setup(m => m.Map<Wallet>(input)).Returns(wallet);
        _walletRepoMock.Setup(r => r.CreateAsync(wallet, It.IsAny<CancellationToken>())).ReturnsAsync(wallet);

        var service = CreateService();
        var result = await service.CreateAsync(input, CancellationToken.None);

        Assert.Equal(1, result.Data);
        Assert.Null(result.Message);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnError_WhenException()
    {
        var input = new CreateOrUpdateWallet();
        _mapperMock.Setup(m => m.Map<Wallet>(input)).Throws(new Exception("fail"));

        var service = CreateService();
        var result = await service.CreateAsync(input, CancellationToken.None);

        Assert.Contains("Error creating wallet", result.Message);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenWalletExists()
    {
        var input = new CreateOrUpdateWallet { OwnerName = "Test", OwnerDocumentId = "123", CurrentBalance = 100 };
        var wallet = new Wallet { Id = 1 };
        _walletRepoMock.Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<Wallet, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(wallet);

        var service = CreateService();
        var result = await service.UpdateAsync(1, input, CancellationToken.None);

        Assert.Null(result.Message);
        Assert.False(result.IsNotFound);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNotFound_WhenWalletDoesNotExist()
    {
        _walletRepoMock.Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<Wallet, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet)null);

        var service = CreateService();
        var result = await service.UpdateAsync(1, new CreateOrUpdateWallet(), CancellationToken.None);

        Assert.True(result.IsNotFound);
        Assert.Equal("wallet not found", result.Message);
    }

 
    [Fact]
    public async Task CreateTransactionAsync_ShouldReturnNotFound_WhenFromWalletNotFound()
    {
        var input = new CreateWalletTransactionDto { Amount = 100, Type = TransactionType.Debit, ToWalletId = 2 };
        _walletRepoMock.Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<Wallet, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Wallet)null);

        var service = CreateService();
        var result = await service.CreateTransactionAsync(1, input, CancellationToken.None);

        Assert.True(result.IsNotFound);
        Assert.Equal("fromWallet not found", result.Message);
    }

    [Fact]
    public async Task CreateTransactionAsync_ShouldReturnNotFound_WhenToWalletNotFound()
    {
        var fromWallet = new Wallet { Id = 1, CurrentBalance = 200 };
        var input = new CreateWalletTransactionDto { Amount = 100, Type = TransactionType.Debit, ToWalletId = 2 };
        int call = 0;
        _walletRepoMock.Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<Wallet, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => ++call == 1 ? fromWallet : null);

        var service = CreateService();
        var result = await service.CreateTransactionAsync(1, input, CancellationToken.None);

        Assert.True(result.IsNotFound);
        Assert.Equal("ToWallet not found", result.Message);
    }

    [Fact]
    public async Task CreateTransactionAsync_ShouldReturnError_WhenInsufficientBalance()
    {
        var fromWallet = new Wallet { Id = 1, CurrentBalance = 50 };
        var toWallet = new Wallet { Id = 2, CurrentBalance = 0 };
        var input = new CreateWalletTransactionDto { Amount = 100, Type = TransactionType.Debit, ToWalletId = 2 };
        int call = 0;
        _walletRepoMock.Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<Wallet, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => ++call == 1 ? fromWallet : toWallet);

        var service = CreateService();
        var result = await service.CreateTransactionAsync(1, input, CancellationToken.None);

        Assert.Equal("Insufficient balance for this transaction", result.Message);
    }

    [Fact]
    public async Task CreateTransactionAsync_ShouldReturnError_WhenToWalletIdIsNull()
    {
        var fromWallet = new Wallet { Id = 1, CurrentBalance = 200 };
        var toWallet = new Wallet { Id = 2, CurrentBalance = 0 };
        var input = new CreateWalletTransactionDto { Amount = 100, Type = TransactionType.Debit, ToWalletId = null };
        int call = 0;
        _walletRepoMock.Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<Wallet, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => ++call == 1 ? fromWallet : toWallet);

        var service = CreateService();
        var result = await service.CreateTransactionAsync(1, input, CancellationToken.None);

        Assert.Equal("ToWalletId is required for debit transactions", result.Message);
    }

    [Fact]
    public async Task CreateTransactionAsync_ShouldReturnError_WhenToWalletIdEqualsFromWalletId()
    {
        var fromWallet = new Wallet { Id = 1, CurrentBalance = 200 };
        var toWallet = new Wallet { Id = 1, CurrentBalance = 0 };
        var input = new CreateWalletTransactionDto { Amount = 100, Type = TransactionType.Debit, ToWalletId = 1 };
        int call = 0;
        _walletRepoMock.Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<Wallet, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => ++call == 1 ? fromWallet : toWallet);

        var service = CreateService();
        var result = await service.CreateTransactionAsync(1, input, CancellationToken.None);

        Assert.Equal("ToWalletId cannot be the same as FromWalletId for debit transactions", result.Message);
    }

    [Fact]
    public async Task CreateTransactionAsync_ShouldReturnError_WhenException()
    {
        var fromWallet = new Wallet { Id = 1, CurrentBalance = 200 };
        var toWallet = new Wallet { Id = 2, CurrentBalance = 0 };
        var input = new CreateWalletTransactionDto { Amount = 100, Type = TransactionType.Debit, ToWalletId = 2 };
        int call = 0;
        _walletRepoMock.Setup(r => r.GetOneAsync(It.IsAny<Expression<Func<Wallet, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => ++call == 1 ? fromWallet : toWallet);
        _transactionRepoMock.Setup(r => r.Attach(It.IsAny<WalletTransaction>())).Throws(new Exception("fail"));

        var service = CreateService();
        var result = await service.CreateTransactionAsync(1, input, CancellationToken.None);

        Assert.Contains("Error creating transaction", result.Message);
    }

    [Fact]
    public async Task GetTransactionsAsync_ShouldReturnSuccess_WhenWalletExists()
    {
        _walletRepoMock.Setup(r => r.ExistAsync(It.IsAny<Expression<Func<Wallet, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var paginated = new PaginationResult<WalletTransaction> { Results = new List<WalletTransaction>(), ActualPage = 1, Qyt = 1, PageTotal = 1, Total = 1 };
        _transactionRepoMock.Setup(r => r.GetPaginatedListAsync(It.IsAny<Paginate>(), It.IsAny<Expression<Func<WalletTransaction, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginated);
        _mapperMock.Setup(m => m.Map<List<WalletTransactionDto>>(It.IsAny<List<WalletTransaction>>())).Returns(new List<WalletTransactionDto>());

        var service = CreateService();
        var result = await service.GetTransactionsAsync(1, new Paginate(), CancellationToken.None);

        Assert.NotNull(result.Data);
        Assert.Null(result.Message);
    }

    [Fact]
    public async Task GetTransactionsAsync_ShouldReturnNotFound_WhenWalletDoesNotExist()
    {
        _walletRepoMock.Setup(r => r.ExistAsync(It.IsAny<Expression<Func<Wallet, bool>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var service = CreateService();
        var result = await service.GetTransactionsAsync(1, new Paginate(), CancellationToken.None);

        Assert.True(result.IsNotFound);
        Assert.Equal("wallet not found", result.Message);
    }
}