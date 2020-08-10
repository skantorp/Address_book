using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Address_Book.Models;
using DataBase;
using Address_Book.ViewModels;
using DataBase.DTO;

namespace Address_Book.Controllers
{
    public class HomeController : Controller
    {
        AddressDataBase dataBase = new AddressDataBase();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult AllUsers()
        {
            var usersViewModel = new List<UserViewModel>();

            var users = dataBase.GetAllUsers();

            foreach (UserDTO user in users)
            {
                List<PhoneViewModel> userPhonesViewModel = new List<PhoneViewModel>();
                List<PhoneDTO> userPhones = dataBase.GetPhonesByUser(user.Id);

                foreach (PhoneDTO phone in userPhones)
                {
                    userPhonesViewModel.Add(new PhoneViewModel
                    {
                        Id = phone.Id,
                        IsActive = phone.IsActive,
                        Number = phone.Number
                    });
                }

                usersViewModel.Add(new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Phones = userPhonesViewModel
                });
            }

            return View("ViewUsers", usersViewModel);
        }

        public IActionResult UsersWithAtLeastOneNumber()
        {
            var usersViewModel = new List<UserViewModel>();

            var users = dataBase.GetAllUsers();

            foreach (UserDTO user in users)
            {
                List<PhoneViewModel> userPhonesViewModel = new List<PhoneViewModel>();
                List<PhoneDTO> userPhones = dataBase.GetPhonesByUser(user.Id);

                if (userPhones.Count > 0)
                {
                    foreach (PhoneDTO phone in userPhones)
                    {
                        userPhonesViewModel.Add(new PhoneViewModel
                        {
                            Id = phone.Id,
                            IsActive = phone.IsActive,
                            Number = phone.Number
                        });
                    }

                    usersViewModel.Add(new UserViewModel
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Phones = userPhonesViewModel
                    });
                }
            }

            return View("ViewUsers", usersViewModel);
        }

        public IActionResult UsersWithOnlyActiveNumbers()
        {
            var usersViewModel = new List<UserViewModel>();

            var users = dataBase.GetAllUsers();

            foreach (UserDTO user in users)
            {
                List<PhoneViewModel> userPhonesViewModel = new List<PhoneViewModel>();
                List<PhoneDTO> userPhones = dataBase.GetPhonesByUser(user.Id);

                if (userPhones.Any() && userPhones.All(x => x.IsActive))
                {
                    foreach (PhoneDTO phone in userPhones)
                    {
                        userPhonesViewModel.Add(new PhoneViewModel
                        {
                            Id = phone.Id,
                            IsActive = phone.IsActive,
                            Number = phone.Number
                        });
                    }

                    usersViewModel.Add(new UserViewModel
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Phones = userPhonesViewModel
                    });
                }
            }

            return View("ViewUsers", usersViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
