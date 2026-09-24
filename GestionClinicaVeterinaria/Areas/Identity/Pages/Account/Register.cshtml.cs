using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

using GestionClinicaVeterinaria.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace GestionClinicaVeterinaria.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        // =========================================================
        // SERVICIOS DE IDENTITY
        // =========================================================

        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }


        // =========================================================
        // DATOS DEL FORMULARIO
        // =========================================================

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }


        // =========================================================
        // INPUT MODEL
        // =========================================================

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre es obligatorio.")]
            [StringLength(
                50,
                ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; } = string.Empty;


            [Required(ErrorMessage = "El apellido es obligatorio.")]
            [StringLength(
                50,
                ErrorMessage = "El apellido no puede superar los 50 caracteres.")]
            [Display(Name = "Apellido")]
            public string Apellido { get; set; } = string.Empty;


            [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
            [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; } = string.Empty;


            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(
                100,
                ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.",
                MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = string.Empty;


            [Required(ErrorMessage = "Debe confirmar la contraseña.")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare(
                "Password",
                ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }


        // =========================================================
        // GET
        // =========================================================

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            ExternalLogins =
                (await _signInManager
                    .GetExternalAuthenticationSchemesAsync())
                    .ToList();
        }


        // =========================================================
        // POST - REGISTRAR USUARIO
        // =========================================================

        public async Task<IActionResult> OnPostAsync(
            string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins =
                (await _signInManager
                    .GetExternalAuthenticationSchemesAsync())
                    .ToList();


            // Si existen errores de validación,
            // regresamos al formulario.
            if (!ModelState.IsValid)
            {
                return Page();
            }


            // =====================================================
            // CREAR APPLICATION USER
            // =====================================================

            var user = CreateUser();

            user.Nombre = Input.Nombre;
            user.Apellido = Input.Apellido;


            // =====================================================
            // CONFIGURAR USERNAME Y EMAIL
            // =====================================================

            await _userStore.SetUserNameAsync(
                user,
                Input.Email,
                CancellationToken.None);

            await _emailStore.SetEmailAsync(
                user,
                Input.Email,
                CancellationToken.None);


            // =====================================================
            // CREAR USUARIO EN IDENTITY
            // =====================================================

            var result = await _userManager.CreateAsync(
                user,
                Input.Password);


            if (result.Succeeded)
            {
                _logger.LogInformation(
                    "Se creó una nueva cuenta de cliente.");


                // =================================================
                // ASIGNAR ROL CLIENTE
                // =================================================

                var resultadoRol =
                    await _userManager.AddToRoleAsync(
                        user,
                        "Cliente");


                // Si ocurre un problema asignando el rol,
                // mostramos los errores.
                if (!resultadoRol.Succeeded)
                {
                    foreach (var error in resultadoRol.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    return Page();
                }


                // =================================================
                // GENERAR CONFIRMACIÓN DE EMAIL
                // =================================================

                var userId =
                    await _userManager.GetUserIdAsync(user);

                var code =
                    await _userManager
                        .GenerateEmailConfirmationTokenAsync(user);

                code = WebEncoders.Base64UrlEncode(
                    Encoding.UTF8.GetBytes(code));


                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new
                    {
                        area = "Identity",
                        userId,
                        code,
                        returnUrl
                    },
                    protocol: Request.Scheme)!;


                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Confirma tu correo electrónico",
                    $"Confirma tu cuenta haciendo clic " +
                    $"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>" +
                    $"aquí</a>.");


                // =================================================
                // ¿SE REQUIERE CONFIRMACIÓN?
                // =================================================

                if (_userManager.Options
                    .SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage(
                        "RegisterConfirmation",
                        new
                        {
                            email = Input.Email,
                            returnUrl
                        });
                }


                // =================================================
                // INICIAR SESIÓN
                // =================================================

                await _signInManager.SignInAsync(
                    user,
                    isPersistent: false);

                return LocalRedirect(returnUrl);
            }


            // =====================================================
            // MOSTRAR ERRORES DE IDENTITY
            // =====================================================

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return Page();
        }


        // =========================================================
        // CREAR INSTANCIA DE APPLICATION USER
        // =========================================================

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator
                    .CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException(
                    $"No se pudo crear una instancia de " +
                    $"'{nameof(ApplicationUser)}'. " +
                    $"Verifique que tenga un constructor vacío.");
            }
        }


        // =========================================================
        // OBTENER EMAIL STORE
        // =========================================================

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException(
                    "Identity requiere un almacén de usuarios " +
                    "con soporte para correo electrónico.");
            }

            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}