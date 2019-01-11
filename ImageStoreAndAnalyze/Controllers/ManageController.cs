using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ImageStoreAndAnalyze.Models;
using ImageStoreAndAnalyze.Models.ManageViewModels;
using ImageStoreAndAnalyze.Services;
using System.Security.Claims;
using ImageStoreAndAnalyze.Models.FamilyAccountViewModels;
using ImageStoreAndAnalyze.Interfaces.Services;
using ImageProcess.Models;
using Microsoft.AspNetCore.Http;
using ImageStoreAndAnalyze.Utilities;
using System.Net;
using System.IO;
using ImageStoreAndAnalyze.Data.DatabaseServices;
using System.Runtime.Serialization.Json;
using ImageStoreAndAnalyze.Interfaces;

namespace ImageStoreAndAnalyze.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;
        private readonly ILogger logger;
        private readonly UrlEncoder urlEncoder;
        private readonly IServiceProvider serviceProvider;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          IServiceProvider serviceProvider)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
            this.logger = logger;
            this.urlEncoder = urlEncoder;
            this.serviceProvider = serviceProvider;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            IList<Claim> userClaims = await userManager.GetClaimsAsync(user);
            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                FirstName = (userClaims == null || userClaims.Count <= 0 ? "" : userClaims.FirstOrDefault(uc => uc.Type == ClaimTypes.GivenName)?.Value),
                LastName = (userClaims == null || userClaims.Count <= 0 ? "" : userClaims.FirstOrDefault(uc => uc.Type == ClaimTypes.Surname)?.Value),
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var claimsToAdd = new List<Claim>();

            var firstName = userClaims?.FirstOrDefault(uc => uc.Type == ClaimTypes.GivenName)?.Value;
            if (model.FirstName != firstName)
            {
                var claim = userClaims?.FirstOrDefault(uc => uc.Type == ClaimTypes.GivenName);
                if (claim != null)
                    await userManager.RemoveClaimAsync(user, claim);

                claimsToAdd.Add(new Claim(ClaimTypes.GivenName, model.FirstName));
            }

            var lastName = userClaims?.FirstOrDefault(uc => uc.Type == ClaimTypes.Surname)?.Value;
            if (model.LastName != lastName)
            {
                var claim = userClaims?.FirstOrDefault(uc => uc.Type == ClaimTypes.Surname);
                if (claim != null)
                    await userManager.RemoveClaimAsync(user, claim);

                claimsToAdd.Add(new Claim(ClaimTypes.Surname, model.LastName));
            }

            var addClaimsResult = await userManager.AddClaimsAsync(user, claimsToAdd);

            StatusMessage = "Your profile has been updated";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var hasPassword = await userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction(nameof(SetPassword));
            }

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            logger.LogInformation("User changed their password successfully.");
            StatusMessage = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var hasPassword = await userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction(nameof(ChangePassword));
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var addPasswordResult = await userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                AddErrors(addPasswordResult);
                return View(model);
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "Your password has been set.";

            return RedirectToAction(nameof(SetPassword));
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await userManager.GetLoginsAsync(user) };
            model.OtherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();
            model.ShowRemoveButton = await userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var info = await signInManager.GetExternalLoginInfoAsync(user.Id);
            if (info == null)
            {
                throw new ApplicationException($"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            var result = await userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            StatusMessage = "The external login was added.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(RemoveLoginViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var result = await userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if (!result.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occurred removing external login for user with ID '{user.Id}'.");
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            StatusMessage = "The external login was removed.";
            return RedirectToAction(nameof(ExternalLogins));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user),
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            return View(nameof(Disable2fa));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var disable2faResult = await userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new ApplicationException($"Unexpected error occured disabling 2FA for user with ID '{user.Id}'.");
            }

            logger.LogInformation("User with ID {UserId} has disabled 2fa.", user.Id);
            return RedirectToAction(nameof(TwoFactorAuthentication));
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            var model = new EnableAuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync(user, model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(
                user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Code", "Verification code is invalid.");
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return View(model);
            }

            await userManager.SetTwoFactorEnabledAsync(user, true);
            logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id);
            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData[RecoveryCodesKey] = recoveryCodes.ToArray();

            return RedirectToAction(nameof(ShowRecoveryCodes));
        }

        [HttpGet]
        public IActionResult ShowRecoveryCodes()
        {
            var recoveryCodes = (string[])TempData[RecoveryCodesKey];
            if (recoveryCodes == null)
            {
                return RedirectToAction(nameof(TwoFactorAuthentication));
            }

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes };
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View(nameof(ResetAuthenticator));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            await userManager.SetTwoFactorEnabledAsync(user, false);
            await userManager.ResetAuthenticatorKeyAsync(user);
            logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", user.Id);

            return RedirectToAction(nameof(EnableAuthenticator));
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodesWarning()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' because they do not have 2FA enabled.");
            }

            return View(nameof(GenerateRecoveryCodes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            if (!user.TwoFactorEnabled)
            {
                throw new ApplicationException($"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled.");
            }

            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", user.Id);

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            return View(nameof(ShowRecoveryCodes), model);
        }


        [HttpGet]
        public async Task<IActionResult> FamiliesManagement()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            IFamilyDatabaseService familyDatabaseService = serviceProvider.GetService(typeof(IFamilyDatabaseService)) as IFamilyDatabaseService;
            var model = new FamiliesManagementViewModel
            {
                StatusMessage = StatusMessage,
                FamiliesAdminOf = familyDatabaseService.GetUserAdminFamiliesWithMainImage(user),
                FamiliesMemberOf = familyDatabaseService.GetUserFamiliesMemberOfWithMainImage(user)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FamiliesManagement(FamiliesManagementViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            //}

            //var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            //if (!changePasswordResult.Succeeded)
            //{
            //    AddErrors(changePasswordResult);
            //    return View(model);
            //}

            //await _signInManager.SignInAsync(user, isPersistent: false);
            logger.LogInformation("User changed family details succesfully.");
            StatusMessage = "Your changes were made successfully.";

            return RedirectToAction(nameof(FamiliesManagement));
        }

        public async Task<IActionResult> ProposeNewFamilyAdmin(string userSecurityStamp, Guid guid)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            IFamilyDatabaseService familyDatabaseService = serviceProvider.GetService(typeof(IFamilyDatabaseService)) as IFamilyDatabaseService;
            var model = new FamiliesManagementViewModel
            {
                StatusMessage = StatusMessage,
                FamiliesAdminOf = familyDatabaseService.GetUserAdminFamiliesWithMainImage(user),
                FamiliesMemberOf = familyDatabaseService.GetUserFamiliesMemberOfWithMainImage(user)
            };

            try
            {
                IUser newAdmin = userManager.Users.FirstOrDefault(u => u.SecurityStamp.Equals(userSecurityStamp));
                IFamily family = familyDatabaseService.GetFamilyByGuid(guid);
                familyDatabaseService.ChangeFamilyAdmin(family, newAdmin);
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Changing family administartor failed.");
                StatusMessage = $"Changing family administartor failed.";

                return View(nameof(FamiliesManagement), model);
            }

            logger.LogInformation($"Family administartor changed succesfully.");
            StatusMessage = $"You changed family administrator succesfully.";

            return View(nameof(FamiliesManagement), model);
        }

        [HttpGet]
        public async Task<IActionResult> RefuseFamilyAdmin(Guid guid)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            IFamilyDatabaseService familyDatabaseService = serviceProvider.GetService(typeof(IFamilyDatabaseService)) as IFamilyDatabaseService;
            IFamily family = familyDatabaseService.GetFamilyByGuid(guid);
            var model = new RefuseFamilyAdminViewModel()
            {
                StatusMessage = StatusMessage,
                FamilyMembers = familyDatabaseService.GetFamilyMemebers(family),
                FamilyRefuseGuid = guid
            };
            //var model = new FamiliesManagementViewModel
            //{
            //    StatusMessage = StatusMessage,
            //    FamiliesAdminOf = familyDatabaseService.GetUserAdminFamiliesWithMainImage(user),
            //    FamiliesMemberOf = familyDatabaseService.GetUserFamiliesMemberOfWithMainImage(user)
            //};

            return View(model);
        }

        /// <summary>
        /// TODO REFACTOR!!!
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="JSONModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> LeaveFamily(Guid guid)
        {
            ///// It is not a bad idea
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(FamiliesManagementViewModel));
            //FamiliesManagementViewModel yourobject = (FamiliesManagementViewModel)ser.ReadObject(UtilitiesLibrary.GenerateStreamFromString(JSONModel));

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            IFamilyDatabaseService familyDatabaseService = serviceProvider.GetService(typeof(IFamilyDatabaseService)) as IFamilyDatabaseService;
            var model = new FamiliesManagementViewModel
            {
                StatusMessage = StatusMessage,
                FamiliesAdminOf = familyDatabaseService.GetUserAdminFamiliesWithMainImage(user),
                FamiliesMemberOf = familyDatabaseService.GetUserFamiliesMemberOfWithMainImage(user)
            };

            Family family = familyDatabaseService.GetFamilyByGuid(guid) as Family;

            try
            {
                familyDatabaseService.RemoveFamilyMemeber(family, user);
            }
            catch (Exception ex)
            {
                return View(nameof(FamiliesManagement), model);
            }

            logger.LogInformation($"User left family {family.FamilyName} succesfully.");
            StatusMessage = $"You left family {family.FamilyName} succesfully.";

            return View(nameof(FamiliesManagement), model);
        }

        [HttpGet]
        public async Task<IActionResult> CreateFamily()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            IImageDatabaseService imageDatabaseService = serviceProvider.GetService(typeof(IImageDatabaseService)) as IImageDatabaseService;
            Guid imageGuid;
            Guid.TryParse(HttpContext.Session.GetString(user.SecurityStamp), out imageGuid);

            var model = new CreateFamilyViewModel
            {
                StatusMessage = StatusMessage,
                ImageModel = imageDatabaseService.GetFamilyByGuid(imageGuid) as ImageModel
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetFamily(CreateFamilyViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFamily(CreateFamilyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            IImageDatabaseService imageDatabaseService = serviceProvider.GetService(typeof(IImageDatabaseService)) as IImageDatabaseService;
            Guid imageGuid;
            Guid.TryParse(HttpContext.Session.GetString(user.SecurityStamp), out imageGuid);
            ImageModel familyMainImage = imageDatabaseService.GetFamilyByGuid(imageGuid) as ImageModel;

            Family family = new Family
            {
                FamilyAdministrator = user,
                FamilyName = model.FamilyName,
                MainImage = familyMainImage,
                Guid = Guid.NewGuid(),
            };

            familyMainImage.Family = family;
            family.FamilyUsers.Add(new FamilyUsers() { Family = family, User = user });

            IFamilyDatabaseService familyDatabaseService = serviceProvider.GetService(typeof(IFamilyDatabaseService)) as IFamilyDatabaseService;
            familyDatabaseService.AddFamily(family);
            familyDatabaseService.ChangeFamilyAdmin(family, user);

            //await _signInManager.SignInAsync(user, isPersistent: false);
            logger.LogInformation("User created family succesfully.");
            StatusMessage = "You created family successfully.";

            return RedirectToAction(nameof(FamiliesManagement));
        }

        [HttpPost]
        public ActionResult UploadFile(CreateFamilyViewModel model)
        {
            // Use Path.GetFileName to obtain the file name, which will
            // strip any path information passed as part of the
            // FileName property. HtmlEncode the result in case it must 
            // be returned in an error message.
            var fileName = WebUtility.HtmlEncode(
                Path.GetFileName(model.Image.FileName));

            var image = FileHelpers.ProcessFormFile(model.Image, logger, StatusMessage);

            var user = userManager.GetUserAsync(User);

            ImageModel imageModel = new ImageModel()
            {
                ImageData = image.Result,
                FileName = fileName,
                UploadedOn = DateTime.Now,
                Name = fileName,
                Guid = Guid.NewGuid(),
                User = user.Result
            };

            IImageDatabaseService imageDatabaseService = serviceProvider.GetService(typeof(IImageDatabaseService)) as IImageDatabaseService;
            imageDatabaseService.AddImageToDatabase(imageModel);

            HttpContext.Session.SetString(user.Result.SecurityStamp, imageModel.Guid.ToString());
            //string path = Server.MapPath("~/Uploads/");
            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}

            //if (postedFile != null)
            //{
            //    string fileName = Path.GetFileName(postedFile.FileName);
            //    postedFile.SaveAs(path + fileName);
            //    ViewBag.Message += string.Format("<b>{0}</b> uploaded.<br />", fileName);
            //}

            return RedirectToAction(nameof(CreateFamily));
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                urlEncoder.Encode("ImageStoreAndAnalyze"),
                urlEncoder.Encode(email),
                unformattedKey);
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user, EnableAuthenticatorViewModel model)
        {
            var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        #endregion
    }
}
