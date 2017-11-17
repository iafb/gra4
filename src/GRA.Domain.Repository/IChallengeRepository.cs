using GRA.Domain.Model;
using GRA.Domain.Model.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Domain.Repository
{
    public interface IChallengeRepository : IRepository<Challenge>
    {
        Task<int> GetChallengeCountAsync(BaseFilter filter);
        Task<IEnumerable<ChallengeTask>> GetChallengeTasksAsync(int challengeId, int? userId);
        new Task<Challenge> GetByIdAsync(int id);
        Task<Challenge> GetActiveByIdAsync(int id, int? userId = default(int));
        Task<ICollection<Challenge>> PageAllAsync(BaseFilter filter);
        Task<DataWithCount<IEnumerable<int>>> PageIdsAsync(BaseFilter filter, int userId);
        Task<IEnumerable<ChallengeTaskUpdateStatus>>
            UpdateUserChallengeTasksAsync(int userId, IEnumerable<ChallengeTask> challengeTasks);
        Task UpdateUserChallengeTaskAsync(
            int userId,
            int challengeTaskId,
            int? userLogId,
            int? bookId);
        Task<ActivityLogResult> GetUserChallengeTaskResultAsync(int userId, int challengeTaskId);
        Task SetValidationAsync(int userId, int challengeId, bool valid);
        Task<bool> HasDependentsAsync(int challengeId);
        Task<Challenge> UpdateSaveAsync(int currentUserId, Challenge challenge, 
            List<int> categoriesToAdd, List<int> categoriesToRemove);
    }
}