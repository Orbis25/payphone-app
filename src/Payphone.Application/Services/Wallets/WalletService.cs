using AutoMapper;
using Payphone.Application.Dtos.Wallets;
using Payphone.Application.Extensions;
using Payphone.Application.Repositories;
using Payphone.Application.Services.Core;
using Payphone.Domain.Enums;

namespace Payphone.Application.Services.Wallets;

public class WalletService : BaseService<Wallet, WalletDto, IWalletRepository>, IWalletService
{
    private readonly IWalletRepository _repository;
    private readonly IWalletTransactionRepository _transactionRepository;
    private readonly ILogger<WalletService> _logger;
    private readonly IMapper _mapper;

    public WalletService(IWalletRepository repository,
        IWalletTransactionRepository transactionRepository,
        IMapper mapper,
        ILogger<WalletService> logger) : base(repository,
        mapper, logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
        _transactionRepository = transactionRepository;
    }

    public async Task<Response<int>> CreateAsync(CreateOrUpdateWallet input, CancellationToken cancellationToken)
    {
        try
        {
            var data = _mapper.Map<Wallet>(input);
            _logger.LogInformation("Creating wallet with code: {code}", data.WalletCode);

            var result = await _repository.CreateAsync(data, cancellationToken);

            return new(result.Id);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating wallet: {Message}", e.Message);

            return new("Error creating wallet, please try again later.");
        }
    }

    public async Task<Response> UpdateAsync(int id, CreateOrUpdateWallet input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating wallet with id {Id}", id);
            var account = await _repository.GetOneAsync(x => x.Id == id, cancellationToken);

            if (account == null)
            {
                _logger.LogWarning("wallet with Id {Id} not found", id);
                return new() { Message = "wallet not found", IsNotFound = true };
            }


            account.OwnerDocumentId = input.OwnerDocumentId;
            account.OwnerName = input.OwnerName;
            account.CurrentBalance = input.CurrentBalance;

            await _repository.UpdateAsync(account, cancellationToken).ConfigureAwait(false);

            return new();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error updating wallet: {Message}", e.Message);
            return new() { Message = "Error updating wallet, please try again" };
        }
    }

    public async Task<Response<CreateWalletTransactionResultDto>> CreateTransactionAsync(int walletId,
        CreateWalletTransactionDto input,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating transaction for wallet {WalletId}", walletId);
            var fromWallet = await _repository.GetOneAsync(x => x.Id == walletId, cancellationToken);

            if (fromWallet is null)
            {
                _logger.LogWarning("wallet with Id {wallet} not found", walletId);
                return new("fromWallet not found") { IsNotFound = true };
            }

            var toWallet = await _repository.GetOneAsync(x => x.Id == input.ToWalletId , cancellationToken);

            if (toWallet is null)
            {
                _logger.LogWarning("to wallet with Id {toWalletId} not found", input.ToWalletId);
                return new("ToWallet not found") { IsNotFound = true };
            }

            var isDebit = input.Type == TransactionType.Debit;

            var validationResult = ValidateTransaction(isDebit, fromWallet, toWallet, input);

            if (validationResult != null)
            {
                return validationResult;
            }

            _logger.LogInformation("Transaction validation passed for wallet {WalletId}", walletId);
            

            switch (isDebit)
            {
                case true:
                    fromWallet.CurrentBalance -= input.Amount;
                    toWallet.CurrentBalance += input.Amount;
                    break;
                case false:
                    fromWallet.CurrentBalance += input.Amount;
                    break;
            }

            var data = new WalletTransaction
            {
                FromWalletId = walletId,
                ToWalletId = isDebit ? input.ToWalletId : null,
                Amount = input.Amount,
                Type = input.Type,
                CurrentWalletBalance = fromWallet.CurrentBalance,
            };

            _logger.LogInformation("Creating transaction for wallet {WalletId} with amount {Amount} and type {Type}",
                walletId, input.Amount, input.Type);

            _transactionRepository.Attach(data);

            await _repository.CommitAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Transaction created successfully for wallet {WalletId}", walletId);

            return new()
            {
                Data = new()
                {
                    TransactionId = data.Id
                }
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, " Error creating transaction for wallet {WalletId}: {Message}", walletId, e.Message);
            return new("Error creating transaction, please try again later.");
        }
    }

    public async Task<Response<PaginationResult<WalletTransactionDto>>> GetTransactionsAsync(int walletId,
        Paginate paginate, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting transactions for wallet {WalletId}", walletId);
            var wallet = await _repository.ExistAsync(x => x.Id == walletId, cancellationToken);

            if (!wallet)
            {
                _logger.LogWarning("wallet with Id {walletId} not found", walletId);
                return new() { Message = "wallet not found", IsNotFound = true };
            }

            var transactions = await _transactionRepository.GetPaginatedListAsync(paginate,
                x => x.FromWalletId == walletId || x.ToWalletId == walletId, cancellationToken);

            var result = new PaginationResult<WalletTransactionDto>
            {
                ActualPage = transactions.ActualPage,
                Qyt = transactions.Qyt,
                PageTotal = transactions.PageTotal,
                Total = transactions.Total,
                Results = _mapper.Map<List<WalletTransactionDto>>(transactions.Results)
            };

            return new(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting transactions for wallet {WalletId}: {Message}", walletId, e.Message);
            return new("Error getting transactions, please try again later.");
        }
    }
    
    
    private Response<CreateWalletTransactionResultDto>? ValidateTransaction(
        bool isDebit,
        Wallet fromWallet,
        Wallet toWallet,
        CreateWalletTransactionDto input)
    {
        //if is a debit transaction, check if the account has sufficient balance
        if ((fromWallet.CurrentBalance <= 0 || fromWallet.CurrentBalance < input.Amount) && isDebit)
        {
            _logger.LogWarning("wallet with Id {walletId} has insufficient balance", fromWallet.Id);
            return new("Insufficient balance for this transaction");
        }

        if (isDebit && input.ToWalletId == null)
        {
            _logger.LogWarning("ToWalletId is required for debit transactions");
            return new("ToWalletId is required for debit transactions");
        }

        if (isDebit && input.ToWalletId == fromWallet.Id)
        {
            _logger.LogWarning("ToWalletId cannot be the same as FromWalletId for debit transactions");
            return new("ToWalletId cannot be the same as FromWalletId for debit transactions");
        }

        return null;
    }
}