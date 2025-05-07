﻿using DataLayer;

namespace RaelState.Assistant;

public class TokenCleanupTask
{
    public async Task ExecuteAsync(IUnitOfWork unitOfWork)
    {
        var tokens = await unitOfWork.TokenBlacklistRepository.GetExpiredTokensAsync();
        if (tokens != null)
        {
            unitOfWork.TokenBlacklistRepository.RemoveRange(tokens);
            await unitOfWork.CommitAsync();
        }
    }
}
