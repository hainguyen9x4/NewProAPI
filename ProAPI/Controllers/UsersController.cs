using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pro.Common.Account;
using Pro.Model;
using Pro.Service;
using System.Security.Claims;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace xStory.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        [Route("api/[controller]/AddUser")]
        [HttpPost]
        public ActionResult<bool> AddUser(User user) =>
            _userService.AddNewUser(user);

        [Route("api/[controller]/UpdateLevelInfoUser1")]
        [HttpPost]
        public ActionResult<User> UpdateLevelInfoUser(int userID, int percent) =>
            _userService.UpdateLevelInfoUser(userID, percent);

        [Route("api/[controller]/UpdateLevelInfoUser")]
        [HttpPost]
        public ActionResult<User> UpdateLevelInfoUser(int userID, LevelUser level) =>
            _userService.UpdateLevelInfoUser(userID, level);

        [AllowAnonymous]
        [HttpPost("api/[controller]/Login")]
        public ActionResult Login(UserRequest req)
        {
            var resp = _userService.UserLogin(req);

            var ret = resp == null ? (ActionResult)Unauthorized("User not found.") : Ok(resp);

            return ret;
        }

        [Authorize]
        [HttpPost("api/[controller]/Logout")]
        public ActionResult Logout()
        {
            var token = GetToken();
            _userService.Logout(token);

            return Ok();
        }

        [Authorize]
        [Route("api/[controller]/GetUserInfo")]
        [HttpGet]
        public ActionResult<User> GetUserInfo(int userID, string accName = "") =>
            _userService.GetUser(userID, accName);

        [Authorize]
        [Route("api/[controller]/DeleteFollowStoryInfoUser")]
        [HttpPost]
        public ActionResult<User> DeleteFollowStoryInfoUser(int userID, int storyID) =>
            _userService.DeleteFollowStoryInfoUser(userID, storyID);

        [Authorize]
        [Route("api/[controller]/AddFollowStoryInfoUser")]
        [HttpPost]
        public ActionResult<User> AddFollowStoryInfoUser(int userID, int storyID) =>
            _userService.AddFollowStoryInfoUser(userID, storyID);

        [Authorize]
        [Route("api/[controller]/UpdateBasicInfoUser")]
        [HttpPost]
        public ActionResult<User> UpdateBasicInfoUser(User user) =>
            _userService.UpdateBasicInfoUser(user);

        private string GetToken()
        {
            var claims = User.Identity as ClaimsIdentity;
            var token = claims.FindFirst("token").Value;

            return token;
        }

    }
}