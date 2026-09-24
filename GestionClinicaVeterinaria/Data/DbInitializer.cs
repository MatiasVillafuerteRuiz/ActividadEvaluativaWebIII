using GestionClinicaVeterinaria.Models;
using Microsoft.AspNetCore.Identity;

namespace GestionClinicaVeterinaria.Models
{
    public static class DbInitializer
    {
        public static async Task InicializarRolesAsync(
            IServiceProvider serviceProvider)
        {
            // =====================================================
            // OBTENER LOS SERVICIOS DE IDENTITY
            // =====================================================

            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


            // =====================================================
            // CREAR ROLES
            // =====================================================

            string[] roles =
            {
                "Administrador",
                "Cliente"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }


            // =====================================================
            // CREAR ADMINISTRADOR INICIAL
            // =====================================================

            string adminEmail = "admin@veterinaria.com";
            string adminPassword = "Admin123";

            var adminExistente =
                await userManager.FindByEmailAsync(adminEmail);

            if (adminExistente == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,

                    Nombre = "Administrador",
                    Apellido = "Sistema"
                };

                var resultado =
                    await userManager.CreateAsync(
                        admin,
                        adminPassword);

                if (resultado.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        admin,
                        "Administrador");
                }
                else
                {
                    foreach (var error in resultado.Errors)
                    {
                        Console.WriteLine(
                            $"Error creando administrador: {error.Description}");
                    }
                }
            }
            else
            {
                // Si el usuario ya existe pero todavía no tiene
                // el rol Administrador, se lo asignamos.

                if (!await userManager.IsInRoleAsync(
                    adminExistente,
                    "Administrador"))
                {
                    await userManager.AddToRoleAsync(
                        adminExistente,
                        "Administrador");
                }
            }
        }
    }
}