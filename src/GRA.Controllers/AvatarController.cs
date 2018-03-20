using GRA.Controllers.ViewModel.Avatar;
using GRA.Domain.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using GRA.Domain.Model;

namespace GRA.Controllers
{
    [Authorize]
    public class AvatarController : Base.UserController
    {
        private readonly ILogger<AvatarController> _logger;
        private readonly AutoMapper.IMapper _mapper;
        private readonly AvatarService _avatarService;
        private readonly UserService _userService;

        public AvatarController(ILogger<AvatarController> logger,
            ServiceFacade.Controller context,
            AvatarService avatarService,
            UserService userService)
            : base(context)
        {
            _logger = Require.IsNotNull(logger, nameof(logger));
            _mapper = context.Mapper;
            _avatarService = Require.IsNotNull(avatarService,
                nameof(avatarService));
            _userService = Require.IsNotNull(userService, nameof(userService));
            PageTitle = "Avatar";
        }

        public async Task<IActionResult> Index(int? id)
        {
            var currentSite = await GetCurrentSiteAsync();
            var userWardrobe = await _avatarService.GetUserWardrobeAsync();
            AvatarJsonModel model = new AvatarJsonModel();
            model.Layers = _mapper
                .Map<ICollection<AvatarJsonModel.AvatarLayer>>(userWardrobe);
            AvatarViewModel viewModel = new AvatarViewModel()
            {
                Layers = userWardrobe,
                GroupIds = userWardrobe.Select(_ => _.GroupId).Distinct(),
                DefaultLayer = userWardrobe.Where(_ => _.DefaultLayer).Select(_ => _.Id).First(),
                ImagePath = _pathResolver.ResolveContentPath($"site{currentSite.Id}/avatars/"),
                AvatarPiecesJson = Newtonsoft.Json.JsonConvert.SerializeObject(model)
            };
            return View(viewModel);
        }

        public async Task<IActionResult> New(int? id)
        {
            var currentSite = await GetCurrentSiteAsync();
            var userWardrobe = await _avatarService.GetUserWardrobeAsync();
            AvatarJsonModel model = new AvatarJsonModel();
            model.Layers = _mapper
                .Map<ICollection<AvatarJsonModel.AvatarLayer>>(userWardrobe);
            AvatarViewModel viewModel = new AvatarViewModel()
            {
                Layers = userWardrobe,
                GroupIds = userWardrobe.Select(_ => _.GroupId).Distinct(),
                DefaultLayer = userWardrobe.Where(_ => _.DefaultLayer).Select(_ => _.Id).First(),
                ImagePath = _pathResolver.ResolveContentPath($"site{currentSite.Id}/avatars/"),
                AvatarPiecesJson = Newtonsoft.Json.JsonConvert.SerializeObject(model)
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> SaveAvatar(string selectionJson)
        {
            try
            {
                var selection = Newtonsoft.Json.JsonConvert
                    .DeserializeObject<ICollection<AvatarLayer>>(selectionJson);
                selection = selection.Where(_ => _.SelectedItem.HasValue).ToList();
                await _avatarService.UpdateUserAvatarAsync(selection);
                return Json(new { success = true });
            }
            catch (GraException gex)
            {
                return Json(new { success = false, message = gex.Message });
            }
        }
    }
}
