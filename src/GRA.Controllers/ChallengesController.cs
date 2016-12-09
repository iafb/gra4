﻿using GRA.Controllers.ViewModel.Challenges;
using GRA.Controllers.ViewModel.Shared;
using GRA.Domain.Model;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GRA.Controllers
{
    public class ChallengesController : Base.Controller
    {
        private readonly ILogger<ChallengesController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        public readonly ActivityService _activityService;
        private readonly ChallengeService _challengeService;
        public ChallengesController(ILogger<ChallengesController> logger,
            ServiceFacade.Controller context,
            ActivityService activityService,
            ChallengeService challengeService) : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mapper = context.Mapper;
            _activityService = Require.IsNotNull(activityService, nameof(activityService));
            _challengeService = Require.IsNotNull(challengeService, nameof(challengeService));
            PageTitle = "Challenges";
        }

        public async Task<IActionResult> Index(string Search, int page = 1, string sitePath = null)
        {
            int siteId = GetCurrentSiteId(sitePath);
            int take = 15;
            int skip = take * (page - 1);

            var challengeList = await _challengeService.GetPaginatedChallengeListAsync(skip, take);

            PaginateViewModel paginateModel = new PaginateViewModel()
            {
                ItemCount = challengeList.Count,
                CurrentPage = page,
                ItemsPerPage = take
            };

            if (paginateModel.MaxPage > 0 && paginateModel.CurrentPage > paginateModel.MaxPage)
            {
                return RedirectToRoute(
                    new
                    {
                        page = paginateModel.LastPage ?? 1
                    });
            }
            ChallengesListViewModel viewModel = new ChallengesListViewModel()
            {
                Challenges = challengeList.Data,
                PaginateModel = paginateModel,
                Search = Search
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(string Search)
        {
            if (Search != null)
            {
                return RedirectToAction("Index", new { Search = Search });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Detail(int id, string sitePath = null)
        {
            var challenge = await _challengeService.GetChallengeDetailsAsync(id);

            ChallengeDetailViewModel viewModel = new ChallengeDetailViewModel()
            {
                IsAuthenticated = AuthUser.Identity.IsAuthenticated,
                Id = challenge.Id,
                Name = challenge.Name,
                Description = challenge.Description,
                BadgePath = "/favicon-96x96.png",
                Tasks = new List<TaskDetailViewModel>()
            };

            viewModel.Details = $"Completing <strong>{challenge.TasksToComplete} "
                + $"{(challenge.TasksToComplete > 1 ? "Tasks" : "Task")}</strong> will earn: "
                + $"<strong>{challenge.PointsAwarded} "
                + $"{(challenge.PointsAwarded > 1 ? "Points" : "Point")}</strong> and "
                + "<strong>a badge</strong>.";

            foreach (var task in challenge.Tasks)
            {
                TaskDetailViewModel taskModel = new TaskDetailViewModel()
                {
                    Id = task.Id,
                    IsCompleted = task.IsCompleted ?? false,
                    TaskType = task.ChallengeTaskType.ToString(),
                    Url = task.Url
                };
                if (task.ChallengeTaskType.ToString() == "Book")
                {
                    string description = $"Read <strong>{task.Title}</strong>";
                    if (!string.IsNullOrWhiteSpace(task.Author))
                    {
                        description += $" by <strong>{task.Author}</strong>";
                    }
                    taskModel.Description = description;
                }
                else
                {
                    taskModel.Description = task.Title;
                }
                viewModel.Tasks.Add(taskModel);
            }
            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CompleteTasks(ChallengeDetailViewModel model)
        {
            List<ChallengeTask> tasks = _mapper.Map<List<ChallengeTask>>(model.Tasks);
            await _activityService.UpdateChallengeTasks(model.Id, tasks);
            return RedirectToAction("Detail", new { id = model.Id });
        }
    }
}
