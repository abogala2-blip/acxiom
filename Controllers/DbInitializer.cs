using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Data
{
    public static class DbInitializer
    {
        // ========================================
        // CREATE DEFAULT ROLES
        // ========================================

        public static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
                "Admin",
                "Manager",
                "SalesExecutive"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role)
                    );
                }
            }
        }


        // ========================================
        // CREATE DEFAULT ADMIN USER
        // ========================================

        public static async Task SeedAdminUserAsync(
            UserManager<ApplicationUser> userManager)
        {
            string adminEmail = "admin@acxiomcrm.com";
            string adminPassword = "Admin@123";

            var existingUser =
                await userManager.FindByEmailAsync(adminEmail);

            if (existingUser == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    adminUser,
                    adminPassword
                );

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        adminUser,
                        "Admin"
                    );
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(
                    existingUser,
                    "Admin"))
                {
                    await userManager.AddToRoleAsync(
                        existingUser,
                        "Admin"
                    );
                }
            }
        }


        // ========================================
        // CREATE SAMPLE CUSTOMERS
        // ========================================

        public static async Task SeedCustomersAsync(
            ApplicationDbContext context)
        {
            var customers = new List<Customer>
            {
                new Customer
                {
                    FullName = "Rahul Sharma",
                    Email = "rahul.sharma@gmail.com",
                    Phone = "9876543210",
                    Company = "TechNova Solutions",
                    City = "Hyderabad",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Priya Reddy",
                    Email = "priya.reddy@gmail.com",
                    Phone = "9876543211",
                    Company = "Reddy Enterprises",
                    City = "Visakhapatnam",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Arjun Kumar",
                    Email = "arjun.kumar@gmail.com",
                    Phone = "9876543212",
                    Company = "Kumar Technologies",
                    City = "Bengaluru",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Sneha Rao",
                    Email = "sneha.rao@gmail.com",
                    Phone = "9876543213",
                    Company = "Rao Digital Solutions",
                    City = "Chennai",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Kiran Patel",
                    Email = "kiran.patel@gmail.com",
                    Phone = "9876543214",
                    Company = "Patel Industries",
                    City = "Mumbai",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Ananya Singh",
                    Email = "ananya.singh@gmail.com",
                    Phone = "9876543215",
                    Company = "Singh Consulting",
                    City = "Delhi",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Vikram Reddy",
                    Email = "vikram.reddy@gmail.com",
                    Phone = "9876543216",
                    Company = "VR Software Labs",
                    City = "Vijayawada",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Meghana Rao",
                    Email = "meghana.rao@gmail.com",
                    Phone = "9876543217",
                    Company = "Meghana Enterprises",
                    City = "Hyderabad",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Aditya Verma",
                    Email = "aditya.verma@gmail.com",
                    Phone = "9876543218",
                    Company = "Verma Technologies",
                    City = "Pune",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Divya Nair",
                    Email = "divya.nair@gmail.com",
                    Phone = "9876543219",
                    Company = "Nair Solutions",
                    City = "Kochi",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Rohit Mehta",
                    Email = "rohit.mehta@gmail.com",
                    Phone = "9876543220",
                    Company = "Mehta Corporation",
                    City = "Ahmedabad",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Pooja Iyer",
                    Email = "pooja.iyer@gmail.com",
                    Phone = "9876543221",
                    Company = "Iyer Business Group",
                    City = "Bengaluru",
                    Status = "Inactive"
                },

                new Customer
                {
                    FullName = "Sandeep Kumar",
                    Email = "sandeep.kumar@gmail.com",
                    Phone = "9876543222",
                    Company = "SK Innovations",
                    City = "Visakhapatnam",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Lakshmi Devi",
                    Email = "lakshmi.devi@gmail.com",
                    Phone = "9876543223",
                    Company = "Lakshmi Textiles",
                    City = "Tirupati",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Naveen Babu",
                    Email = "naveen.babu@gmail.com",
                    Phone = "9876543224",
                    Company = "NB Software Solutions",
                    City = "Hyderabad",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Keerthi Reddy",
                    Email = "keerthi.reddy@gmail.com",
                    Phone = "9876543225",
                    Company = "KR Marketing",
                    City = "Vijayawada",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Manoj Kumar",
                    Email = "manoj.kumar@gmail.com",
                    Phone = "9876543226",
                    Company = "MK Enterprises",
                    City = "Chennai",
                    Status = "Inactive"
                },

                new Customer
                {
                    FullName = "Swathi Rao",
                    Email = "swathi.rao@gmail.com",
                    Phone = "9876543227",
                    Company = "SR Technologies",
                    City = "Visakhapatnam",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Tarun Gupta",
                    Email = "tarun.gupta@gmail.com",
                    Phone = "9876543228",
                    Company = "Gupta Industries",
                    City = "Delhi",
                    Status = "Active"
                },

                new Customer
                {
                    FullName = "Neha Kapoor",
                    Email = "neha.kapoor@gmail.com",
                    Phone = "9876543229",
                    Company = "Kapoor Consulting",
                    City = "Mumbai",
                    Status = "Active"
                }
            };

            // Add only customers that don't already exist.
            // This keeps your existing Ananth customer.

            foreach (var customer in customers)
            {
                bool exists = await context.Customers
                    .AnyAsync(c => c.Email == customer.Email);

                if (!exists)
                {
                    customer.CreatedAt = DateTime.UtcNow;

                    await context.Customers.AddAsync(customer);
                }
            }

            await context.SaveChangesAsync();
        }
    }
}