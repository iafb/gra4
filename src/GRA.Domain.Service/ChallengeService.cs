﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GRA.Domain.Repository;
using GRA.Domain.Model;

namespace GRA.Domain.Service
{
    public class ChallengeService : Abstract.BaseService<ChallengeService>
    {
        private readonly IChallengeRepository challengeRepository;
        public ChallengeService(ILogger<ChallengeService> logger,
            IChallengeRepository challengeRepository) : base(logger)
        {
            if (challengeRepository == null)
            {
                throw new ArgumentNullException(nameof(challengeRepository));
            }
            this.challengeRepository = challengeRepository;
        }

        /// <summary>
        /// A paginated list of challenges which are visible to the provided user
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="skip">The number of elements to skip before returning the remaining elements</param>
        /// <param name="take">The number of elements to return</param>
        /// <returns><see cref="DataWithCount{DataType}"/> containing the challenges and the total challenge count</returns>
        public DataWithCount<IEnumerable<Challenge>> GetPaginatedChallengeList(User user,
            int skip,
            int take)
        {
            // todo: fix user id
            // todo: add access control - only view authorized challenges
            return new DataWithCount<IEnumerable<Challenge>>
            {
                Data = challengeRepository.GetPagedChallengeList(skip, take),
                Count = challengeRepository.GetChallengeCount()
            };
        }

        /// <summary>
        /// Details on a specific challenge if it's visible to the provided user
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="challengeId">A challenge id</param>
        /// <returns>Details for the requested challenge</returns>
        public Challenge GetChallengeDetails(User user, int challengeId)
        {
            // todo: fix user id
            // todo: add access control - only view authorized challenges
            return challengeRepository.GetById(challengeId);
        }

        /// <summary>
        /// Create a new challenge if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="challenge">A populated challenge object</param>
        /// <returns>The challenge which was added with the Id property populated</returns>
        public Challenge AddChallenge(User user, Challenge challenge)
        {
            // todo: fix user id
            // todo: add access control - only some users can add
            return challengeRepository.AddSave(0, challenge);
        }

        /// <summary>
        /// Edit an existing challenge if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="challenge">The modified challenge object</param>
        /// <returns>The updated challenge</returns>
        public Challenge EditChallenge(User user, Challenge challenge)
        {
            // todo: fix user id
            // todo: add access control - only some users can edit
            return challengeRepository.UpdateSave(0, challenge);
        }

        /// <summary>
        /// Remove an existing challenge if the provided user has rights
        /// </summary>
        /// <param name="user">A valid user</param>
        /// <param name="challenge">The id of the challenge to remove</param>
        public void RemoveChallenge(User user, int challengeId)
        {
            // todo: fix user id
            // todo: add access control - only some users can remove
            challengeRepository.Remove(0, challengeId);
        }
    }
}