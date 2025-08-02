using AutoMapper;
using Payphone.Application.Dtos.Wallets;
using Payphone.Application.Repositories;
using Payphone.Application.Services.Core;

namespace Payphone.Application.Services.Wallets;

public class WalletService : BaseService<Wallet, WalletDto, IWalletRepository>, IWalletService
{
    private readonly IWalletRepository _repository;
    private readonly ILogger<WalletService> _logger;
    private readonly IMapper _mapper;

    public WalletService(IWalletRepository repository, IMapper mapper, ILogger<WalletService> logger) : base(repository,
        mapper, logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
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
    
    public async Task<Response> UpdateAsync(int id, CreateOrUpdateWallet input, CancellationToken cancellationToken = default)
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

}