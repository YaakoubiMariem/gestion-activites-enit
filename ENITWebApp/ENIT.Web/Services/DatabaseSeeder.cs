using ENIT.BL.Entities;
using ENIT.BL.Enums;
using ENIT.DAL.Context;
using ENIT.Web.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ENIT.Web.Services;

public class DatabaseSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ENITDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await db.Database.EnsureCreatedAsync();

        foreach (var role in new[] { RoleNames.Student, RoleNames.DepartmentAdmin, RoleNames.SuperAdmin })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var officialDepartments = new[]
        {
            new Department
            {
                Name = "Genie Civil",
                Code = "GC",
                Description = "Departement dedie aux infrastructures, aux structures et a l'innovation dans la construction.",
                LogoPath = "/images/enit-logo.svg"
            },
            new Department
            {
                Name = "Genie Hydraulique et Environnement",
                Code = "GH",
                Description = "Departement axe sur les systemes hydrauliques, l'eau et les enjeux environnementaux.",
                LogoPath = "/images/enit-logo.svg"
            },
            new Department
            {
                Name = "Genie Electrique",
                Code = "GE",
                Description = "Departement couvrant les reseaux electriques, l'electronique de puissance et les systemes intelligents.",
                LogoPath = "/images/enit-logo.svg"
            },
            new Department
            {
                Name = "Genie Industriel",
                Code = "GI",
                Description = "Departement centre sur l'optimisation, la logistique, la qualite et la performance industrielle.",
                LogoPath = "/images/enit-logo.svg"
            },
            new Department
            {
                Name = "Genie Mecanique",
                Code = "GM",
                Description = "Departement axe sur la mecanique, l'energie, la conception et l'innovation industrielle.",
                LogoPath = "/images/enit-logo.svg"
            },
            new Department
            {
                Name = "Informatique et TIC",
                Code = "INFO",
                Description = "Departement consacre au logiciel, aux systemes d'information, a l'IA et aux technologies de l'information.",
                LogoPath = "/images/enit-logo.svg"
            },
            new Department
            {
                Name = "Telecommunications",
                Code = "TEL",
                Description = "Departement dedie aux reseaux, aux communications numeriques et aux systemes connectes.",
                LogoPath = "/images/enit-logo.svg"
            },
            new Department
            {
                Name = "Techniques Avancees",
                Code = "TA",
                Description = "Departement oriente vers la modelisation, les services et les approches avancees pour l'industrie.",
                LogoPath = "/images/enit-logo.svg"
            }
        };

        foreach (var department in officialDepartments)
        {
            var existingDepartment = await db.Departments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Code == department.Code);

            if (existingDepartment is null)
            {
                db.Departments.Add(department);
                continue;
            }

            existingDepartment.Name = department.Name;
            existingDepartment.Description = department.Description;
            existingDepartment.LogoPath = department.LogoPath;
            existingDepartment.IsDeleted = false;
        }

        await db.SaveChangesAsync();

        var departments = await db.Departments.OrderBy(x => x.Id).ToListAsync();
        var gc = GetDepartmentByCode(departments, "GC");
        var gh = GetDepartmentByCode(departments, "GH");
        var ge = GetDepartmentByCode(departments, "GE");
        var gi = GetDepartmentByCode(departments, "GI");
        var gm = GetDepartmentByCode(departments, "GM");
        var info = GetDepartmentByCode(departments, "INFO");
        var tel = GetDepartmentByCode(departments, "TEL");
        var ta = GetDepartmentByCode(departments, "TA");

        await EnsureUserAsync(userManager, "superadmin@enit.tn", "Super Administrateur", "0000", null, RoleNames.SuperAdmin);
        var infoAdmin = await EnsureUserAsync(userManager, "admin.info@enit.tn", "Responsable INFO", "INFO-ADM-001", info.Id, RoleNames.DepartmentAdmin);
        var giAdmin = await EnsureUserAsync(userManager, "admin.gi@enit.tn", "Responsable GI", "GI-ADM-001", gi.Id, RoleNames.DepartmentAdmin);
        var student1 = await EnsureUserAsync(userManager, "etudiant1@enit.tn", "Aymen Ben Salem", "INFO-2026-014", info.Id, RoleNames.Student);
        var student2 = await EnsureUserAsync(userManager, "etudiant2@enit.tn", "Nour El Houda Trabelsi", "GC-2026-022", gc.Id, RoleNames.Student);

        if (!await db.Activities.AnyAsync())
        {
            db.Activities.AddRange(
                new Activity
                {
                    Title = "Journée IA & Data Science",
                    Description = "Conférences, mini-ateliers et démonstrations autour de l'IA générative, de l'analyse prédictive et des applications académiques à l'ENIT.",
                    Type = ActivityType.Conference,
                    StartDate = DateTime.Today.AddDays(3).AddHours(9),
                    EndDate = DateTime.Today.AddDays(3).AddHours(16),
                    Location = "Amphithéâtre Central",
                    Capacity = 180,
                    Status = ActivityStatus.Published,
                    DepartmentId = info.Id,
                    CreatedById = infoAdmin.Id,
                    CoverImagePath = "/images/activity-ai.svg"
                },
                new Activity
                {
                    Title = "Bootcamp Lean Manufacturing",
                    Description = "Session immersive sur l'excellence opérationnelle, les KPI industriels et les méthodes Lean dans les systèmes de production.",
                    Type = ActivityType.Training,
                    StartDate = DateTime.Today.AddDays(7).AddHours(10),
                    EndDate = DateTime.Today.AddDays(7).AddHours(15),
                    Location = "Salle Innovation 2",
                    Capacity = 90,
                    Status = ActivityStatus.Published,
                    DepartmentId = gi.Id,
                    CreatedById = giAdmin.Id,
                    CoverImagePath = "/images/activity-industry.svg"
                },
                new Activity
                {
                    Title = "Hackathon Smart Campus",
                    Description = "Compétition inter-départements pour prototyper des solutions numériques au service d'un campus plus intelligent, durable et connecté.",
                    Type = ActivityType.Competition,
                    StartDate = DateTime.Today.AddDays(12).AddHours(8),
                    EndDate = DateTime.Today.AddDays(13).AddHours(18),
                    Location = "Learning Lab ENIT",
                    Capacity = 120,
                    Status = ActivityStatus.Published,
                    DepartmentId = info.Id,
                    CreatedById = infoAdmin.Id,
                    CoverImagePath = "/images/activity-campus.svg"
                },
                new Activity
                {
                    Title = "Séminaire BIM & Construction Durable",
                    Description = "Rencontre académique autour du BIM, des matériaux durables et des bonnes pratiques de gestion de projet dans le génie civil.",
                    Type = ActivityType.Seminar,
                    StartDate = DateTime.Today.AddDays(15).AddHours(9),
                    EndDate = DateTime.Today.AddDays(15).AddHours(13),
                    Location = "Bloc Civil - Salle C12",
                    Capacity = 80,
                    Status = ActivityStatus.Published,
                    DepartmentId = gc.Id,
                    CreatedById = giAdmin.Id,
                    CoverImagePath = "/images/activity-civil.svg"
                },
                new Activity
                {
                    Title = "Forum Smart Grids",
                    Description = "Rencontre autour des reseaux intelligents, de l'energie et des innovations en genie electrique.",
                    Type = ActivityType.Conference,
                    StartDate = DateTime.Today.AddDays(10).AddHours(9),
                    EndDate = DateTime.Today.AddDays(10).AddHours(15),
                    Location = "Bloc Electrique - Salle E05",
                    Capacity = 110,
                    Status = ActivityStatus.Published,
                    DepartmentId = ge.Id,
                    CreatedById = infoAdmin.Id,
                    CoverImagePath = "/images/activity-ai.svg"
                },
                new Activity
                {
                    Title = "Journee Eau et Environnement",
                    Description = "Ateliers, debats et sessions de sensibilisation autour des enjeux hydrauliques et environnementaux.",
                    Type = ActivityType.Seminar,
                    StartDate = DateTime.Today.AddDays(18).AddHours(9),
                    EndDate = DateTime.Today.AddDays(18).AddHours(14),
                    Location = "Bloc Hydraulique - Salle H03",
                    Capacity = 95,
                    Status = ActivityStatus.Published,
                    DepartmentId = gh.Id,
                    CreatedById = giAdmin.Id,
                    CoverImagePath = "/images/activity-campus.svg"
                });

            await db.SaveChangesAsync();
        }

        if (!await db.Participations.AnyAsync())
        {
            var activities = await db.Activities.OrderBy(x => x.Id).ToListAsync();
            db.Participations.AddRange(
                new Participation { ActivityId = activities[0].Id, UserId = student1.Id, Status = ParticipationStatus.Confirmed },
                new Participation { ActivityId = activities[0].Id, UserId = student2.Id, Status = ParticipationStatus.Confirmed },
                new Participation { ActivityId = activities[1].Id, UserId = student1.Id, Status = ParticipationStatus.Confirmed },
                new Participation { ActivityId = activities[2].Id, UserId = student2.Id, Status = ParticipationStatus.Waitlist });

            await db.SaveChangesAsync();
        }

        if (!await db.Notifications.AnyAsync())
        {
            db.Notifications.AddRange(
                new Notification
                {
                    Title = "Nouvelle activité publiée",
                    Message = "Le Hackathon Smart Campus est ouvert aux inscriptions.",
                    Type = NotificationType.NewActivity,
                    UserId = student1.Id
                },
                new Notification
                {
                    Title = "Rappel académique",
                    Message = "Votre activité Journée IA & Data Science commence dans 3 jours.",
                    Type = NotificationType.Reminder,
                    UserId = student2.Id
                });

            await db.SaveChangesAsync();
        }

        if (!await db.ActivityComments.AnyAsync())
        {
            var firstActivity = await db.Activities.OrderBy(x => x.Id).FirstAsync();
            db.ActivityComments.Add(new ActivityComment
            {
                ActivityId = firstActivity.Id,
                UserId = student1.Id,
                Content = "Très bonne initiative pour rapprocher les étudiants des sujets IA appliqués au contexte universitaire.",
                Rating = 5
            });

            await db.SaveChangesAsync();
        }
    }

    private static async Task<ApplicationUser> EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string fullName,
        string matricule,
        int? departmentId,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                Matricule = matricule,
                DepartmentId = departmentId,
                PhoneNumber = "00000000",
                ProfilePhotoPath = "/images/avatar-default.svg"
            };

            await userManager.CreateAsync(user, "P@ssword123!");
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }

        return user;
    }

    private static Department GetDepartmentByCode(IEnumerable<Department> departments, string code)
    {
        var department = departments.FirstOrDefault(x => x.Code == code);
        if (department is null)
        {
            throw new InvalidOperationException($"Le departement requis '{code}' est introuvable apres le seed.");
        }

        return department;
    }
}
